using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace AriaLibrary.Archives
{
    public class KPackFile
    {
        public Stream? Stream;
        public Stream BaseStream;
        public int Offset;
        public int Size;

        public void Open()
        {
            using (BinaryReader reader = new BinaryReader(BaseStream, Encoding.UTF8, true))
            {
                reader.BaseStream.Seek(Offset, SeekOrigin.Begin);
                Stream = new MemoryStream(reader.ReadBytes(Size));
            }
        }

        public void Close()
        {
            Stream?.Close();
        }

        public KPackFile(int offset, int size, Stream baseStream)
        {
            Offset = offset;
            Size = size;
            BaseStream = baseStream;
        }
    }
    public class KPack
    {
        public List<KPackFile> Files;
        public Stream? BaseStream;

        public KPack()
        {
            Files = new List<KPackFile>();
            BaseStream = null;
        }

        public void Load(Stream fileStream)
        {
            BaseStream = fileStream;
            using (BinaryReader reader = new BinaryReader(fileStream, Encoding.UTF8, true))
            {
                int magic = reader.ReadInt32();
                if (magic != 0x794B504B)
                {
                    throw new InvalidDataException("Invalid KPack file.");
                }
                int fileCount = reader.ReadInt32();
                int unk = reader.ReadInt32();
                int[] tFileOffsets = new int[fileCount];
                int[] tFileSizes = new int[fileCount];
                for (int i = 0; i < fileCount; i++)
                {
                    tFileOffsets[i] = reader.ReadInt32();
                }
                for (int i = 0; i < fileCount; i++)
                {
                    tFileSizes[i] = reader.ReadInt32();
                }
                for (int i = 0; i < fileCount; i++)
                {
                    Files.Add(new KPackFile(tFileOffsets[i], tFileSizes[i], BaseStream));
                }
            }
        }
        public void Load(string filePath)
        {
            Stream file = File.OpenRead(filePath);
            Load(file);
        }
    }
}