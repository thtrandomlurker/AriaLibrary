using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Nodes
{
    public class CSTV : NodeBlock
    {
        public override string Type => "CSTV";
        public int ConstantName;
        public int ConstantDataName;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            ConstantName = reader.ReadInt32();
            ConstantDataName = reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'C', 'S', 'T', 'V' });
            writer.Write(0x08);
            writer.Write(ConstantName);
            writer.Write(ConstantDataName);
        }

        public CSTV()
        {
        }
    }
}
