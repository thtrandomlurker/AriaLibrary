using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Nodes
{
    public class PRIM : NodeBlock
    {
        public override string Type => "PRIM";
        public int PrimitiveID;
        public int U04;
        public int U08;
        public int U0C;
        public int U10;
        public int MaterialId;
        public int U18;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            PrimitiveID = reader.ReadInt32();
            U04 = reader.ReadInt32();
            U08 = reader.ReadInt32();
            U0C = reader.ReadInt32();
            U10 = reader.ReadInt32();
            MaterialId = reader.ReadInt32();
            U18 = reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'P', 'R', 'I', 'M' });
            writer.Write(0x1C);
            writer.Write(PrimitiveID);
            writer.Write(U04);
            writer.Write(U08);
            writer.Write(U0C);
            writer.Write(U10);
            writer.Write(MaterialId);
            writer.Write(U18);
        }

        public PRIM()
        {
        }
    }
}
