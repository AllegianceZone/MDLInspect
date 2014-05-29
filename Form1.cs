using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MDL;
using System.Globalization;
using System.IO;

namespace MDLUtility2
{
    public partial class Form1 : Form
    {
        MDLFile currentBinaryMDL;
        System.IO.FileInfo currentFile;
        System.IO.DirectoryInfo currentDirectory;
        System.IO.FileInfo[] fiarr;
        Bitmap currentBitmap = null;
        Dictionary<string, string> currentReferencedFiles;
        string windowTitle = "";

        bool optionsButtonFocus = false;

        public Form1()
        {
            InitializeComponent();
            if (Properties.Settings.Default.LastDirectory != "")
            {
                currentDirectory = new System.IO.DirectoryInfo(Properties.Settings.Default.LastDirectory);
                if (currentDirectory.Exists)
                {
                    SetDirectory(currentDirectory.FullName);
                }
            }
            rememberZoomLevelToolStripMenuItem.Checked = Properties.Settings.Default.RememberZoom;
            useQuickZoomToolStripMenuItem.Checked = Properties.Settings.Default.UseQuickZoom;
            windowTitle = this.Text;
            this.Icon = Properties.Resources.He_he;
        }

        private void btnpickDirectory_Click(object sender, EventArgs e)
        {            
            if(currentDirectory != null)
            {
            folderBrowserDialog1.SelectedPath = currentDirectory.FullName;
            }
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.LastDirectory = folderBrowserDialog1.SelectedPath;
                currentDirectory = new System.IO.DirectoryInfo(Properties.Settings.Default.LastDirectory);
                Properties.Settings.Default.Save();
                SetDirectory(currentDirectory.FullName);

                this.Text = windowTitle;
            }
       }

        private void SetDirectory(string directory)
        {
            pictureBox1.Image = null;
            btnSaveAs.Enabled = false;
            listBox1.Items.Clear();
            toolStripStatusLabel1.Text = directory;
            fiarr = currentDirectory.GetFiles("*.mdl", System.IO.SearchOption.TopDirectoryOnly);
            listBox1.Items.AddRange(fiarr);
            fiarr = currentDirectory.GetFiles("*.bmp", System.IO.SearchOption.TopDirectoryOnly);
            listBox1.Items.AddRange(fiarr);
            fiarr = currentDirectory.GetFiles("*.png", System.IO.SearchOption.TopDirectoryOnly);
            listBox1.Items.AddRange(fiarr);
            fiarr = currentDirectory.GetFiles("*.jpg", System.IO.SearchOption.TopDirectoryOnly);
            listBox1.Items.AddRange(fiarr);
            fiarr = currentDirectory.GetFiles("*.jpeg", System.IO.SearchOption.TopDirectoryOnly);
            listBox1.Items.AddRange(fiarr);
            fiarr = currentDirectory.GetFiles("*.emf", System.IO.SearchOption.TopDirectoryOnly);
            listBox1.Items.AddRange(fiarr);
            fiarr = currentDirectory.GetFiles("*.exif", System.IO.SearchOption.TopDirectoryOnly);
            listBox1.Items.AddRange(fiarr);
            fiarr = currentDirectory.GetFiles("*.gif", System.IO.SearchOption.TopDirectoryOnly);
            listBox1.Items.AddRange(fiarr);
            fiarr = currentDirectory.GetFiles("*.ico", System.IO.SearchOption.TopDirectoryOnly);
            listBox1.Items.AddRange(fiarr);
            fiarr = currentDirectory.GetFiles("*.tiff", System.IO.SearchOption.TopDirectoryOnly);
            listBox1.Items.AddRange(fiarr);
            fiarr = currentDirectory.GetFiles("*.wmf", System.IO.SearchOption.TopDirectoryOnly);
            listBox1.Items.AddRange(fiarr);
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 1)
            {
                // Set the text on our save button to say batch
                btnSaveAs.Text = "Save Batch As";
                return; // no need to change the image displayed when they multiselect.
            }
            if (listBox1.SelectedItem == null)
            {
                btnSaveAs.Enabled = false;
                return;
            }
            else
            {
                // Set the text on our save button to say batch
                btnSaveAs.Text = "Save As";
                btnSaveAs.Enabled = true;
            }
            if (listBox1.SelectedItem.GetType() == typeof(System.IO.FileInfo))
            {
                currentFile = listBox1.SelectedItem as System.IO.FileInfo;
                System.IO.FileInfo fi = currentFile;
                SetViewImage(fi,"");
            }
        }

        private void lstInternalImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null && listBox1.SelectedItem.GetType() == typeof(System.IO.FileInfo))
            {
                if (lstInternalImages.SelectedItem != null && lstInternalImages.SelectedItem.GetType() == typeof(string))
                {
                    currentFile = listBox1.SelectedItem as System.IO.FileInfo;
                    System.IO.FileInfo fi = currentFile;
                    SetViewImage(fi, lstInternalImages.SelectedItem as string);
                }
            }
        }

        private void SetViewImage(System.IO.FileInfo fi,string requestedName)
        {
            if (fi != null)
            {
                lblInternalName.Text = "";
                lblInternalReference.Text = "";
                string varname = "";
                fileType ft = LoadFile(fi, requestedName, out varname, out currentBitmap); 

                pictureBox1.Image = currentBitmap;

                switch (ft)
                {
                    case fileType.Image:
                        Viewer1.Visible = false; Viewer1.Dock = DockStyle.None;
                        lstInternalImages.Visible = false; lstInternalImages.Dock = DockStyle.None; 
                        txtBinaryHeader.Dock = DockStyle.Fill; txtBinaryHeader.Visible = true; 
                        pbPreviewImage.Dock = DockStyle.Fill; pbPreviewImage.Visible = true;
                        WriteImageFileInfo();
                        break;
                    case fileType.TextMdl:
                        Viewer1.Visible = false; Viewer1.Dock = DockStyle.None;
                        txtBinaryHeader.Visible = false; txtBinaryHeader.Dock = DockStyle.None;
                        lstInternalImages.Dock = DockStyle.Fill; lstInternalImages.Visible = true; 
                        pbPreviewImage.Dock = DockStyle.Fill; pbPreviewImage.Visible = true;

                        // deteremine if we need to reload the list.
                        WriteTextFileInfo(varname);
                        break;
                    case fileType.ImageMdl:
                        Viewer1.Visible = false; Viewer1.Dock = DockStyle.None;
                        lstInternalImages.Visible = false; lstInternalImages.Dock = DockStyle.None;
                        txtBinaryHeader.Dock = DockStyle.Fill; txtBinaryHeader.Visible = true; 
                        pbPreviewImage.Dock = DockStyle.Fill; pbPreviewImage.Visible = true;
                        WriteBinaryFileInfo(currentBinaryMDL.header.ImportedSymbols[0].Name);
                        break;
                    case fileType.ModelMdl:
                        pbPreviewImage.Visible = false; pbPreviewImage.Dock = DockStyle.None;
                        lstInternalImages.Visible = false; lstInternalImages.Dock = DockStyle.None;
                        WriteBinaryFileInfo(currentBinaryMDL.header.NameSpaces[0]);
                        if(currentBinaryMDL.RootObject.Where(x => x.type == MDLType.mdl_mesh).Count()>0)
                        {
                            // Lets load up any referenced Images Namespaces
                            List<Bitmap> referencedImages = GetReferencedImages(currentBinaryMDL);
                            // Lets send our file to the 3DViewer!!
                            MDLViewer mv = Viewer1.Child as MDLViewer;
                            mv.LoadMDLFile(currentBinaryMDL,referencedImages);
                        }

                        txtBinaryHeader.Dock = DockStyle.Fill; txtBinaryHeader.Visible = true; 
                        Viewer1.Dock = DockStyle.Fill; Viewer1.Visible = true;
                        break;
                    default:
                        break;
                }


                if (currentBitmap != null)
                {
                    if (ft != fileType.ModelMdl)
                    {
                        lblDetails.Text = string.Format("w:{0} h:{1} {2}", currentBitmap.Width, currentBitmap.Height, currentBitmap.PixelFormat.ToString());
                        // fill info on the details tab.                
                        lblFormat.Text = "Pixel: " + currentBitmap.PixelFormat.ToString();
                        lblWidth.Text = "Width: " + currentBitmap.Width.ToString();
                        lblHeight.Text = "Height: " + currentBitmap.Height.ToString();
                        if (numericUpDown1.Value == 100 || Properties.Settings.Default.RememberZoom == false)
                        {
                            pictureBox1.Width = pictureBox1.Image.Width;
                            pictureBox1.Height = pictureBox1.Image.Height;
                            numericUpDown1.Value = 100;
                        }
                        else
                        {
                            pictureBox1.Image = Scale(currentBitmap, Convert.ToSingle(numericUpDown1.Value) / 100, Convert.ToSingle(numericUpDown1.Value) / 100);
                            pictureBox1.Width = pictureBox1.Image.Width;
                            pictureBox1.Height = pictureBox1.Image.Height;
                        }
 
                        pbPreviewImage.Image = pictureBox1.Image;
                    }
                }
                else
                {
                    lblDetails.Text = "No Images Found";
                    // fill info on the details tab.
                    lblFormat.Text = "Pixel: " ;
                    lblWidth.Text = "Width: ";
                    lblHeight.Text = "Height: ";
                    lstInternalImages.Items.Clear();
                    pictureBox1.Image = null;
                    pbPreviewImage.Image = null;
                }
                this.Text = windowTitle + " .. " + currentFile.Name;
                // the status label
                lblFileName.Text = "File Name: " + currentFile.Name;
                
            }
        }

        private void WriteImageFileInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("PixelFormat: {0}", currentBitmap.PixelFormat.ToString()));
            sb.AppendLine(string.Format("PhysicalDimension: {0}", currentBitmap.PhysicalDimension.ToString()));
            sb.AppendLine(string.Format("Size: {0}", currentBitmap.Size.ToString()));
            sb.AppendLine(string.Format("Width: {0}", currentBitmap.Width.ToString()));
            sb.AppendLine(string.Format("Height: {0}", currentBitmap.Height.ToString()));
            sb.AppendLine(string.Format("HorizontalResolution: {0}", currentBitmap.HorizontalResolution.ToString()));
            sb.AppendLine(string.Format("VerticalResolution: {0}", currentBitmap.VerticalResolution.ToString()));
            sb.AppendLine(string.Format("Flags: {0}", currentBitmap.Flags));
            sb.AppendLine(string.Format("RawFormat: {0}", currentBitmap.RawFormat.ToString()));
            sb.AppendLine(string.Format("Property Items::"));
            for (int i = 0; i < currentBitmap.PropertyItems.Count(); i++)
            {
                sb.AppendLine(string.Format("\t Type: {0}, Id: {1}, Len: {2}, Value: {3},", currentBitmap.PropertyItems[i].Type.ToString()
                                                        , currentBitmap.PropertyItems[i].Id
                                                        , currentBitmap.PropertyItems[i].Len
                                                        , currentBitmap.PropertyItems[i].Value));    
            }
            

            txtBinaryHeader.Text = sb.ToString();
        }

        private void WriteTextFileInfo(string varname)
        {
            // advoid reloading the list if we just clicked a different
            // item in the internalImages list.
            if (currentReferencedFiles != null)
            {
                if (lstInternalImages.Items.Count != currentReferencedFiles.Count)
                {
                    lstInternalImages.Items.Clear();
                    foreach (KeyValuePair<string, string> item in currentReferencedFiles)
                    {
                        lstInternalImages.Items.Add(item.Key);
                    }
                }


                // get the selected Item, and display its info.
                if (varname != "")
                {
                    if(currentReferencedFiles.ContainsKey(varname))
                    {
                        var selectedItem = currentReferencedFiles.FirstOrDefault(x => x.Key.ToLower() == varname.ToLower());
                        lblInternalName.Text = "Internal Name: " + selectedItem.Key;
                        lblInternalReference.Text = "References: " + selectedItem.Value.Replace(toolStripStatusLabel1.Text, "[CURRENT-PATH]");
                        toolTip1.SetToolTip(lblInternalReference, selectedItem.Value);
                    }
                }
            }
        }

        private List<Bitmap> GetReferencedImages(MDLFile currentBinaryMDL)
        {
            List<Bitmap> retunvalue = new List<Bitmap>();
            List<string> possibleNS = currentBinaryMDL.header.NameSpaces.Where(x=>x!="model"&& x!="effect").ToList();
	        // lets loop over our possible NS
            foreach (var imageNameSpace in possibleNS)
	        {
                IEnumerable<FileInfo> myFiles = listBox1.Items.OfType<FileInfo>();
                foreach (var fileInfo in myFiles.Where(x => x.Name.ToLower() == imageNameSpace.ToLower() + ".mdl"))
                {
                    // lets try to load an image out of this file.
                    string fName = ""; // The namespace we got out of the file.
                    Bitmap resultImage; // The image that we loaded from the file.

                    fileType ft = LoadImage(fileInfo, imageNameSpace, out fName, out resultImage);
                    if (resultImage != null)
                    {// if we loaded an image lets add it to our list.
                        retunvalue.Add(resultImage);
                    }
                }
	        }
            return retunvalue;            
        }

        private void WriteBinaryFileInfo(string varname)
        {

            // DISPLAY BINARY MDL INFO
            if (currentBinaryMDL != null && lstInternalImages.Visible == false)
            {
                CultureInfo ci = new CultureInfo("en-us");
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("---Binary MDL---");
                sb.AppendLine("--Header--");
                sb.AppendLine("MDLMagic: " + currentBinaryMDL.header.MDLMAGIC.ToString("X",ci));
                sb.AppendLine("MDLVersion: " + currentBinaryMDL.header.MDLVersion.ToString("X", ci));
                sb.AppendLine("Imported Libs: " + currentBinaryMDL.header.ImportedNameSpacesCount.ToString());
                sb.AppendLine("Imported Symbols: " + currentBinaryMDL.header.ImportedSymoblCount.ToString());
                sb.AppendLine("Exported Symbols: " + currentBinaryMDL.header.ExportedSymbolCount.ToString());
                sb.AppendLine("Others Objs: " + currentBinaryMDL.header.OtherObjectsCount.ToString());

                sb.AppendLine("");
                sb.AppendLine("--Imported Libraries:" + currentBinaryMDL.header.NameSpaces.Count);
                foreach (string item in currentBinaryMDL.header.NameSpaces)
                {
                    sb.AppendLine("\t NameSpace:" + item);
                }
                sb.AppendLine("--Imported Symbols:" + currentBinaryMDL.header.ImportedSymbols.Count);
                foreach (SymbolPair item in currentBinaryMDL.header.ImportedSymbols)
                {
                    sb.AppendFormat("\t Sym:[{0},{1}] {2}", item.Name, item.Value, Environment.NewLine);
                }
                sb.AppendLine("--Exported Symbols:" + currentBinaryMDL.header.ExportedSymbols.Count);
                foreach (string item in currentBinaryMDL.header.ExportedSymbols)
                {
                    sb.AppendLine("\t Sym: " + item);
                }
                sb.AppendLine("--Exported Symbol Data:" + currentBinaryMDL.Textures.Count);
                foreach (var item in currentBinaryMDL.RootObject.Where(x=>x.type == MDLType.mdl_image).Select(x=>x.image))
                {
                    sb.AppendFormat("Data:[MDLImage]: {0}",  Environment.NewLine);

                    sb.AppendLine("\t Width: " + item.ImageHeader.width.ToString());
                    sb.AppendLine("\t Height: " + item.ImageHeader.height.ToString());
                    sb.AppendLine("\t Pitch: " + item.ImageHeader.pitch.ToString());
                    sb.AppendLine("\t Bpp: " + item.ImageHeader.bitCount.ToString());
                    sb.AppendLine("\t RedMask: " + item.ImageHeader.redMask.ToString());
                    sb.AppendLine("\t GreenMask: " + item.ImageHeader.greenMask.ToString());
                    sb.AppendLine("\t BlueMask: " + item.ImageHeader.blueMask.ToString());
                    sb.AppendLine("\t AlphaMask: " + item.ImageHeader.alphaMask.ToString());
                    sb.AppendLine("\t ColorKey: " + item.ImageHeader.UseColorKey.ToString());
                    
                }
                if (currentBinaryMDL.Lights != null)
                {
                    sb.AppendFormat("Data:[MDLLights]: {0}", Environment.NewLine);
                    foreach (var item in currentBinaryMDL.Lights)
                    {
                        sb.AppendLine(string.Format("\t Light: speed({0}) color({1},{2},{3}) pos({4},{5},{6})", Convert.ToInt32(item.red * 255), Convert.ToInt32(item.green * 255), Convert.ToInt32(item.blue * 255), item.speed.ToString(), item.posx, item.posy, item.posz));
                    }
                }
                if (currentBinaryMDL.FrameDatas!= null)
                {
                    sb.AppendFormat("Data:[MDLFrames]: {0}", Environment.NewLine);
                    foreach (var item in currentBinaryMDL.FrameDatas)
                    {
                        sb.AppendLine(string.Format("\t Frame: {0} pos({1},{2},{3}) n({4},{5},{6}) p({7},{8},{9})", item.name ,item.posx,item.posy,item.posz,item.nx,item.ny,item.nz,item.px,item.py,item.pz));
                    }
                }
                if (currentBinaryMDL.Textures.Count > 0)
                {
                    sb.AppendFormat("Data:[MDLTexture]: {0}", Environment.NewLine);
                    foreach (var item in currentBinaryMDL.Textures)
                    {
                        sb.AppendLine(string.Format("\t Texture: {0} ref: {1}", item.Key, item.Value));
                    }
                }

                for (int i = 0; i < currentBinaryMDL.RootObject.Count; i++)
			    {
                    MDLObject obj = currentBinaryMDL.RootObject[i];
			        if(obj.type == MDLType.mdl_mesh)
                    {
                        sb.AppendFormat("Data:[MDLMesh]: {0}", Environment.NewLine);
                        sb.AppendLine("\t Faces: " + obj.mesh.faces.Count());
                        sb.AppendLine("\t NFaces: " + obj.mesh.nfaces.ToString());
                        sb.AppendLine("\t NVertex: " + obj.mesh.nvertex.ToString());
                        sb.AppendLine("\t VerticiesLoaded: " + obj.mesh.vertices.Count());
                        sb.AppendLine("\t TextureIndex: " + obj.textidx.ToString());
                        if (obj.textidx > -1 && currentBinaryMDL.Textures.Count > obj.textidx)
                        {
                            sb.AppendLine(string.Format("\t\t Maps to Texture({0})", currentBinaryMDL.Textures.ElementAt(obj.textidx)));
                        }
                        else
                        {
                            sb.AppendLine(string.Format("\t\t Maps to Texture( Error! - BAD Index - No Texture at Index {0})", obj.textidx.ToString()));
                        }
                        sb.AppendLine("\t LOD: " + obj.lodval.ToString());
                    }
			    }

                txtBinaryHeader.Text = sb.ToString();
                txtBinaryHeader.ScrollBars = ScrollBars.Both;
            }
        }

        enum fileType
        {
            TextMdl,
            ImageMdl,
            ModelMdl,
            Image
        }

        private fileType LoadFile(System.IO.FileInfo fi, string requestedName, out string varname, out Bitmap output)
        {
            fileType returnType = fileType.ImageMdl;
            varname = "";
            output = null;
            if (fi.Name.EndsWith(".mdl"))
            {
                MDL.MDLFile test = new MDL.MDLFile();
                bool valid = test.ReadFromFile(fi.FullName);
                if (valid)
                { 
                    currentBinaryMDL = test;
                    returnType = fileType.ModelMdl;
                    if (test.RootObject.Where(x=>x.type == MDLType.mdl_image).Count() >0)
                    {
                        output = test.RootObject.First(x=>x.type == MDLType.mdl_image).image.myBitmap;
                        returnType = fileType.ImageMdl;
                    }                
                }
                else
                { 
                    currentBinaryMDL = null; 
                    System.IO.FileStream fs;
                    try
                    {
                        fs = fi.OpenRead();
                        using (System.IO.BinaryReader br = new System.IO.BinaryReader(fs))
                        {
                            br.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                            output = LoadTextBmpMDL(fi, out currentReferencedFiles, requestedName, out varname);
                            returnType = fileType.TextMdl;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }                
            }
            else
            {
                returnType = fileType.Image;
                currentBinaryMDL = null;
                output = (Bitmap)Bitmap.FromFile(fi.FullName);
            }

            return returnType;
        }
        private fileType LoadImage(System.IO.FileInfo fi, string requestedName, out string varname, out Bitmap output)
        {
            fileType returnType = fileType.ImageMdl;
            varname = "";
            output = null;
            if (fi.Name.EndsWith(".mdl"))
            {
                MDL.MDLFile test = new MDL.MDLFile();
                bool valid = test.ReadFromFile(fi.FullName);
                if (valid)
                { 
                    returnType = fileType.ModelMdl;
                    if (test.RootObject.Where(x => x.type == MDLType.mdl_image).Count() > 0)
                    {
                        output = test.RootObject.First(x => x.type == MDLType.mdl_image).image.myBitmap;
                        returnType = fileType.ImageMdl;
                    }
                }
                else
                {
                    System.IO.FileStream fs;
                    try
                    {
                        fs = fi.OpenRead();
                        using (System.IO.BinaryReader br = new System.IO.BinaryReader(fs))
                        {
                            br.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
                            output = LoadTextBmpMDL(fi, out currentReferencedFiles, requestedName, out varname);
                            returnType = fileType.ImageMdl;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            else
            {
                returnType = fileType.Image;
                output = (Bitmap)Bitmap.FromFile(fi.FullName);
            }

            return returnType;
        }


        private Bitmap LoadTextBmpMDL(System.IO.FileInfo bmpMdlFile, out Dictionary<string,string> referencedFiles,string requestedName,out string varname)
        {
            //use "model"; 
            //fx09bmp = ImportImageFromFile("Textures/Effects/mycoolshieldfx.png", true);
            string[] fileLines = System.IO.File.ReadAllLines(bmpMdlFile.FullName);
            Regex containsValidImportImageStatement = new Regex(@"(?<=ImportImage.*\(\"").*(?=\"")", RegexOptions.None);
            referencedFiles = new Dictionary<string,string>();
            for (int i = 0; i < fileLines.Length; i++)
            {
                Match regM = containsValidImportImageStatement.Match(fileLines[i]);
                if (regM.Success)
                {
                    string[] splitString = fileLines[i].Split(new string[] { "ImportImage" }, StringSplitOptions.None);
                    string containingVariable = splitString[0].TrimStart(' ').TrimEnd(' ');
                    int j = i;
                    while (containingVariable.Contains("=")==false && j > 0)
                    {
                        // lets prepend the previous line.
                        containingVariable = fileLines[--j].TrimStart(' ').TrimEnd(' ') + containingVariable;
                    }

                    int k = 1;
                    if (referencedFiles.ContainsKey(containingVariable))
                    {
                        while (referencedFiles.ContainsKey((k).ToString() + containingVariable))
                        {
                            k++;
                        }
                        containingVariable = (k).ToString() + containingVariable;
                    }
                    string filenameReferenced =  regM.Value.Replace("/", "\\");
                    if(filenameReferenced.EndsWith("bmp") && !filenameReferenced.Contains("."))
                    {
                        filenameReferenced += ".mdl";
                    }
                    filenameReferenced = bmpMdlFile.Directory + "\\" +filenameReferenced;
                    referencedFiles.Add(containingVariable, filenameReferenced);
                }
            }
            foreach (string line in fileLines)
            {
                
            }
            // for now lets just load the first match if there was one.
            Bitmap output = null;
            varname = "";
            if (referencedFiles.Count > 0)
            {
                foreach (KeyValuePair<string, string> item in referencedFiles)
                {
                    if (requestedName == "" || item.Key.ToLower() == requestedName.ToLower())
                    {
                        varname = item.Key;
                        string otherVarname = "";
                        fileType ft = LoadFile(new System.IO.FileInfo(item.Value), requestedName, out otherVarname, out output);// (Bitmap)Bitmap.FromFile(item.Value);
                        break;
                    }
                }
            }
            else
            {
                referencedFiles = null;
            }
            return output;
        }


        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if(currentBitmap !=null)
            {
            NumericUpDown sndr = sender as NumericUpDown;
                if (sndr != null)
                {
                    pictureBox1.Image = Scale(currentBitmap,Convert.ToSingle(sndr.Value) / 100,Convert.ToSingle(sndr.Value) / 100);
                    pictureBox1.Width = pictureBox1.Image.Width;
                    pictureBox1.Height = pictureBox1.Image.Height;
                }
            }

        }

        public static Bitmap Scale(Bitmap Bitmap, float ScaleFactorX, float ScaleFactorY)
        {
            int scaleWidth = (int)Math.Max(Bitmap.Width * ScaleFactorX, 1.0f);
            int scaleHeight = (int)Math.Max(Bitmap.Height * ScaleFactorY, 1.0f);

            Bitmap scaledBitmap = new Bitmap(scaleWidth, scaleHeight);

            // Scale the bitmap in high quality mode.
            using (Graphics gr = Graphics.FromImage(scaledBitmap))
            {
                if (Properties.Settings.Default.UseQuickZoom)
                {
                    gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                    gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                    gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                    gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                }
                else
                {
                    gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                }
                gr.DrawImage(Bitmap, new Rectangle(0, 0, scaleWidth, scaleHeight), new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), GraphicsUnit.Pixel);
            }

            // Copy original Bitmap's EXIF tags to new bitmap.
            foreach (PropertyItem propertyItem in Bitmap.PropertyItems)
            {
                scaledBitmap.SetPropertyItem(propertyItem);
            }

            return scaledBitmap;
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            HScrollBar sndr = sender as HScrollBar;
            if (sndr != null)
            {
                if(pictureBox1.Width - sndr.Parent.Width>0)
                sndr.Maximum = pictureBox1.Width - sndr.Parent.Width;
                pictureBox1.Left = 0 - sndr.Value;
            }
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            VScrollBar sndr = sender as VScrollBar;
            if (sndr != null)
            {
                if(pictureBox1.Height - sndr.Parent.Height>0)
                sndr.Maximum = pictureBox1.Height - sndr.Parent.Height;
                pictureBox1.Top = 0 - sndr.Value;
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 1)
            {
                List<System.IO.FileInfo> files = new List<System.IO.FileInfo>();
                foreach (object item in listBox1.SelectedItems)
                {
                    if (item.GetType() == typeof(System.IO.FileInfo))
                    {
                        files.Add((System.IO.FileInfo)item);
                    }
                }                
                
                // save all the images here.
                string fileNamePattern = "";
                ImageFormat resType = GetBatchNameAndType("png",out fileNamePattern,currentDirectory.FullName);

                if (resType != null)
                {
                    foreach (System.IO.FileInfo fi in files)
                    {
                        string curName = fi.Name.Replace(fi.Extension, "");
                        string newName = fileNamePattern.Replace("[currentname]", curName);
                        //newName += "."+resType.ToString();
                        string tmp;
                        Bitmap curBitmap;
                        fileType ft = LoadFile(fi,"",out tmp,out curBitmap);
                        curBitmap.Save(newName, resType);
                    }
                }
            }
            else if (listBox1.SelectedItem!=null && listBox1.SelectedItem.GetType() == typeof(System.IO.FileInfo))
            {
                string ext = ((System.IO.FileInfo)listBox1.SelectedItem).Extension.ToLower();
                string fname = ((System.IO.FileInfo)listBox1.SelectedItem).Name;
                string cDir = currentDirectory.FullName;
                Bitmap btmap = currentBitmap;

                SaveImageAs(ext, fname, cDir, btmap);
            }
        }

        private ImageFormat GetBatchNameAndType(string ext,out string fname,string cDir)
        {
            fname = "[currentname].png";
            saveFileDialog1.InitialDirectory = cDir;
            saveFileDialog1.FileName = fname;
            if (ext == ".mdl")
                saveFileDialog1.FileName = saveFileDialog1.FileName.Replace(ext, ".png");
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.Filter = "PNG Image (*.png)|*.png"
                                     + "|JPEG Image (*.jpg)|*.jpg"
                                     + "|Bitmap Image (*.bmp)|*.bmp"
                                     + "|Enhanced Metafile Image (*.emf)|*.emf"
                                     + "|Exchangable Image File (*.exif)|*.exif"
                                     + "|Graphics Interchange File (*.gif)|*.gif"
                                     + "|Windows Icon Image (*.ico)|*.ico"
                                     + "|Tagged Image File Format (*.tiff)|*.tiff"
                                     + "|Windows Metafile Image Format (*.wmf)|*.wmf"
                                     ;//|Allegiance binary bmp.mdl Image (*.mdl)|*.mdl|Allegiance text bmp.mdl Image (*.mdl)|*.mdl";
            saveFileDialog1.AddExtension = true;
            if (ext == ".png")
                saveFileDialog1.FilterIndex = 1;
            if (ext == ".jpg")
                saveFileDialog1.FilterIndex = 2;
            if (ext == ".bmp")
                saveFileDialog1.FilterIndex = 3;
            if (ext == ".emf")
                saveFileDialog1.FilterIndex = 4;
            if (ext == ".exif")
                saveFileDialog1.FilterIndex = 5;
            if (ext == ".gif")
                saveFileDialog1.FilterIndex = 6;
            if (ext == ".ico")
                saveFileDialog1.FilterIndex = 7;
            if (ext == ".tiff")
                saveFileDialog1.FilterIndex = 8;
            if (ext == ".wmf")
                saveFileDialog1.FilterIndex = 9;
            ImageFormat iff = ImageFormat.Png;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fname = saveFileDialog1.FileName;
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1: iff = ImageFormat.Png; break;//png
                    case 2: iff = ImageFormat.Jpeg; break;//jpg
                    case 3: iff = ImageFormat.Bmp; break;//bmp
                    case 4: iff = ImageFormat.Emf; break;//emf
                    case 5: iff = ImageFormat.Exif; break;//exit
                    case 6: iff = ImageFormat.Gif; break;//gif
                    case 7: iff = ImageFormat.Icon; break;//icon
                    case 8: iff = ImageFormat.Tiff; break;//tiff
                    case 9: iff = ImageFormat.Wmf; break;//wfm
                    default:
                        break;
                }
            }
            else
            {
                fname = "[currentname].png";
                return null;
            }
            return iff;
        }


        private void SaveImageAs(string ext, string fname, string cDir, Bitmap btmap)
        {
            saveFileDialog1.InitialDirectory = cDir;
            saveFileDialog1.FileName = fname;
            if (ext == ".mdl")
                saveFileDialog1.FileName = saveFileDialog1.FileName.Replace(ext, ".png");
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.Filter = "PNG Image (*.png)|*.png"
                                     + "|JPEG Image (*.jpg)|*.jpg"
                                     + "|Bitmap Image (*.bmp)|*.bmp"
                                     + "|Enhanced Metafile Image (*.emf)|*.emf"
                                     + "|Exchangable Image File (*.exif)|*.exif"
                                     + "|Graphics Interchange File (*.gif)|*.gif"
                                     + "|Windows Icon Image (*.ico)|*.ico"
                                     + "|Tagged Image File Format (*.tiff)|*.tiff"
                                     + "|Windows Metafile Image Format (*.wmf)|*.wmf"
                                     ;//|Allegiance binary bmp.mdl Image (*.mdl)|*.mdl|Allegiance text bmp.mdl Image (*.mdl)|*.mdl";
            saveFileDialog1.AddExtension = true;
            if (ext == ".png")
                saveFileDialog1.FilterIndex = 1;
            if (ext == ".jpg")
                saveFileDialog1.FilterIndex = 2;
            if (ext == ".bmp")
                saveFileDialog1.FilterIndex = 3;
            if (ext == ".emf")
                saveFileDialog1.FilterIndex = 4;
            if (ext == ".exif")
                saveFileDialog1.FilterIndex = 5;
            if (ext == ".gif")
                saveFileDialog1.FilterIndex = 6;
            if (ext == ".ico")
                saveFileDialog1.FilterIndex = 7;
            if (ext == ".tiff")
                saveFileDialog1.FilterIndex = 8;
            if (ext == ".wmf")
                saveFileDialog1.FilterIndex = 9;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImageFormat iff = ImageFormat.Png;
                switch (saveFileDialog1.FilterIndex)
                {
                    case 1: iff = ImageFormat.Png; break;//png
                    case 2: iff = ImageFormat.Jpeg; break;//jpg
                    case 3: iff = ImageFormat.Bmp; break;//bmp
                    case 4: iff = ImageFormat.Emf; break;//emf
                    case 5: iff = ImageFormat.Exif; break;//exit
                    case 6: iff = ImageFormat.Gif; break;//gif
                    case 7: iff =ImageFormat.Icon; break;//icon
                    case 8: iff =ImageFormat.Tiff; break;//tiff
                    case 9: iff = ImageFormat.Wmf; break;//wfm
                    default:
                        break;
                }

                btmap.Save(saveFileDialog1.FileName, iff);
                SetDirectory(currentDirectory.FullName);
            }
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            Button sndr = sender as Button;
            if (sndr != null)
            {
                if (optionsButtonFocus == false)
                {
                    optionsButtonFocus = true;
                    Point p = new Point(15, 15);
                    contextMenuStripOptions.Show(sndr, p, ToolStripDropDownDirection.BelowRight);
                }
                else
                {
                    optionsButtonFocus = false;
                }
            }
        }

        private void rememberZoomLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rememberZoomLevelToolStripMenuItem.Checked = !rememberZoomLevelToolStripMenuItem.Checked;
            Properties.Settings.Default.RememberZoom = rememberZoomLevelToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
        }

        private void useQuickZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            useQuickZoomToolStripMenuItem.Checked = !useQuickZoomToolStripMenuItem.Checked;
            Properties.Settings.Default.UseQuickZoom = useQuickZoomToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
        }

        private void btnOptions_Leave(object sender, EventArgs e)
        {
            optionsButtonFocus = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }





}
