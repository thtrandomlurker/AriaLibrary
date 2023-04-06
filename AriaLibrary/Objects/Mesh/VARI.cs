using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Mesh
{
    public class VARI : MeshBlock
    {
        public override string Type => "VARI";
        public int VARIId;
        public int U04;
        public int U08;
        public int U0C;
        public List<PRIM> PRIMs;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            long basePos = reader.BaseStream.Position;
            VARIId = reader.ReadInt32();
            U04 = reader.ReadInt32();
            U08 = reader.ReadInt32();
            U0C = reader.ReadInt32();
            while (reader.BaseStream.Position < basePos + dataSize)
            {
                // Skip PRIM magic
                reader.BaseStream.Seek(4, SeekOrigin.Current);
                PRIM prim = new PRIM();
                prim.Read(reader);
                PRIMs.Add(prim);
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'V', 'A', 'R', 'I' });
            writer.Write(0x10 + (0x24 * PRIMs.Count()));
            writer.Write(VARIId);
            writer.Write(U04);
            writer.Write(U08);
            writer.Write(U0C);
            foreach (PRIM prim in PRIMs)
                prim.Write(writer);
        }

        public VARI()
        {
            PRIMs = new List<PRIM>();
        }
    }
}
