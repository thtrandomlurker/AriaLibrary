using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;
using System.Numerics;
using System.Runtime.Intrinsics.X86;
using System.ComponentModel;

namespace AriaLibrary.Objects.Nodes
{
    public class NODE : NodeBlock
    {
        public override string Type => "NODE";
        public int NodeId { get; set; }
        public int NodeParent { get; set; }
        public int NodeChild { get; set; }
        public int NodeName { get; set; }

        public Matrix4x4 NodeMatrix { get; set; }

        public int One { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public INST? InstanceData { get; set; }

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            long basePos = reader.BaseStream.Position;
            NodeId = reader.ReadInt32();
            NodeParent = reader.ReadInt32();
            NodeChild = reader.ReadInt32();
            NodeName = reader.ReadInt32();
            Matrix4x4 tMatrix = new Matrix4x4();
            tMatrix[0, 0] = reader.ReadSingle();
            tMatrix[0, 1] = reader.ReadSingle();
            tMatrix[0, 2] = reader.ReadSingle();
            tMatrix[0, 3] = reader.ReadSingle();
            tMatrix[1, 0] = reader.ReadSingle();
            tMatrix[1, 1] = reader.ReadSingle();
            tMatrix[1, 2] = reader.ReadSingle();
            tMatrix[1, 3] = reader.ReadSingle();
            tMatrix[2, 0] = reader.ReadSingle();
            tMatrix[2, 1] = reader.ReadSingle();
            tMatrix[2, 2] = reader.ReadSingle();
            tMatrix[2, 3] = reader.ReadSingle();
            tMatrix[3, 0] = reader.ReadSingle();
            tMatrix[3, 1] = reader.ReadSingle();
            tMatrix[3, 2] = reader.ReadSingle();
            tMatrix[3, 3] = reader.ReadSingle();
            NodeMatrix = tMatrix;
            One = reader.ReadInt32();
            if (dataSize > 0x54)
            {
                // skip INST magic
                reader.BaseStream.Seek(4, SeekOrigin.Current);
                InstanceData = new INST();
                InstanceData.Read(reader);
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'N', 'O', 'D', 'E' });
            int size = 0;
            if (InstanceData != null)
            {
                size = 0x10 + InstanceData.MeshCluster.GetSize();
            }
            writer.Write(0x54 + size);
            writer.Write(NodeId);
            writer.Write(NodeParent);
            writer.Write(NodeChild);
            writer.Write(NodeName);
            writer.Write(NodeMatrix[0, 0]);
            writer.Write(NodeMatrix[0, 1]);
            writer.Write(NodeMatrix[0, 2]);
            writer.Write(NodeMatrix[0, 3]);
            writer.Write(NodeMatrix[1, 0]);
            writer.Write(NodeMatrix[1, 1]);
            writer.Write(NodeMatrix[1, 2]);
            writer.Write(NodeMatrix[1, 3]);
            writer.Write(NodeMatrix[2, 0]);
            writer.Write(NodeMatrix[2, 1]);
            writer.Write(NodeMatrix[2, 2]);
            writer.Write(NodeMatrix[2, 3]);
            writer.Write(NodeMatrix[3, 0]);
            writer.Write(NodeMatrix[3, 1]);
            writer.Write(NodeMatrix[3, 2]);
            writer.Write(NodeMatrix[3, 3]);
            writer.Write(One);
            InstanceData?.Write(writer);
        }

        public NODE()
        {
            NodeMatrix = Matrix4x4.Identity;
        }
    }
}
