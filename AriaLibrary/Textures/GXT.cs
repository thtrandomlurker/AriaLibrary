using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AriaLibrary.Textures
{

    // Borrowed from Scarlet: https://github.com/xdanieldzd/Scarlet

    // Copyright(c) 2016 xdaniel(Daniel R.) / DigitalZero Domain

    // Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
    // associated documentation files (the "Software"), to deal in the Software without restriction, including
    // without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    // copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

    //The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
    // THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
    // OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
    // OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

    // https://github.com/vitasdk/vita-headers/blob/master/include/psp2/gxm.h

    public enum SceGxmTextureBaseFormat : uint
    {
        U8 = 0x00000000,
        S8 = 0x01000000,
        U4U4U4U4 = 0x02000000,
        U8U3U3U2 = 0x03000000,
        U1U5U5U5 = 0x04000000,
        U5U6U5 = 0x05000000,
        S5S5U6 = 0x06000000,
        U8U8 = 0x07000000,
        S8S8 = 0x08000000,
        U16 = 0x09000000,
        S16 = 0x0A000000,
        F16 = 0x0B000000,
        U8U8U8U8 = 0x0C000000,
        S8S8S8S8 = 0x0D000000,
        U2U10U10U10 = 0x0E000000,
        U16U16 = 0x0F000000,
        S16S16 = 0x10000000,
        F16F16 = 0x11000000,
        F32 = 0x12000000,
        F32M = 0x13000000,
        X8S8S8U8 = 0x14000000,
        X8U24 = 0x15000000,
        U32 = 0x17000000,
        S32 = 0x18000000,
        SE5M9M9M9 = 0x19000000,
        F11F11F10 = 0x1A000000,
        F16F16F16F16 = 0x1B000000,
        U16U16U16U16 = 0x1C000000,
        S16S16S16S16 = 0x1D000000,
        F32F32 = 0x1E000000,
        U32U32 = 0x1F000000,
        PVRT2BPP = 0x80000000,
        PVRT4BPP = 0x81000000,
        PVRTII2BPP = 0x82000000,
        PVRTII4BPP = 0x83000000,
        UBC1 = 0x85000000,
        UBC2 = 0x86000000,
        UBC3 = 0x87000000,
        YUV420P2 = 0x90000000,
        YUV420P3 = 0x91000000,
        YUV422 = 0x92000000,
        P4 = 0x94000000,
        P8 = 0x95000000,
        U8U8U8 = 0x98000000,
        S8S8S8 = 0x99000000,
        U2F10F10F10 = 0x9A000000
    };

    public enum SceGxmTextureType : uint
    {
        Swizzled = 0x00000000,
        Cube = 0x40000000,
        Linear = 0x60000000,
        Tiled = 0x80000000,
        LinearStrided = 0xC0000000
    };

    // From here unless otherwise stated is my own code

    public class GXTTexData
    {
        public int PaletteIndex;
        public int Flags;
        public SceGxmTextureType TextureType;
        public SceGxmTextureBaseFormat TextureFormat;
        public uint SwizzleMode;
        public short Width;
        public short Height;
        public int MipCount;
        public byte[] PixelData;
        public bool IsBlockCompressed()
        {
            switch (TextureFormat)
            {
                case SceGxmTextureBaseFormat.UBC1:
                case SceGxmTextureBaseFormat.UBC2:
                case SceGxmTextureBaseFormat.UBC3:
                case SceGxmTextureBaseFormat.PVRT2BPP:
                case SceGxmTextureBaseFormat.PVRT4BPP:
                case SceGxmTextureBaseFormat.PVRTII2BPP:
                case SceGxmTextureBaseFormat.PVRTII4BPP:
                    return true;
                default:
                    return false;
            }
        }

        public void Read(BinaryReader reader)
        {
            int textureDataOffset = reader.ReadInt32();
            int textureDataSize = reader.ReadInt32();
            PaletteIndex = reader.ReadInt32();
            if (PaletteIndex != -1)
                throw new NotImplementedException("Texture is Paletted. FIX THIS PLEASE!");
            Flags = reader.ReadInt32();
            TextureType = (SceGxmTextureType)reader.ReadUInt32();
            if (!(TextureType == SceGxmTextureType.Swizzled || TextureType == SceGxmTextureType.Cube))
                throw new NotImplementedException($"I didn't think {TextureType} would exist in ia/vt.");
            uint tFmt = reader.ReadUInt32();
            TextureFormat = (SceGxmTextureBaseFormat)(tFmt & 0xFFFF0000);
            SwizzleMode = tFmt & 0x0000FFFF;
            Console.WriteLine(TextureFormat);
            Width = reader.ReadInt16();
            Height = reader.ReadInt16();
            MipCount = reader.ReadInt32();
            reader.BaseStream.Seek(textureDataOffset, SeekOrigin.Begin);
            PixelData = reader.ReadBytes(textureDataSize);
        }
    }

    public class GXT
    {
        public uint VersionWord;
        public string Version => $"{VersionWord >> 24}.{(VersionWord << 8) >> 24}.{(VersionWord << 16) >> 24}.{(VersionWord << 24) >> 24}";

        public GXTTexData TextureData; // only one per tex in ia/vt

        public void Read(BinaryReader reader)
        {
            char[] magic = reader.ReadChars(4);
            if (new string(magic) != "GXT\0")
            {
                throw new InvalidDataException("File magic is not GXT\\0");
            }
            VersionWord = reader.ReadUInt32();
            int imageCount = reader.ReadInt32();
            if (imageCount > 1)
                throw new NotImplementedException("More than one texture.");
            int imageDataOffset = reader.ReadInt32();
            int imageDataSizeTotal = reader.ReadInt32();
            int numP4Palettes = reader.ReadInt32();
            int numP8Palettes = reader.ReadInt32();
            int paddingThatIsUsuallyOneInIAVT = reader.ReadInt32();
            // we can cheat here since IA/VT textures are always single images
            // it also seems to always use BC1/2/3
            TextureData.Read(reader);
        }

        public void Load(string filePath)
        {
            using (Stream fileStream = File.OpenRead(filePath))
            {
                using (BinaryReader reader = new BinaryReader(fileStream))
                {
                    Read(reader);
                }
            }
        }
        public void Load(Stream fileStream)
        {
            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                Read(reader);
            }
        }

        public static int GetBlockSize(SceGxmTextureBaseFormat fmt)
        {
            switch (fmt)
            {
                case SceGxmTextureBaseFormat.UBC1:
                    return 8;
                case SceGxmTextureBaseFormat.UBC2:
                    return 16;
                case SceGxmTextureBaseFormat.UBC3:
                    return 16;
                case SceGxmTextureBaseFormat.U8U8U8:
                    return 3;
                case SceGxmTextureBaseFormat.U8U8U8U8:
                    return 4;
                default:
                    throw new NotImplementedException("Unhandled format");
            }
        }

        /// <summary>
        /// Loads a GXT from the given path and saves it as a DDS to the given output path
        /// </summary>
        /// <param name="gxtPath">Path to an input GXT file</param>
        /// <param name="ddsPath">Path to save the DDS to</param>
        public static void MakeDDSFromGXT(string gxtPath, string ddsPath)
        {
            GXT gxt = new GXT();
            gxt.Load(gxtPath);
            using (Stream outFile = File.Create(ddsPath))
            {
                using (BinaryWriter writer = new BinaryWriter(outFile))
                {
                    // prepare the DDS header
                    int blockSize = GetBlockSize(gxt.TextureData.TextureFormat);
                    int blockWidth = GetBlockWidth(gxt.TextureData.TextureFormat);

                    int pWidth = gxt.TextureData.Width / blockWidth;
                    int pHeight = gxt.TextureData.Height / blockWidth;

                    // magic
                    writer.Write(0x20534444);
                    // header size
                    writer.Write(0x0000007C);
                    // flags
                    writer.Write(0x000A1007);
                    // dimensions
                    writer.Write((uint)gxt.TextureData.Height);
                    writer.Write((uint)gxt.TextureData.Width);
                    // size of prim mip
                    writer.Write((pWidth * pHeight * blockSize) * (gxt.TextureData.TextureType == SceGxmTextureType.Cube ? 6 : 1));
                    writer.Write(1);
                    writer.Write(gxt.TextureData.MipCount);
                    // reserved iirc
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    writer.Write(0);
                    // pixel format info
                    writer.Write(0x20);
                    // for now only handle dxt
                    switch (gxt.TextureData.TextureFormat)
                    {
                        case SceGxmTextureBaseFormat.UBC1:
                            writer.Write(4);
                            writer.Write(0x31545844);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(gxt.TextureData.TextureType == SceGxmTextureType.Cube ? 0x00001008 : 0x00401008);
                            writer.Write(gxt.TextureData.TextureType == SceGxmTextureType.Cube ? 0x0000FE00 : 0x00000000);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            break;
                        case SceGxmTextureBaseFormat.UBC2:
                            writer.Write(4);
                            writer.Write(0x33545844);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(gxt.TextureData.TextureType == SceGxmTextureType.Cube ? 0x00001008 : 0x00401008);
                            writer.Write(gxt.TextureData.TextureType == SceGxmTextureType.Cube ? 0x0000FE00 : 0x00000000);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            break;
                        case SceGxmTextureBaseFormat.UBC3:
                            writer.Write(4);
                            writer.Write(0x35545844);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(gxt.TextureData.TextureType == SceGxmTextureType.Cube ? 0x00001008 : 0x00401008);
                            writer.Write(gxt.TextureData.TextureType == SceGxmTextureType.Cube ? 0x0000FE00 : 0x00000000);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            break;
                        case SceGxmTextureBaseFormat.U8U8U8:
                            writer.Write(0x41);
                            writer.Write(0);
                            writer.Write(32);
                            writer.Write(0xFF);
                            writer.Write(0xFF00);
                            writer.Write(0xFF0000);
                            writer.Write(0xFF000000);
                            writer.Write(gxt.TextureData.TextureType == SceGxmTextureType.Cube ? 0x00001008 : 0x00401008);
                            writer.Write(gxt.TextureData.TextureType == SceGxmTextureType.Cube ? 0x0000FE00 : 0x00000000);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            break;
                        case SceGxmTextureBaseFormat.U8U8U8U8:
                            writer.Write(0x41);
                            writer.Write(0);
                            writer.Write(32);
                            writer.Write(0xFF);
                            writer.Write(0xFF00);
                            writer.Write(0xFF0000);
                            writer.Write(0xFF000000);
                            writer.Write(gxt.TextureData.TextureType == SceGxmTextureType.Cube ? 0x00001008 : 0x00401008);
                            writer.Write(gxt.TextureData.TextureType == SceGxmTextureType.Cube ? 0x0000FE00 : 0x00000000);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            break;

                        default:
                            throw new NotImplementedException("Unsupported.");
                    }

                    byte[] outPixelArray = new byte[gxt.TextureData.PixelData.Length];

                    // P(ixel)P(processing)C(ode)
                    // Depth for cube maps
                    for (int d = 0; d < (gxt.TextureData.TextureType == SceGxmTextureType.Cube ? 6 : 1); d++)
                    {

                        for (int m = 0; m < gxt.TextureData.MipCount; m++)
                        {

                            int mipWidth = pWidth / ((int)Math.Pow(2, m));
                            int mipHeight = pHeight / ((int)Math.Pow(2, m));

                            // borrowed and significantly modified from https://github.com/xdanieldzd/Scarlet

                            // Copyright(c) 2016 xdaniel(Daniel R.) / DigitalZero Domain

                            // Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
                            // associated documentation files (the "Software"), to deal in the Software without restriction, including
                            // without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
                            // copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

                            //The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

                            // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
                            // THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
                            // OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
                            // OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
                            for (int i = 0; i < pWidth * pHeight; i++)
                            {
                                int dMin = pWidth < pHeight ? pWidth : pHeight;
                                int k = (int)Math.Log(dMin, 2);
                                int x = 0;
                                int y = 0;
                                if (pWidth < pHeight)
                                {
                                    int j = ((i >> (2 * k) << (2 * k)) | (DecodeMorton2X(i) & (dMin - 1)) << k | (DecodeMorton2Y(i) & (dMin - 1)) << 0);
                                    x = j % pWidth;
                                    y = j / pWidth;
                                }
                                else
                                {

                                    int j = ((i >> (2 * k) << (2 * k)) | (DecodeMorton2Y(i) & (dMin - 1)) << k | (DecodeMorton2X(i) & (dMin - 1)) << 0);
                                    x = j / pHeight;
                                    y = j % pHeight;
                                }

                                if (y >= pHeight || x >= pWidth)
                                    continue;

                                for (int p = 0; p < blockSize; p++)
                                {
                                    outPixelArray[((d * (mipWidth * mipHeight * blockSize))) + ((((y * mipWidth) + x) * blockSize) + p)] = gxt.TextureData.PixelData[(d * (mipWidth * mipHeight * blockSize)) + (i * blockSize) + p];
                                }
                            }
                        }
                    }

                    if (gxt.TextureData.IsBlockCompressed())
                        writer.Write(outPixelArray);
                    else
                    {
                        if (blockSize < 4)
                        {
                            for (int i = 0; i < outPixelArray.Length; i += blockSize)
                            {
                                writer.Write(outPixelArray.Skip(i).Take(blockSize).ToArray());
                                for (int v = 0; v < (4 - blockSize) - 1; v++)
                                {
                                    writer.Write((byte)0x00);
                                }
                                writer.Write((byte)0xFF);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < outPixelArray.Length; i += 4)
                            {
                                writer.Write(outPixelArray.Skip(i).Take(4).ToArray());
                            }
                        }
                    }
                }
            }
        }

        public static void MakeGXTFromDDS(string ddsPath, string gxtPath)
        {
            // Since supported DDS data will be far more limited, there's not much reason to completely handle a GXT file's data
            // So we'll just take a more direct approach
            using (Stream ddsStream = File.OpenRead(ddsPath))
            {
                using (Stream gxtStream = File.Create(gxtPath))
                {
                    using (BinaryReader reader = new BinaryReader(ddsStream))
                    {
                        using (BinaryWriter writer = new BinaryWriter(gxtStream))
                        {
                            reader.BaseStream.Seek(0x0C, SeekOrigin.Begin);
                            int texHeight = reader.ReadInt32();
                            int texWidth = reader.ReadInt32();
                            reader.BaseStream.Seek(0x4C, SeekOrigin.Begin);
                            int fmtBlockSize = reader.ReadInt32();
                            int fmtFlags = reader.ReadInt32();
                            SceGxmTextureBaseFormat fmt = 0;
                            if ((fmtFlags & 0x04) != 0)
                            {
                                string fourCC = new string(reader.ReadChars(4));
                                switch (fourCC)
                                {
                                    case "DXT1":
                                        fmt = SceGxmTextureBaseFormat.UBC1;
                                        break;
                                    case "DXT2":
                                        fmt = SceGxmTextureBaseFormat.UBC2;
                                        break;
                                    case "DXT3":
                                        fmt = SceGxmTextureBaseFormat.UBC2;
                                        break;
                                    case "DXT4":
                                        fmt = SceGxmTextureBaseFormat.UBC3;
                                        break;
                                    case "DXT5":
                                        fmt = SceGxmTextureBaseFormat.UBC3;
                                        break;

                                }
                            }
                            else
                            {
                                throw new NotImplementedException("Only supports Block compressed formats.");
                            }
                            reader.BaseStream.Seek(0x6C, SeekOrigin.Begin);
                            int lytFlags = reader.ReadInt32();
                            SceGxmTextureType type = (lytFlags & 0x00400000) != 0 ? SceGxmTextureType.Swizzled : SceGxmTextureType.Cube;
                            reader.BaseStream.Seek(0x80, SeekOrigin.Begin);
                            int blockSize = GetBlockSize(fmt);
                            int blockWidth = GetBlockWidth(fmt);
                            int pWidth = texWidth / blockWidth;
                            int pHeight = texHeight / blockWidth;
                            byte[] pixelData = reader.ReadBytes((pWidth * pHeight * blockSize) * (type == SceGxmTextureType.Cube ? 6 : 1));
                            writer.Write(new char[4] { 'G', 'X', 'T', '\0' });
                            writer.Write(0x10000003);
                            writer.Write(1);
                            writer.Write(0x40);
                            writer.Write((pWidth * pHeight * blockSize) * (type == SceGxmTextureType.Cube ? 6 : 1));
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write(0x40);
                            writer.Write((pWidth * pHeight * blockSize) * (type == SceGxmTextureType.Cube ? 6 : 1));
                            writer.Write(-1);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write((uint)fmt);
                            writer.Write((ushort)texWidth);
                            writer.Write((ushort)texHeight);
                            writer.Write((ushort)1);
                            writer.Write((ushort)0);

                            byte[] outPixelArray = new byte[pixelData.Length];
                            // PPC
                            for (int d = 0; d < (type == SceGxmTextureType.Cube ? 6 : 1); d++)
                            {

                                for (int y = 0; y < pHeight; y++)
                                {
                                    for (int x = 0; x < pWidth; x++)
                                    {
                                        int encPos = EncodeMorton(x, y);
    
                                        for (int p = 0; p < blockSize; p++)
                                        {
                                            Console.WriteLine((encPos * blockSize) + p);
                                            Console.WriteLine((((y * pWidth) + x) * blockSize) + p);
                                            outPixelArray[(encPos * blockSize) + p] = pixelData[(((y * pWidth) + x) * blockSize) + p];
                                        }
                                    }
                                }
                            }
                            writer.Write(outPixelArray);
                        }
                    }
                }
            }
        }

        private static int GetBitsPerPixel(SceGxmTextureBaseFormat fmt)
        {
            switch (fmt)
            {
                case SceGxmTextureBaseFormat.UBC1:
                    return 4;
                case SceGxmTextureBaseFormat.UBC2:
                case SceGxmTextureBaseFormat.UBC3:
                    return 8;
                case SceGxmTextureBaseFormat.U8U8U8:
                    return 24;
                case SceGxmTextureBaseFormat.U8U8U8U8:
                    return 32;
                default:
                    throw new NotImplementedException("Unsupported Format");
            }
        }

        /// <summary>
        /// Gets the width of a block.
        /// </summary>
        /// <param name="fmt">Texture Format</param>
        /// <returns>Number of pixels per block</returns>
        private static int GetBlockWidth(SceGxmTextureBaseFormat fmt)
        {
            switch (fmt)
            {
                case SceGxmTextureBaseFormat.UBC1:
                case SceGxmTextureBaseFormat.UBC2:
                case SceGxmTextureBaseFormat.UBC3:
                    return 4;
                case SceGxmTextureBaseFormat.PVRT2BPP:
                case SceGxmTextureBaseFormat.PVRT4BPP:
                case SceGxmTextureBaseFormat.PVRTII2BPP:
                case SceGxmTextureBaseFormat.PVRTII4BPP:
                    return 4;
                case SceGxmTextureBaseFormat.U8U8U8:
                    return 1;
                case SceGxmTextureBaseFormat.U8U8U8U8:
                    return 1;
                default:
                    return 1;
            }
        }

        // Borrowed from Scarlet: https://github.com/xdanieldzd/Scarlet

        // Copyright(c) 2016 xdaniel(Daniel R.) / DigitalZero Domain

        // Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
        // associated documentation files (the "Software"), to deal in the Software without restriction, including
        // without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        // copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

        //The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

        // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
        // THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
        // OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
        // OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

        // Unswizzle logic by @FireyFly
        // http://xen.firefly.nu/up/rearrange.c.html

        private static int Compact1By1(int x)
        {
            x &= 0x55555555;                    // x = -f-e -d-c -b-a -9-8 -7-6 -5-4 -3-2 -1-0
            x = (x ^ (x >> 1)) & 0x33333333;    // x = --fe --dc --ba --98 --76 --54 --32 --10
            x = (x ^ (x >> 2)) & 0x0f0f0f0f;    // x = ---- fedc ---- ba98 ---- 7654 ---- 3210
            x = (x ^ (x >> 4)) & 0x00ff00ff;    // x = ---- ---- fedc ba98 ---- ---- 7654 3210
            x = (x ^ (x >> 8)) & 0x0000ffff;    // x = ---- ---- ---- ---- fedc ba98 7654 3210
            return x;
        }

        private static int DecodeMorton2X(int code)
        {
            return Compact1By1(code >> 0);
        }

        private static int DecodeMorton2Y(int code)
        {
            return Compact1By1(code >> 1);
        }

        // Inverse of Compact1By1
        public static int EncodeMorton(int x, int y)
        {
            int encX = x;
            int encY = y;
            encX = encX & 0x0000FFFF;
            encY = encY & 0x0000FFFF;


            encX = ((encX | (encX << 8)) & 0x00FF00FF);
            encY = ((encY | (encY << 8)) & 0x00FF00FF);


            encX = ((encX | (encX << 4)) & 0x0F0F0F0F);
            encY = ((encY | (encY << 4)) & 0x0F0F0F0F);


            encX = ((encX | (encX << 2)) & 0x33333333);
            encY = ((encY | (encY << 2)) & 0x33333333);


            encX = ((encX | (encX << 1)) & 0x55555555);
            encY = ((encY | (encY << 1)) & 0x55555555);


            return ((encX << 1) | encY);
        }

        public GXT()
        {
            TextureData = new GXTTexData();
        }
    }
}
