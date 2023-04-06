using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Mesh
{
    public class MATE : MeshBlock
    {
        public override string Type => "MATE";
        public int MaterialID;
        public int Name1;
        public int Name2;
        public int EffectID;
        public int VertexConstantId;
        public int Neg1;
        public int FragmentConstantId;
        public int SamplerId;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            MaterialID = reader.ReadInt32();
            Name1 = reader.ReadInt32();
            Name2 = reader.ReadInt32();
            EffectID = reader.ReadInt32();
            VertexConstantId = reader.ReadInt32();
            Neg1 = reader.ReadInt32();
            FragmentConstantId = reader.ReadInt32();
            SamplerId = reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'M', 'A', 'T', 'E' });
            writer.Write(0x20);
            writer.Write(MaterialID);
            writer.Write(Name1);
            writer.Write(Name2);
            writer.Write(EffectID);
            writer.Write(VertexConstantId);
            writer.Write(Neg1);
            writer.Write(FragmentConstantId);
            writer.Write(SamplerId);
        }

        public MATE()
        {
        }
    }
}
