using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Mesh
{
    public class CSTS : MeshBlock
    {
        public override string Type => "CSTS";
        public int ConstantSetId;
        public List<CSTV> ConstantValues;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            long basePos = reader.BaseStream.Position;
            ConstantSetId = reader.ReadInt32();
            while (reader.BaseStream.Position < basePos + dataSize)
            {
                // Skip CSTV magic
                reader.BaseStream.Seek(4, SeekOrigin.Current);
                CSTV cstv = new CSTV();
                cstv.Read(reader);
                ConstantValues.Add(cstv);
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'C', 'S', 'T', 'S' });
            writer.Write(0x04 + (0x10 * ConstantValues.Count()));
            writer.Write(ConstantSetId);
            foreach (CSTV cstv in ConstantValues)
                cstv.Write(writer);
        }

        public CSTS()
        {
            ConstantValues = new List<CSTV>();
        }
    }
}
