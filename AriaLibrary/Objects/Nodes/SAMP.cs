using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Nodes
{
    public class SAMP : NodeBlock
    {
        public override string Type => "SAMP";
        public int SamplerID;
        public List<SSTV> SSTVs;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            long basePos = reader.BaseStream.Position;
            SamplerID = reader.ReadInt32();
            while (reader.BaseStream.Position < basePos + dataSize)
            {
                // skip SSTV magic
                reader.BaseStream.Seek(4, SeekOrigin.Current);
                SSTV sstv = new SSTV();
                sstv.Read(reader);
                SSTVs.Add(sstv);
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'S', 'A', 'M', 'P' });
            writer.Write(0x4 + (0x14 * SSTVs.Count()));
            writer.Write(SamplerID);
            foreach (SSTV sstv in SSTVs)
                sstv.Write(writer);
        }

        public SAMP()
        {
            SSTVs = new List<SSTV>();
        }
    }
}
