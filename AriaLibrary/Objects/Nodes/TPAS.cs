using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Nodes
{
    public class TPAS : NodeBlock
    {
        public override string Type => "TPAS";
        public int U00;
        public int U04;
        public int TPASId;
        public int VertexShaderName;
        public int PixelShaderName;
        public int U14;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            U00 = reader.ReadInt32();
            U04 = reader.ReadInt32();
            TPASId = reader.ReadInt32();
            VertexShaderName = reader.ReadInt32();
            PixelShaderName = reader.ReadInt32();
            U14 = reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'T', 'P', 'A', 'S' });
            writer.Write(0x18);
            writer.Write(U00);
            writer.Write(U04);
            writer.Write(TPASId);
            writer.Write(VertexShaderName);
            writer.Write(PixelShaderName);
            writer.Write(U14);
        }

        public TPAS()
        {
        }
    }
}
