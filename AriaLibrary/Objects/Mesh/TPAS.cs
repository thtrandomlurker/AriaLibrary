using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Mesh
{
    public class TPAS : MeshBlock
    {
        public override string Type => "TPAS";
        public int U00;
        public int U04;
        public int U08;
        public int U0C;
        public int U10;
        public int U14;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            U00 = reader.ReadInt32();
            U04 = reader.ReadInt32();
            U08 = reader.ReadInt32();
            U0C = reader.ReadInt32();
            U10 = reader.ReadInt32();
            U14 = reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'T', 'P', 'A', 'S' });
            writer.Write(0x18);
            writer.Write(U00);
            writer.Write(U04);
            writer.Write(U08);
            writer.Write(U0C);
            writer.Write(U10);
            writer.Write(U14);
        }

        public TPAS()
        {
        }
    }
}
