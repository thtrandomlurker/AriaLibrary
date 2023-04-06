using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Nodes
{
    public class INST : NodeBlock
    {
        public override string Type => "INST";
        public MESC MeshCluster;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            long basePos = reader.BaseStream.Position;
            while (reader.BaseStream.Position < basePos + dataSize)
            {
                char[] magic = reader.ReadChars(4);
                switch (new string(magic))
                {
                    case "MESC":
                        MeshCluster.Read(reader);
                        break;
                    default:
                        throw new InvalidDataException($"Invalid Node in INST: {new string(magic)}");
                }
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'I', 'N', 'S', 'T' });
            // 0x08 comes from the the MESC and it's size
            writer.Write(0x08 + MeshCluster.GetSize());
            MeshCluster.Write(writer);
        }

        public INST()
        {
            MeshCluster = new MESC();
        }
    }
}
