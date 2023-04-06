using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Nodes
{
    public class BOIF : NodeBlock
    {
        public override string Type => "BOIF";
        public int BoneName;
        public int BoneId;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            BoneName = reader.ReadInt32();
            BoneId = reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'B', 'O', 'I', 'F' });
            writer.Write(0x08);
            writer.Write(BoneName);
            writer.Write(BoneId);
        }

        public BOIF()
        {
        }
    }
}
