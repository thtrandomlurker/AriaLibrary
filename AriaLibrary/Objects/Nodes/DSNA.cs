using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Nodes
{
    public class DSNA : NodeBlock
    {
        public override string Type => "DSNA";
        public int DSNAID;
        public int U04;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            DSNAID = reader.ReadInt32();
            U04 = reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'D', 'S', 'N', 'A' });
            writer.Write(0x08);
            writer.Write(DSNAID);
            writer.Write(U04);
        }

        public DSNA()
        {
        }
    }
}
