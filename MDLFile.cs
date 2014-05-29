/*
	MDLFile.cs - Allegiance MDL files API 
	Copyright (C) Kirth Gersen, 2001-2003.  All rights reserved.
	v 0.95 
	SaveTofile limitation:
		- LOD not supported
		- single group mdl only
		- no 'real' error handling (BinaryWriter exceptions arent handled)
*/

using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

namespace MDL
{
    public struct SymbolPair
    {
        public string Name;
        public int Value;
    }

    public struct MDLLight //size = 12 float
    {
        public float red;
        public float green;
        public float blue;
        public float speed; // or time factor
        public float posx;
        public float posy;
        public float posz;
        public float todo1; // 1.25 (0 = crash !)
        public float todo2; // 0
        public float todo3; // 0.1
        public float todo4; // 0
        public float todo5; // 0.05
        public bool Read(BinaryReader br)
        {
            red = br.ReadSingle();
            green = br.ReadSingle();
            blue = br.ReadSingle();
            speed = br.ReadSingle();
            posx = br.ReadSingle();
            posy = br.ReadSingle();
            posz = br.ReadSingle();
            todo1 = br.ReadSingle();
            todo2 = br.ReadSingle();
            todo3 = br.ReadSingle();
            todo4 = br.ReadSingle();
            todo5 = br.ReadSingle();
            return true;
        }
        public void Write(BinaryWriter bw)
        {
            bw.Write(red);
            bw.Write(green);
            bw.Write(blue);
            bw.Write(speed);
            bw.Write(posx);
            bw.Write(posy);
            bw.Write(posz);
            bw.Write(todo1);
            bw.Write(todo2);
            bw.Write(todo3);
            bw.Write(todo4);
            bw.Write(todo5);
        }
    }
    public struct MDLFrameData // size = name + 9 float
    {
        public string name;
        public float posx;
        public float posy;
        public float posz;
        public float nx;
        public float ny;
        public float nz;
        public float px;
        public float py;
        public float pz;
        public bool Read(BinaryReader br)
        {
            posx = br.ReadSingle();
            posy = br.ReadSingle();
            posz = br.ReadSingle();
            nx = br.ReadSingle();
            ny = br.ReadSingle();
            nz = br.ReadSingle();
            px = br.ReadSingle();
            py = br.ReadSingle();
            pz = br.ReadSingle();
            return true;
        }
        public void Write(BinaryWriter bw)
        {
            bw.Write(posx);
            bw.Write(posy);
            bw.Write(posz);
            bw.Write(nx);
            bw.Write(ny);
            bw.Write(nz);
            bw.Write(px);
            bw.Write(py);
            bw.Write(pz);
        }
    }
    public struct MDLVertice
    {
        public float x;
        public float y;
        public float z;
        public float mx;
        public float my;
        public float nx;
        public float ny;
        public float nz;
    }
    public struct MDLMesh
    {
        public int nvertex;
        public int nfaces;
        public MDLVertice[] vertices;
        public ushort[] faces;
    }
    //#define MDLImageInitSize 20
    
    public struct MDLImage
    {
        public struct BinarySurfaceInfo
        {
            public Int32 width;
            public Int32 height;
            public Int32 pitch;
            public Int32 bitCount;
            public Int32 redMask;
            public Int32 greenMask;
            public Int32 blueMask;
            public Int32 alphaMask;
            public bool UseColorKey;
            public void ReadFromReader(System.IO.BinaryReader br)
            {
                width = br.ReadInt32();
                height = br.ReadInt32();
                pitch = br.ReadInt32();
                bitCount = br.ReadInt32();
                redMask = br.ReadInt32();
                greenMask = br.ReadInt32();
                blueMask = br.ReadInt32();
                alphaMask = br.ReadInt32();
                UseColorKey = Convert.ToBoolean(br.ReadInt32());
            }
            public void WriteToWriter(BinaryWriter bwr)
            {
                bwr.Write(width);
                bwr.Write(height);
                bwr.Write(pitch);
                bwr.Write(bitCount);
                bwr.Write(redMask);
                bwr.Write(greenMask);
                bwr.Write(blueMask);
                bwr.Write(alphaMask);
                bwr.Write(Convert.ToInt32(UseColorKey));
            }
        }
        public BinarySurfaceInfo ImageHeader;
        public byte[] PixelData;
        public Bitmap myBitmap;
        public bool Read(BinaryReader br)
        {
            ImageHeader.ReadFromReader(br);
            int numBytes = ImageHeader.pitch * ImageHeader.height * (ImageHeader.bitCount / 16);
            PixelData = br.ReadBytes(numBytes);
            reloadBmpFromPixelData();
            return true;
        }
        public void Write(BinaryWriter bwr)
        {
            ImageHeader.WriteToWriter(bwr);
            int numBytes = ImageHeader.pitch * ImageHeader.height * (ImageHeader.bitCount / 16);
            bwr.Write(PixelData, 0, numBytes);
        }
        private void reloadBmpFromPixelData()
        {
            int bpp = ImageHeader.bitCount;
            int width = ImageHeader.width;
            int height = ImageHeader.height;
            myBitmap = null;
            if (bpp == 16)
            {
                myBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            }
            else if (bpp == 24)
            {
                myBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            }

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, myBitmap.Width, myBitmap.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                myBitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                myBitmap.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;


            // Declare an array to hold the bytes of the bitmap.            
            int numBytes = bmpData.Stride * bmpData.Height;
            byte[] rgbValues = PixelData;//new byte[bytes];
            bool succeded = false;
            try
            {
                // Copy the RGB values back to the bitmap
                System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, numBytes);
                succeded = true;
            }
            catch (Exception ex)
            {

            }
            if (succeded == false)
            {
                // attempt to load with accuall scanwidth in header..
                numBytes = ImageHeader.pitch * ImageHeader.height * (ImageHeader.bitCount / 16);
                System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, numBytes);
            }


            // Unlock the bits.
            myBitmap.UnlockBits(bmpData);
        }
    }

    public class MDLObject
    {
        public MDLType type;
        public int nchildren;
        public MDLObject[] childrens;
        public MDLMesh mesh;
        public float lodval;
        public int textidx;
        public MDLImage image;
    }

    public enum MDLType { mdl_empty, mdl_mesh, mdl_group, mdl_lod, mdl_image };
    /// <summary>
    /// MDLFile : object reprensenting a MDL file in memory
    /// </summary>
    public class MDLFile
    {
        public struct MDLHeader
        {
            public uint MDLMAGIC;
            public int MDLVersion;
            public int ImportedNameSpacesCount;
            public int ImportedSymoblCount;
            public int ExportedSymbolCount;
            public int OtherObjectsCount;
            public List<string> NameSpaces ;
            public List<SymbolPair> ImportedSymbols ;
            public List<string> ExportedSymbols ;
            public bool ReadHeader(BinaryReader br)
            {
                MDLMAGIC = br.ReadUInt32();
                if (MDLMAGIC != 0xDEBADF00)
                    return false;
                MDLVersion = br.ReadInt32();
                ImportedNameSpacesCount = br.ReadInt32();
                ImportedSymoblCount = br.ReadInt32();
                ExportedSymbolCount = br.ReadInt32();
                OtherObjectsCount = br.ReadInt32();
                NameSpaces = new List<string>();
                ImportedSymbols = new List<SymbolPair>();
                ExportedSymbols = new List<string>();
                return true;
            }
            public void WriteHeader(BinaryWriter bw)
            {
                bw.Write(MDLVersion);
                bw.Write(ImportedNameSpacesCount);
                bw.Write(ImportedSymoblCount);
                bw.Write(ExportedSymbolCount);
                bw.Write(OtherObjectsCount);
            }
        }
        public MDLHeader header;
        public int NumLights;
        public List<MDLLight> Lights;
        public int NumFrameDatas;
        public List<MDLFrameData> FrameDatas;
        public List<MDLObject> RootObject;
        public string ReadError;
        public int NumTextures;
        public Dictionary<string, int> Textures;
        public float FrameVal;

        public MDLFile()
        {
            NumLights = 0;
            NumFrameDatas = 0;
            NumTextures = 0;
            RootObject = new List<MDLObject>();
            RootObject.Add(new MDLObject());

            RootObject[0].type = MDLType.mdl_empty;
        }
        /// <summary>
        /// Read a binary MDL file
        /// a real mess !
        /// return true on success
        /// </summary>
        /// <param name="sFileName"></param>
        /// <returns></returns>
        public bool ReadFromFile(string sFileName)
        {
            FileStream cf;
                try
                {
                    cf = new FileStream(sFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                }
                catch
                {
                    return false;
                }
            BinaryReader br = new BinaryReader(cf);
            
            

            header = new MDLHeader();
            if (!header.ReadHeader(br))
            {
                br.Close();
                cf.Close();
                return false;
            }

             // Read in namespaces 
            for (int i = 0; i < header.ImportedNameSpacesCount; i++)
            {
                string tag = ReadAlignedString(br);
                header.NameSpaces.Add( tag );
                //if ((tag != "model") && (tag != "effect"))
               //     NumTextures++;
            }
            // Count number of names spaecs we dont recognize and assume they are texture namespaces.
            NumTextures = header.NameSpaces.Count(x => x != "model" && x != "effect");

            Textures = header.NameSpaces.Where(x => x != "model" && x != "effect").ToDictionary(x=>x,x=>this.header.NameSpaces.IndexOf(x));

            // ASSERT(idx==NumTextures);
            for (int i = 0; i < header.ImportedSymoblCount; i++)
            {
                int uk1 = br.ReadInt32();
                string tag = ReadAlignedString(br);
                SymbolPair sp;
                sp.Name = tag;
                sp.Value = uk1;
                header.ImportedSymbols.Add(sp);
            }

            for (int i = 0; i < header.ExportedSymbolCount; i++)
            {
                string tag = ReadAlignedString(br);
                header.ExportedSymbols.Add(tag);
            }
            // LOOP LEVEL 3

            int lastText = -1;
            //MDLObject lastObject = new MDLObject();
            MDLObject[] stackedObjects = new MDLObject[500];
            int[] stack = new int[200];
            int sopos = -1;
            for (int L3 = 0; L3 < header.ExportedSymbolCount; L3++)
            {
                int l3val = br.ReadInt32();
                bool cont = true;
                int stackpos = -1;
                // LOOL LEVEL 2
                while (cont)
                {
                    int token = br.ReadInt32();
                    switch (token)
                    {
                        case 5:
                            {
                                // start of group
                                // push # val
                                int nentry = br.ReadInt32();
                                stack[++stackpos] = nentry;
                                break;
                            }
                        case 9:
                            {
                                int l2idx = br.ReadInt32();
                                if ((l2idx < 0) || (l2idx > header.ImportedSymoblCount))
                                {
                                    //ReadError.Format("unmatched l2idx = %s",l2idx);
                                    cont = false;
                                    break;
                                }
                                else
                                {
                                    string l2type = header.ImportedSymbols[l2idx].Name;
                                    bool matched = false;
                                    if (l2type == "MeshGeo")
                                    {
                                        matched = true;
                                        int datatype = br.ReadInt32();
                                        if (datatype != 7)
                                        {
                                            cont = false;
                                            break;
                                        }
                                        MDLMesh mesh = new MDLMesh();
                                        mesh.nvertex = br.ReadInt32();
                                        mesh.nfaces = br.ReadInt32();
                                        mesh.vertices = new MDLVertice[mesh.nvertex];
                                        for (int n = 0; n < mesh.nvertex; n++)
                                        {
                                            // read vertice
                                            MDLVertice vert = new MDLVertice();
                                            vert.x = br.ReadSingle();
                                            vert.y = br.ReadSingle(); ;
                                            vert.z = br.ReadSingle(); ;
                                            vert.mx = br.ReadSingle(); ;
                                            vert.my = br.ReadSingle(); ;
                                            vert.nx = br.ReadSingle(); ;
                                            vert.ny = br.ReadSingle(); ;
                                            vert.nz = br.ReadSingle(); ;
                                            mesh.vertices[n] = vert;
                                        }
                                        mesh.faces = new ushort[mesh.nfaces];
                                        for (int n = 0; n < mesh.nfaces; n++)
                                            mesh.faces[n] = br.ReadUInt16();
                                        stackedObjects[++sopos] = NewMDLObject();
                                        stackedObjects[sopos].mesh = mesh;
                                        stackedObjects[sopos].type = MDLType.mdl_mesh;
                                    }
                                    if (l2type == "ModifiableNumber")
                                    {
                                        int six = br.ReadInt32();
                                        matched = true;
                                    }
                                    if (l2type == "LightsGeo")
                                    {
                                        matched = true;
                                        int datatype = br.ReadInt32();
                                        if (datatype != 7)
                                        {
                                            // ReadError.Format("bad data %d in LightsGeo",datatype);
                                            cont = false;
                                            break;
                                        }
                                        if (NumLights != 0)
                                        {
                                            // ReadError.Format("double ligths!!!");
                                            cont = false;
                                            break;
                                        }
                                        int nlite = br.ReadInt32();
                                        NumLights = nlite;
                                        Lights = new List<MDLLight>();
                                        for (int n = 0; n < nlite; n++)
                                        {
                                            MDLLight lite = new MDLLight();
                                            lite.Read(br);
                                            Lights.Add(lite);
                                        }
                                    }
                                    if (l2type == "FrameData")
                                    {
                                        matched = true;
                                        int datatype = br.ReadInt32();
                                        if (datatype != 7)
                                        {
                                            //ReadError.Format("bad data %d in FrameData",datatype);
                                            cont = false;
                                            break;
                                        }
                                        if (NumFrameDatas != 0)
                                        {
                                            // ReadError.Format("double framedata!!!");
                                            cont = false;
                                            break;
                                        }
                                        int ndata = br.ReadInt32();
                                        NumFrameDatas = ndata;
                                        FrameDatas = new List<MDLFrameData>();
                                        for (int n = 0; n < ndata; n++)
                                        {
                                            MDLFrameData data = new MDLFrameData();
                                            data.name = ReadAlignedString(br);
                                            data.Read(br);
                                            FrameDatas.Add( data);
                                        }
                                    }
                                    if (l2type == "TextureGeo")
                                    {
                                        matched = true;
                                        int six = br.ReadInt32();
                                        // ASSERT(lastObject != NULL);
                                        stackedObjects[sopos].textidx = lastText;
                                    }
                                    if (l2type == "LODGeo")
                                    {
                                        matched = true;
                                        int six = br.ReadInt32();
                                        MDLObject lastObject = NewMDLObject();
                                        lastObject.type = MDLType.mdl_lod;
                                        lastObject.nchildren = stack[stackpos] + 1;
                                        lastObject.childrens = new MDLObject[lastObject.nchildren];
                                        for (int n = 0; n < lastObject.nchildren; n++)
                                        {
                                            lastObject.childrens[n] = stackedObjects[sopos--];
                                        }
                                        stackedObjects[++sopos] = lastObject;
                                        stackpos--;
                                    }
                                    if (l2type == "GroupGeo")
                                    {
                                        matched = true;
                                        int six = br.ReadInt32();
                                        MDLObject lastObject = NewMDLObject();
                                        lastObject.type = MDLType.mdl_group;
                                        lastObject.nchildren = stack[stackpos];
                                        lastObject.childrens = new MDLObject[lastObject.nchildren];
                                        for (int n = 0; n < lastObject.nchildren; n++)
                                        {
                                            lastObject.childrens[n] = stackedObjects[sopos--];
                                        }
                                        stackedObjects[++sopos] = lastObject;
                                        stackpos--;
                                    }
                                    if (l2type == "time")
                                    {
                                        matched = true;
                                        //ReadError.Format("!!time!!"),
                                        cont = false;
                                        break;
                                    }
                                    if (l2type == "ImportImage")
                                    {
                                        matched = true;
                                        cont = false;
                                        int datatype = br.ReadInt32();
                                        if (datatype != 7)
                                        {
                                            // ReadError.Format("bad data %d in ImportImage",datatype);
                                            cont = false;
                                            break;
                                        }
                                        MDLImage img = new MDLImage();
                                        img.Read(br);
                                        stackedObjects[++sopos] = NewMDLObject();
                                        stackedObjects[sopos].image = img;
                                        stackedObjects[sopos].type = MDLType.mdl_image;
                                        break;
                                    }
                                    if (!matched)
                                    {
                                        for (int n = 0; n < header.ImportedNameSpacesCount; n++)
                                            if (l2type == header.NameSpaces[n])
                                            {
                                                matched = true;
                                                lastText = -1;
                                                for (int p = 0; p < NumTextures; p++)
                                                {                                                    
                                                    if (Textures.ElementAt(p).Value == header.ImportedSymbols[l2idx].Value)
                                                        lastText = p;
                                                }
                                                // ASSERT(lastText != -1);
                                            }
                                    }
                                    if (!matched)
                                    {
                                        //ReadError.Format("unmatched l2type = %s\n",l2type);
                                        cont = false;
                                        break;
                                    }
                                }
                                break;
                            }
                        case 1:
                            {
                                float val = br.ReadSingle();
                                if (header.ExportedSymbols[L3] == "frame")
                                {
                                    FrameVal = val;
                                }
                                else
                                {
                                    // ASSERT(lastObject != NULL);
                                    stackedObjects[sopos].lodval = val;
                                }
                                break;
                            }
                        case 10:
                            {
                                // handle 10
                                break;
                            }
                        case 0:
                            {
                                if (stackpos >= 0)
                                {//stack[stackpos] -=1;
                                }
                                else
                                    cont = false;
                                break;
                            }
                        default:
                            //ReadError.Format("unknow token = %d\n",token);
                            cont = false;
                            break;
                    } // switch
                } // while(cont)
            } // l3
            RootObject = new List<MDLObject>(stackedObjects.Where(x=>x != null));//[sopos];
            br.Close();
            cf.Close();
            return true;
        }
        /// <summary>
        /// Parse a mdl string (could be optimized)
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        string ReadAlignedString(BinaryReader br)
        {
            string res = "";
            char[] data = new char[5];
            data[4] = '\0';
            do
            {
                data = br.ReadChars(4);
                for (int i = 0; i < 4 && data[i] != '\0'; i++)
                {
                    res += data[i];
                }
            } while (data[3] != '\0');
            return res;
        }
        /// <summary>
        /// Construct a new MDLObject
        /// </summary>
        /// <returns></returns>
        public static MDLObject NewMDLObject()
        {
            MDLObject o = new MDLObject();
            o.nchildren = 0;
            o.childrens = null;
            o.lodval = 0;
            o.type = MDLType.mdl_empty;
            o.textidx = -1;
            return o;
        }


        //// save to .mdl binary format
        //public bool SaveToFile(string sFileName)
        //{
        //    // Compute header but assume FrameVal is valid (no check)

        //    if (RootObject.type == MDLType.mdl_empty)
        //        return false;
        //    // open file
        //    FileStream cf;
        //    try
        //    {
        //        cf = new FileStream(sFileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //    BinaryWriter bw = new BinaryWriter(cf);

        //    // compute header
        //    bool bIsNotImage = (RootObject.type != MDLType.mdl_image);
        //    MDLHeader hdr = new MDLHeader();
        //    hdr.MDLVersion = 0;
        //    hdr.ImportedNameSpacesCount = 1; // model + [effect = lights or datas] + [textures]
        //    int TexturePos = 1;
        //    if ((NumLights > 0) || (NumFrameDatas > 0))
        //    {
        //        TexturePos = 2;
        //        hdr.ImportedNameSpacesCount++;
        //    }
        //    for (int t = 0; t < NumTextures; t++)
        //        hdr.ImportedNameSpacesCount++;
        //    hdr.NameSpaces = new List<string>();
        //    int tnpos = 0;
        //    hdr.NameSpaces[tnpos++] = "model";
        //    if ((NumLights > 0) || (NumFrameDatas > 0))
        //        hdr.NameSpaces[tnpos++] = "effect";
        //    for (int t = 0; t < NumTextures; t++)
        //        hdr.NameSpaces[tnpos++] = Textures.ElementAt(t).Key;
        //    // ------------------------------- L2 -----------------------------------
        //    hdr.ImportedSymoblCount = 0;
        //    hdr.ImportedSymbols = new List<SymbolPair>(); // not clean, should compute hdr.l2 then alloc (2 passes)
        //    if (bIsNotImage)
        //    {
        //        hdr.ImportedSymoblCount = 1; // ModifiableNumber,LightsGeo, time, FrameData then Texture & Group
        //        SymbolPair sp = new SymbolPair();
        //        sp.Name = "ModifiableNumber";
        //        sp.Value = 0;
        //        hdr.ImportedSymbols.Add(sp);
        //    }
        //    int LightsPos = -1;
        //    if (NumLights > 0)
        //    {
        //        SymbolPair sp = new SymbolPair();
        //        sp.Name = "LightsGeo";
        //        sp.Value = 1;
        //        hdr.ImportedSymbols.Add(sp);
        //        LightsPos = hdr.ImportedSymbols.IndexOf(sp);

        //        sp = new SymbolPair();
        //        sp.Name = "time";
        //        sp.Value = 0;
        //        hdr.ImportedSymbols.Add(sp);
        //    }
        //    int FramesPos = -1;
        //    if (NumFrameDatas > 0)
        //    {
        //        SymbolPair sp = new SymbolPair();
        //        sp.Name = "FrameData";
        //        sp.Value = 1;
        //        hdr.ImportedSymbols.Add(sp);
        //        FramesPos = hdr.ImportedSymbols.IndexOf(sp);
        //    }
        //    int GroupPos = -1;
        //    if (RootObject.type == MDLType.mdl_group)
        //    {
        //        SymbolPair sp = new SymbolPair();
        //        sp.Name = "GroupGeo";
        //        sp.Value = 0;
        //        hdr.ImportedSymbols.Add(sp);
        //        GroupPos = hdr.ImportedSymbols.IndexOf(sp);
        //    }
        //    int MeshPos = -1;
        //    if (bIsNotImage)
        //    {
        //        SymbolPair sp = new SymbolPair();
        //        sp.Name = "MeshGeo";
        //        sp.Value = 0;
        //        hdr.ImportedSymbols.Add(sp);
        //        MeshPos = hdr.ImportedSymbols.IndexOf(sp);
        //    }
        //    int TextPos = -1;
        //    int TextGeoPos = -1;
        //    for (int t = 0; t < NumTextures; t++)
        //    {
        //        if (t == 0)
        //        {

        //            SymbolPair sp = new SymbolPair();
        //            sp.Name = "TextureGeo";
        //            sp.Value = 0;
        //            hdr.ImportedSymbols.Add(sp);
        //            TextGeoPos = hdr.ImportedSymbols.IndexOf(sp);

        //            TextPos = TextGeoPos + 1;
        //        }
        //        SymbolPair sp2 = new SymbolPair();
        //        sp2.Name = hdr.NameSpaces[TexturePos + t];
        //        sp2.Value = TexturePos + t;
        //        hdr.ImportedSymbols.Add(sp2);
        //    }
        //    int ImagePos = -1;
        //    if (!bIsNotImage)
        //    {
        //        SymbolPair sp = new SymbolPair();
        //        sp.Name = "ImportImage";
        //        sp.Value = 0;
        //        hdr.ImportedSymbols.Add(sp);
        //        ImagePos = hdr.ImportedSymbols.IndexOf(sp);
        //    }
        //    // ------------------------------- L3 -----------------------------------
        //    hdr.ExportedSymbols = new List<string>();
        //    if (bIsNotImage)
        //    {
        //        hdr.ExportedSymbolCount = 2; // frame,[lights],[frames],object
        //        hdr.ExportedSymbols.Add("frame");
        //        if (NumLights > 0)
        //        {
        //            hdr.ExportedSymbols.Add("lights");
        //        }
        //        if (NumFrameDatas > 0)
        //        {
        //            hdr.ExportedSymbols.Add("frames");
        //        }
        //        hdr.ExportedSymbols.Add("object");
        //    }
        //    else // image
        //    {
        //      //  hdr.ExportedSymbols.Add(Textures[0]);
        //    }
        //    // ------------------------------- L4 -----------------------------------
        //    hdr.OtherObjectsCount = 0;
        //    // ------------------------------- end of header-------------------------

        //    // write cookie & fixed size header
        //    uint cookie = 0xDEBADF00;
        //    bw.Write(cookie);
        //    hdr.WriteHeader(bw);

        //    // write TagsNames
        //    for (int t = 0; t < hdr.ImportedNameSpacesCount; t++)
        //    {
        //        SaveString(bw, hdr.NameSpaces[t]);
        //    }
        //    // Write L2 entries
        //    for (int t = 0; t < hdr.ImportedSymoblCount; t++)
        //    {
        //        bw.Write(hdr.ImportedSymbols[t].Value);
        //        SaveString(bw, hdr.ImportedSymbols[t].Name);
        //    }

        //    // Write L3 levels name
        //    for (int t = 0; t < hdr.ExportedSymbolCount; t++)
        //    {
        //        SaveString(bw, hdr.ExportedSymbols[t]);
        //    }

        //    // write body
        //    // Main L3 save loop
        //    for (int t = 0; t < hdr.ExportedSymbolCount; t++)
        //    {
        //        bw.Write(t);
        //        if (hdr.ExportedSymbols[t] == "frame")
        //        {
        //            bw.Write((int)1);
        //            bw.Write(FrameVal);
        //            bw.Write((int)9);
        //            bw.Write((int)0);
        //            bw.Write((int)6);
        //        }
        //        if (hdr.ExportedSymbols[t] == "lights")
        //        {
        //            bw.Write((int)9);
        //            bw.Write(LightsPos);
        //            bw.Write((int)7);
        //            bw.Write(NumLights);
        //            for (int l = 0; l < NumLights; l++)
        //            {
        //                Lights[l].Write(bw);
        //            }
        //        }
        //        if (hdr.ExportedSymbols[t] == "frames")
        //        {
        //            bw.Write((int)9);
        //            bw.Write(FramesPos);
        //            bw.Write((int)7);
        //            bw.Write(NumFrameDatas);
        //            for (int l = 0; l < NumFrameDatas; l++)
        //            {
        //                SaveString(bw, FrameDatas[l].name);
        //                FrameDatas[l].Write(bw);
        //            }
        //        }
        //        if (hdr.ExportedSymbols[t] == "object")
        //        {
        //            SaveObject(RootObject, bw, GroupPos, MeshPos, TextPos, TextGeoPos);
        //        }
        //        if (!bIsNotImage)
        //        {
        //            bw.Write((int)9);
        //            bw.Write(ImagePos);
        //            bw.Write((int)7);
        //            // save the image 
        //            RootObject.image.Write(bw);
        //        }
        //        bw.Write((int)0);
        //    }
        //    bw.Close();
        //    cf.Close();
        //    return true;
        //}

        // SaveToFile sub part - save a child object
        void SaveObject(MDLObject po, BinaryWriter bw, int GroupPos, int MeshPos, int TextPos, int TextGeoPos)
        {
            if (po.type == MDLType.mdl_group)
            {
                bw.Write((int)5);
                bw.Write(po.nchildren);
                for (int n = 0; n < po.nchildren; n++)
                {
                    SaveObject(po.childrens[n], bw, GroupPos, MeshPos, TextPos, TextGeoPos);
                    bw.Write((int)0);
                }
                bw.Write((int)9);
                bw.Write(GroupPos);
                bw.Write((int)6);
            }
            if (po.type == MDLType.mdl_lod)
            {
                bw.Write((int)5);
                bw.Write(po.nchildren);
                for (int n = 0; n < po.nchildren; n++)
                {
                    SaveObject(po.childrens[n], bw, GroupPos, MeshPos, TextPos, TextGeoPos);
                    if (po.childrens[n].lodval != 0)
                    {
                        bw.Write((int)1);
                        bw.Write(po.childrens[n].lodval);
                        bw.Write((int)10);
                    }
                    bw.Write((int)0);
                }
                bw.Write((int)9);
                bw.Write(GroupPos);
                bw.Write((int)6);
            }
            if (po.type == MDLType.mdl_mesh)
            {
                if (po.textidx != -1)
                {
                    bw.Write((int)9);
                    bw.Write(TextPos + po.textidx);
                }
                // save mesh
                bw.Write((int)9);
                bw.Write(MeshPos);
                bw.Write((int)7);
                bw.Write((int)po.mesh.nvertex);
                bw.Write((int)po.mesh.nfaces);
                for (int v = 0; v < po.mesh.nvertex; v++)
                {
                    bw.Write(po.mesh.vertices[v].x);
                    bw.Write(po.mesh.vertices[v].y);
                    bw.Write(po.mesh.vertices[v].z);
                    bw.Write(po.mesh.vertices[v].mx);
                    bw.Write(po.mesh.vertices[v].my);
                    bw.Write(po.mesh.vertices[v].nx);
                    bw.Write(po.mesh.vertices[v].ny);
                    bw.Write(po.mesh.vertices[v].nz);
                }
                for (int v = 0; v < po.mesh.nfaces; v++)
                {
                    bw.Write(po.mesh.faces[v]);
                }
                if (po.textidx != -1)
                {
                    bw.Write((int)9);
                    bw.Write(TextGeoPos);
                    bw.Write((int)6);
                }
            }
        }
        // string write
        void SaveString(BinaryWriter bw, string s)
        {
            for (int i = 0; i < s.Length; i++)
                bw.Write(s[i]);
            // complete with trailing 0
            char buf = '\0';
            int l = s.Length;
            l = l / 4;
            l = l * 4;
            l = s.Length - l;
            l = 4 - l;
            for (int p = 0; p < l; p++)
                bw.Write(buf);
        }
    }

}
