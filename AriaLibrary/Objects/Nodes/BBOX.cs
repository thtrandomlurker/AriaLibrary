using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Runtime.Intrinsics.X86;

namespace AriaLibrary.Objects.Nodes
{
    public class BBOX : NodeBlock
    {
        public override string Type => "BBOX";
        public int BoundingBoxName;
        public int U04;
        public int U08;
        public int U0C;

        public Matrix4x4 BoundingMatrix;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            long basePos = reader.BaseStream.Position;
            BoundingBoxName = reader.ReadInt32();
            U04 = reader.ReadInt32();
            U08 = reader.ReadInt32();
            U0C = reader.ReadInt32();
            BoundingMatrix[0, 0] = reader.ReadSingle();
            BoundingMatrix[0, 1] = reader.ReadSingle();
            BoundingMatrix[0, 2] = reader.ReadSingle();
            BoundingMatrix[0, 3] = reader.ReadSingle();
            BoundingMatrix[1, 0] = reader.ReadSingle();
            BoundingMatrix[1, 1] = reader.ReadSingle();
            BoundingMatrix[1, 2] = reader.ReadSingle();
            BoundingMatrix[1, 3] = reader.ReadSingle();
            BoundingMatrix[2, 0] = reader.ReadSingle();
            BoundingMatrix[2, 1] = reader.ReadSingle();
            BoundingMatrix[2, 2] = reader.ReadSingle();
            BoundingMatrix[2, 3] = reader.ReadSingle();
            BoundingMatrix[3, 0] = reader.ReadSingle();
            BoundingMatrix[3, 1] = reader.ReadSingle();
            BoundingMatrix[3, 2] = reader.ReadSingle();
            BoundingMatrix[3, 3] = reader.ReadSingle();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'B', 'B', 'O', 'X' });
            writer.Write(0x50);
            writer.Write(BoundingBoxName);
            writer.Write(U04);
            writer.Write(U08);
            writer.Write(U0C);
            writer.Write(BoundingMatrix[0, 0]);
            writer.Write(BoundingMatrix[0, 1]);
            writer.Write(BoundingMatrix[0, 2]);
            writer.Write(BoundingMatrix[0, 3]);
            writer.Write(BoundingMatrix[1, 0]);
            writer.Write(BoundingMatrix[1, 1]);
            writer.Write(BoundingMatrix[1, 2]);
            writer.Write(BoundingMatrix[1, 3]);
            writer.Write(BoundingMatrix[2, 0]);
            writer.Write(BoundingMatrix[2, 1]);
            writer.Write(BoundingMatrix[2, 2]);
            writer.Write(BoundingMatrix[2, 3]);
            writer.Write(BoundingMatrix[3, 0]);
            writer.Write(BoundingMatrix[3, 1]);
            writer.Write(BoundingMatrix[3, 2]);
            writer.Write(BoundingMatrix[3, 3]);
        }

        public BBOX()
        {
            BoundingMatrix = Matrix4x4.Identity;
        }
    }
}
