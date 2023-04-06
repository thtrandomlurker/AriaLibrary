using AriaLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Objects.Mesh
{
    public class Mesh
    {
        public REM Remark;
        public STRB StringBlock;
        public List<MeshBlock> MeshBlocks;

        public void Read(BinaryReader reader)
        {
            char[] magic = reader.ReadChars(4);
            if (magic[0] != 'M' || magic[1] != 'E' || magic[2] != 'S' || magic[3] != 'H')
                throw new InvalidDataException($"Invalid MESH file: Expected \"MESH\" got {new string(magic)}");
            int dataSize = reader.ReadInt32();
            long basePos = reader.BaseStream.Position;
            while (reader.BaseStream.Position < basePos + dataSize)
            {
                char[] blockMagic = reader.ReadChars(4);
                switch (new string(blockMagic))
                {
                    case "REM\0":
                        Remark = new REM();
                        Remark.Read(reader);
                        break;
                    case "STRB":
                        StringBlock = new STRB();
                        StringBlock.Read(reader);
                        break;
                    case "DSNA":
                        DSNA dsnaBlock = new DSNA();
                        dsnaBlock.Read(reader);
                        MeshBlocks.Add(dsnaBlock);
                        break;
                    case "TRSP":
                        TRSP trspBlock = new TRSP();
                        trspBlock.Read(reader);
                        MeshBlocks.Add(trspBlock);
                        break;
                    case "CSTS":
                        CSTS cstsBlock = new CSTS();
                        cstsBlock.Read(reader);
                        MeshBlocks.Add(cstsBlock);
                        break;
                    case "EFFE":
                        EFFE effeBlock = new EFFE();
                        effeBlock.Read(reader);
                        MeshBlocks.Add(effeBlock);
                        break;
                    case "SAMP":
                        SAMP sampBlock = new SAMP();
                        sampBlock.Read(reader);
                        MeshBlocks.Add(sampBlock);
                        break;
                    case "MATE":
                        MATE mateBlock = new MATE();
                        mateBlock.Read(reader);
                        MeshBlocks.Add(mateBlock);
                        break;
                    case "VARI":
                        VARI variBlock = new VARI();
                        variBlock.Read(reader);
                        MeshBlocks.Add(variBlock);
                        break;
                    case "BONE":
                        BONE boneBlock = new BONE();
                        boneBlock.Read(reader);
                        MeshBlocks.Add(boneBlock);
                        break;
                    default:
                        Console.WriteLine(new string(blockMagic));
                        int blockDataSize = reader.ReadInt32();
                        reader.BaseStream.Seek(blockDataSize, SeekOrigin.Current);
                        break;
                }
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'M', 'E', 'S', 'H' });
            // temp until we know the size
            writer.Write(0);
            Remark.Write(writer);
            StringBlock.Write(writer);
            foreach (var block in MeshBlocks)
            {
                Console.WriteLine(block.Type);
                Console.WriteLine(writer.BaseStream.Position);
                block.Write(writer);
            }
            writer.BaseStream.Seek(4, SeekOrigin.Begin);
            writer.Write((int)(writer.BaseStream.Length - 8));
        }

        public void Load(string filePath)
        {
            Stream file = File.OpenRead(filePath);
            Load(file);
        }
        public void Load(Stream file)
        {
            using (BinaryReader reader = new BinaryReader(file))
            {
                Read(reader);
            }
        }

        public void Save(string filePath)
        {
            Stream file = File.Create(filePath);
            Save(file);
        }

        public void Save(Stream file)
        {
            using (BinaryWriter writer = new BinaryWriter(file))
            {
                Write(writer);
            }
        }

        public Mesh()
        {
            Remark = new REM("Created using AriaLibrary");
            StringBlock = new STRB();
            MeshBlocks = new List<MeshBlock>();
        }
    }
}
