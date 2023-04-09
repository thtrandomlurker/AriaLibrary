using AriaLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Objects.Nodes
{
    public class NODT: NodeBlock
    {
        public override string Type => "NODT";
        public REM Remark;
        public STRB StringBuffer;
        public List<NodeBlock> ChildNodes;

        public override void Read(BinaryReader reader)
        {
            char[] magic = reader.ReadChars(4);
            if (magic[0] != 'N' | magic[1] != 'O' | magic[2] != 'D' | magic[3] != 'T')
                throw new InvalidDataException("Invalid NODT");
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
                    case "NODE":
                        NODE node = new NODE();
                        node.Read(reader);
                        ChildNodes.Add(node);
                        break;
                    default:
                        throw new InvalidDataException($"Invalid Node Type {new string(nodeType)}");
                }
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'N', 'O', 'D', 'T' });
            // temp until we know the size
            writer.Write(0);
            Remark.Write(writer);
            StringBuffer.Write(writer);
            foreach (var block in ChildNodes)
            {
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

        public NODT()
        {
            Remark = new REM("Created using AriaLibrary");
            StringBuffer = new STRB();
            ChildNodes = new List<NodeBlock>();
        }
    }
}
