using AriaLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Objects.Nodes
{
    public class MESH
    {
        public REM Remark;
        public STRB StringBuffer;
        public List<NodeBlock> ChildNodes;

        public void Read(BinaryReader reader)
        {
            char[] magic = reader.ReadChars(4);
            if (magic[0] != 'M' | magic[1] != 'E' | magic[2] != 'S' | magic[3] != 'H')
                throw new InvalidDataException("Invalid MESH");
            int dataSize = reader.ReadInt32();
            long basePos = reader.BaseStream.Position;
            while (reader.BaseStream.Position < basePos + dataSize)
            {
                char[] nodeType = reader.ReadChars(4);
                switch (new string(nodeType))
                {
                    case "REM\0":
                        Remark = new REM();
                        Remark.Read(reader);
                        break;
                    case "STRB":
                        StringBuffer = new STRB();
                        StringBuffer.Read(reader);
                        break;
                    case "DSNA":
                        DSNA dsna = new DSNA();
                        dsna.Read(reader);
                        ChildNodes.Add(dsna);
                        break;
                    case "TRSP":
                        TRSP trsp = new TRSP();
                        trsp.Read(reader);
                        ChildNodes.Add(trsp);
                        break;
                    case "CSTS":
                        CSTS csts = new CSTS();
                        csts.Read(reader);
                        ChildNodes.Add(csts);
                        break;
                    case "EFFE":
                        EFFE effe = new EFFE();
                        effe.Read(reader);
                        ChildNodes.Add(effe);
                        break;
                    case "SAMP":
                        SAMP samp = new SAMP();
                        samp.Read(reader);
                        ChildNodes.Add(samp);
                        break;
                    case "MATE":
                        MATE mate = new MATE();
                        mate.Read(reader);
                        ChildNodes.Add(mate);
                        break;
                    case "VARI":
                        VARI vari = new VARI();
                        vari.Read(reader);
                        ChildNodes.Add(vari);
                        break;
                    case "BONE":
                        BONE bone = new BONE();
                        bone.Read(reader);
                        ChildNodes.Add(bone);
                        break;
                    default:
                        throw new InvalidDataException($"Invalid Node Type {new string(nodeType)}");
                }
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'M', 'E', 'S', 'H' });
            // temp until we know the size
            writer.Write(0);
            Remark.Write(writer);
            StringBuffer.Write(writer);
            foreach (var block in ChildNodes)
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
        public void Save(Stream file, bool leaveOpen)
        {
            using (BinaryWriter writer = new BinaryWriter(file, Encoding.UTF8, leaveOpen))
            {
                Write(writer);
            }
        }

        public MESH()
        {
            Remark = new REM("Created using AriaLibrary");
            StringBuffer = new STRB();
            ChildNodes = new List<NodeBlock>();
        }
    }
}
