using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Nodes
{
    public class MATE : NodeBlock
    {
        public override string Type => "MATE";
        public int MaterialID;
        public int Name1;
        public int Name2;
        public int EffectID;
        public int VertexConstantID;
        public int Neg1;
        public int PixelConstantID;
        public int SamplerID;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            MaterialID = reader.ReadInt32();
            Name1 = reader.ReadInt32();
            Name2 = reader.ReadInt32();
            EffectID = reader.ReadInt32();
            VertexConstantID = reader.ReadInt32();
            Neg1 = reader.ReadInt32();
            PixelConstantID = reader.ReadInt32();
            SamplerID = reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'M', 'A', 'T', 'E' });
            writer.Write(0x20);
            writer.Write(MaterialID);
            writer.Write(Name1);
            writer.Write(Name2);
            writer.Write(EffectID);
            writer.Write(VertexConstantID);
            writer.Write(Neg1);
            writer.Write(PixelConstantID);
            writer.Write(SamplerID);
        }

        public MATE()
        {
        }
    }
}
