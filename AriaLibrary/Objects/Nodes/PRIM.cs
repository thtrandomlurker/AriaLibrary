using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Nodes
{
    public class PRIM : NodeBlock
    {
        public override string Type => "PRIM";
        public int PrimitiveID;
        public int MeshName;
        public int SetPolygonName;
        public int ObjectName;
        public int MeshNameDupe;
        public int MaterialID;
        public int U18;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            PrimitiveID = reader.ReadInt32();
            MeshName = reader.ReadInt32();
            SetPolygonName = reader.ReadInt32();
            ObjectName = reader.ReadInt32();
            MeshNameDupe = reader.ReadInt32();
            MaterialID = reader.ReadInt32();
            U18 = reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'P', 'R', 'I', 'M' });
            writer.Write(0x1C);
            writer.Write(PrimitiveID);
            writer.Write(MeshName);
            writer.Write(SetPolygonName);
            writer.Write(ObjectName);
            writer.Write(MeshNameDupe);
            writer.Write(MaterialID);
            writer.Write(U18);
        }

        public PRIM()
        {
        }
    }
}
