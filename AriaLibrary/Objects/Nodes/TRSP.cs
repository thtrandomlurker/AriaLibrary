using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Nodes
{
    public enum CullMode : int
    {
        None,
        BackFace,
        FrontFace
    }
    public class TRSP : NodeBlock
    {
        public override string Type => "TRSP";
        public int TRSPId { get; set; }
        public CullMode Culling { get; set; }
        public int U08 { get; set; }
        public int U0C { get; set; }
        public int U10 { get; set; }
        public int U14 { get; set; }
        public int U18 { get; set; }
        public int U1C { get; set; }

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            TRSPId = reader.ReadInt32();
            Culling = (CullMode)reader.ReadInt32();
            U08 = reader.ReadInt32();
            U0C = reader.ReadInt32();
            U10 = reader.ReadInt32();
            U14 = reader.ReadInt32();
            U18 = reader.ReadInt32();
            U1C = reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'T', 'R', 'S', 'P' });
            writer.Write(0x20);
            writer.Write(TRSPId);
            writer.Write((int)Culling);
            writer.Write(U08);
            writer.Write(U0C);
            writer.Write(U10);
            writer.Write(U14);
            writer.Write(U18);
            writer.Write(U1C);
        }

        public TRSP()
        {
        }
    }
}
