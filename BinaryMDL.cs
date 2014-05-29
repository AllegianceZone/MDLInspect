using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MDL;

namespace MDLUtility2
{
    public class BinaryMDL
    {
        BinaryMDLHeader binaryHeader;
        public List<MDLLight> lights = new List<MDLLight>();
        public List<MDLFrameData> frames = new List<MDLFrameData>();
        public List<string> textures = new List<string>();
        public MDLObject mainObject;
        public float FrameVal;
        public BinaryMDLHeader BinaryHeader
        {
            get { return binaryHeader; }
            set { binaryHeader = value; }
        }
        List<String> importedLibriesList = new List<String>();

        public List<String> ImportedLibriesList
        {
            get { return importedLibriesList; }
            set { importedLibriesList = value; }
        }
        List<ImportedSymbol> importedSymbolList = new List<ImportedSymbol>();

        public List<ImportedSymbol> ImportedSymbolList
        {
            get { return importedSymbolList; }
            set { importedSymbolList = value; }
        }
        List<String> exportedSymbolsList = new List<String>();

        public List<String> ExportedSymbolsList
        {
            get { return exportedSymbolsList; }
            set { exportedSymbolsList = value; }
        }
        List<SymbolData> exportedSymbolsDataList = new List<SymbolData>();

        public List<SymbolData> ExportedSymbolsDataList
        {
            get { return exportedSymbolsDataList; }
            set { exportedSymbolsDataList = value; }
        }

        public BinaryMDL(MDL.MDLFile mdlf)
        {
            this.binaryHeader._1_MDLMagic = 0xDEBADF00;
            binaryHeader._2_MDLVersion = mdlf.header.MDLVersion;
            binaryHeader._3_ImportedNameSpacesCount = mdlf.header.ImportedNameSpacesCount;
            binaryHeader._4_ImportedSymbolCount = mdlf.header.ImportedSymoblCount;
            binaryHeader._5_ExportedSymbolCount = mdlf.header.ExportedSymbolCount;
            binaryHeader._6_OtherObjectCount = mdlf.header.OtherObjectsCount;
            if(mdlf.Lights!=null)
            lights = new List<MDLLight>(mdlf.Lights);
            if (mdlf.FrameDatas != null)
            frames = new List<MDLFrameData>(mdlf.FrameDatas);
            if (mdlf.Textures != null)
            textures = new List<string>(mdlf.Textures);
            mainObject = mdlf.RootObject;
            FrameVal = mdlf.FrameVal;
        }
        public BinaryMDL(System.IO.BinaryReader br)
        {
            binaryHeader.fillFromReader(br); // Load the Header Info.
            if (binaryHeader.isValid())
            {
                // read in the importedNamespaces
                for (int i = 0; i < binaryHeader._3_ImportedNameSpacesCount; i++)
                {
                    importedLibriesList.Add(AlignedString.ReadAlignedString(br));
                }

                // read in the importedSymbols
                for (int i = 0; i < binaryHeader._4_ImportedSymbolCount; i++)
                {
                    importedSymbolList.Add(new ImportedSymbol(br));
                }

                // read in the exportedSymbols
                for (int i = 0; i < binaryHeader._5_ExportedSymbolCount; i++)
                {
                    exportedSymbolsList.Add(AlignedString.ReadAlignedString(br));
                }

                // read in the exportedSymbols Data
                for (int i = 0; i < binaryHeader._5_ExportedSymbolCount; i++)
                {
                    exportedSymbolsDataList.Add(new SymbolData(br, importedSymbolList));
                }
            }
        }
    }
    public struct BinaryMDLHeader
    {
        public UInt32 _1_MDLMagic; // should be 3736788736
        public Int32 _2_MDLVersion;
        public Int32 _3_ImportedNameSpacesCount; // usually 1
        public Int32 _4_ImportedSymbolCount; // usually 1
        public Int32 _5_ExportedSymbolCount; // usually 1
        public Int32 _6_OtherObjectCount; // always should be 0
        public void fillFromReader(System.IO.BinaryReader br)
        {
            _1_MDLMagic = br.ReadUInt32();
            if (isValid())
            {
                _2_MDLVersion = br.ReadInt32();
                _3_ImportedNameSpacesCount = br.ReadInt32();
                _4_ImportedSymbolCount = br.ReadInt32();
                _5_ExportedSymbolCount = br.ReadInt32();
                _6_OtherObjectCount = br.ReadInt32();
            }
        }
        public bool isValid()
        {
            return _1_MDLMagic == 3736788736;
        }
    }
    public class ImportedSymbol
    {
        Int32 index;
        String value;

        public Int32 Index
        {
            get { return index; }
            set { index = value; }
        }

        public String Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
      

        public ImportedSymbol(System.IO.BinaryReader br)
        {
            // read in the index
            index = br.ReadInt32();
            // read in the alignedString
            value = AlignedString.ReadAlignedString(br);
        }
    }
    public class AlignedString
    {
        string value = "";

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public AlignedString(System.IO.BinaryReader br)
        {
            this.value = ReadAlignedString(br);
        }
        public static string ReadAlignedString(System.IO.BinaryReader br)
        {
            string value = "";
            // ignore any null padding in the front.
            while (br.PeekChar() == '\0')// loop over the embeded name.
                br.ReadChar(); // discard the null

            while (br.PeekChar() != '\0')// loop over the embeded name.
                value += br.ReadChar();

            br.ReadChar();//discard the null terminating the end of the string.

            // if we are not alligned, then drop some bytes.
            while (br.BaseStream.Position % 4 != 0)
                br.ReadChar(); // should be null padding for alignment

            return value;
        }
    }
    public class SymbolData
    {
        Int32 index, unk1, unk2, unk3;
        object data;
        SymbolDataTypes indexType;
        public Int32 Index
        {
            get { return index; }
            set { index = value; }
        }
        public SymbolDataTypes DataType
        {
            get { return indexType; }
        }

        public object Data
        {
            get { return data; }
            set { data = value; }
        }
        public BinarySurface DataAsBinarySurface
        {
            get { return data as BinarySurface; }
        }

        public SymbolData(System.IO.BinaryReader br, List<ImportedSymbol> symbolList)
        {
            do
            {
                index = br.ReadInt32();
            } while (index != 9);
            index = br.ReadInt32();
            string DataSubType = symbolList[index].Value;
            // figure out the procedure to do.
            Int32 DataTypeIndex = br.ReadInt32(); // This is usually 7 I think.. 
            // Which is type  (SymbolDataTypes)7 or ObjectBinary
            // so this should probabally be read before this method is entered.
            // And be used as a type selector.
            if (symbolList.Count > index)
                indexType = (SymbolDataTypes)DataTypeIndex;


            switch (indexType)
            {
                case SymbolDataTypes.ObjectFloat:
                    if (DataSubType == "frame")
                    {
                        float val = br.ReadSingle();
                    }                    
                    break;

                case SymbolDataTypes.ObjectString:
                    //   stack.Push(new StringValue(ReadString()));
                    break;

                case SymbolDataTypes.ObjectTrue:
                    //   stack.Push(new Boolean(true));
                    break;

                case SymbolDataTypes.ObjectFalse:
                    //   stack.Push(new Boolean(false));
                    break;

                case SymbolDataTypes.ObjectList:
                    Int32 listCount = br.ReadInt32();
                    break;

                case SymbolDataTypes.ObjectApply:
                    //  stack.Push(ReadApply(stack));
                    Int32 tmp = br.ReadInt32();
                    break;

                case SymbolDataTypes.ObjectBinary:
                    if (DataSubType == "ImportImage")// we need to switch over something here.
                    {
                        data = new BinarySurface(br);
                    }
                    else if (DataSubType == "MeshGeo")
                    {
                        LoadMDLMesh(br);
                    }
                    else if (DataSubType == "LightsGeo")
                    {
                    }
                    else if (DataSubType == "FrameData")
                    {
                    }
                    else if (DataSubType == "TextureGeo")
                    {
                    }
                    else if (DataSubType == "LODGeo")
                    {
                    }
                    else if (DataSubType == "GroupGeo")
                    {
                    }
                    else if (DataSubType == "time")
                    {
                    }
                    break;

                case SymbolDataTypes.ObjectReference:
                    //  stack.Push(m_pobjects[ReadDWORD()]);
                    break;

                case SymbolDataTypes.ObjectImport:
                    //   stack.Push(m_pobjectImports[ReadDWORD()]);
                    break;

                case SymbolDataTypes.ObjectPair:
                    //   stack.Push(ReadPair(stack));
                    break;

                case SymbolDataTypes.ObjectEnd:
                    break;
                default:
                    break;
            }

            // Read the object End.
            // advance position to the null int at the end of the data.
            int lookingForNull = -1;
            do
            {
                lookingForNull = br.ReadInt32();
            } while (lookingForNull != 0);

        }

        private static MDLObject LoadMDLMesh(System.IO.BinaryReader br)
        {
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
            MDLObject mdlo = MDLFile.NewMDLObject();
            mdlo.mesh = mesh;
            mdlo.type = MDLType.mdl_mesh;
            return mdlo;
        }
    }

    public enum BinaryDataTypes
    {
        ImportImage
    }

    public enum SymbolDataTypes
    {
        ObjectEnd = 0,
        ObjectFloat = 1,
        ObjectString = 2,
        ObjectTrue = 3,
        ObjectFalse = 4,
        ObjectList = 5,
        ObjectApply = 6,
        ObjectBinary = 7,
        ObjectReference = 8,
        ObjectImport = 9,
        ObjectPair = 10,
        ImportImage = 11,

    }
    public class BinarySurface
    {
        int unk1, unk2, unk3;
        BinarySurfaceInfo header;
        byte[] pixelData;
        Bitmap myBMP;

        public Bitmap bmp
        {
            get { return myBMP; }
            // set { myBMP = value; }
        }
        internal BinarySurfaceInfo Header
        {
            get { return header; }
            set { header = value; }
        }

        public byte[] PixelData
        {
            get { return pixelData; }
            set { pixelData = value; }
        }
        public BinarySurface(System.IO.BinaryReader br)
        {
            // unk2 = br.ReadInt32();
            //unk3 = br.ReadInt32();
            header.fillFromReader(br);
            int numBytes = header._3_pitch * header._2_y * (header._4_bitCount / 16);
            PixelData = new byte[numBytes];
            PixelData = br.ReadBytes(numBytes);

           

            reloadBmpFromPixelData();
        }
        public void reloadBmpFromPixelData()
        {
            int bpp = header._4_bitCount;
            int width = header._1_x;
            int height = header._2_y;
            myBMP = null;
            if (bpp == 16)
            {
                myBMP = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            }
            else if (bpp == 24)
            {
                myBMP = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            }

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, myBMP.Width, myBMP.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                myBMP.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                myBMP.PixelFormat);

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
                numBytes = header._3_pitch * header._2_y * (header._4_bitCount / 16);
                System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, numBytes);
            }


            // Unlock the bits.
            myBMP.UnlockBits(bmpData);
        }
    }
    public struct BinarySurfaceInfo
    {
        public Int32 _1_x;
        public Int32 _2_y;
        public Int32 _3_pitch;
        public Int32 _4_bitCount;
        public Int32 _5_redMask;
        public Int32 _6_greenMask;
        public Int32 _7_blueMask;
        public Int32 _8_alphaMask;
        public bool _9_bColorKey;
        public void fillFromReader(System.IO.BinaryReader br)
        {
            _1_x = br.ReadInt32();
            _2_y = br.ReadInt32();
            _3_pitch = br.ReadInt32();
            _4_bitCount = br.ReadInt32();
            _5_redMask = br.ReadInt32();
            _6_greenMask = br.ReadInt32();
            _7_blueMask = br.ReadInt32();
            _8_alphaMask = br.ReadInt32();
            _9_bColorKey = Convert.ToBoolean(br.ReadInt32());
        }
    }
}
