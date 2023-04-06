using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;
using System.Numerics;

namespace AriaLibrary.Objects.Mesh
{
    public class IMTX : MeshBlock
    {
        public override string Type => "IMTX";
        public float[] Matrix;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            for (int i = 0; i < 12; i++)
                Matrix[i] = reader.ReadSingle();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'I', 'M', 'T', 'X' });
            writer.Write(0x30);
            for (int i = 0; i < 12; i++)
                writer.Write(Matrix[i]);
        }

        public IMTX()
        {
            Matrix = new float[12];
        }
    }
}
