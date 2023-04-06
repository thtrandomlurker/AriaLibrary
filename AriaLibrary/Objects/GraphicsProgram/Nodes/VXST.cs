using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;
using StringReader = AriaLibrary.IO.StringReader;

namespace AriaLibrary.Objects.GraphicsProgram.Nodes
{
    public class VXSTData
    {
        public int U00;
        public VXBOData VertexBindingObjectReference;
        public int U08;
        public int FaceIndexCount;
        public List<VXBFData> VertexBufferReferences;
        public int VXBFCount;
        public VXARData VertexArrayReference;
        public IXBFData IndexBufferReference;

        public void Read(BinaryReader reader, int dataPosition, int heapDataPosition)
        {
            long cur = reader.BaseStream.Position;
            reader.BaseStream.Seek(dataPosition, SeekOrigin.Begin);

            U00 = reader.ReadInt32();
            int vxboDataOffset = reader.ReadInt32();
            U08 = reader.ReadInt32();
            FaceIndexCount = reader.ReadInt32();
            int vxbfDataOffset = reader.ReadInt32();
            VXBFCount = reader.ReadInt32();
            int vxarDataOffset = reader.ReadInt32();
            int ixbfDataOffset = reader.ReadInt32();

            VertexBindingObjectReference.Read(reader, heapDataPosition + vxboDataOffset);
            for (int i = 0; i < VXBFCount; i++)
            {
                VXBFData data = new VXBFData();
                data.Read(reader, heapDataPosition + vxbfDataOffset + (0x10 * i));
                VertexBufferReferences.Add(data);
            }

            VertexArrayReference.Read(reader, heapDataPosition + vxarDataOffset);
            IndexBufferReference.Read(reader, heapDataPosition + ixbfDataOffset);

            reader.BaseStream.Seek(cur, SeekOrigin.Begin);
        }

        public void Write(BinaryWriter writer, int allocatedDataPosition)
        {
            writer.Seek(allocatedDataPosition, SeekOrigin.Begin);
            writer.Write(U00);
            writer.Write(allocatedDataPosition + 0x20);
            writer.Write(U08);
            writer.Write(FaceIndexCount);
            writer.Write(allocatedDataPosition + 0x20 + 0x70);
            writer.Write(VXBFCount);
            writer.Write(allocatedDataPosition + 0x20 + 0x70 + (0x10 * VXBFCount));
            writer.Write(allocatedDataPosition + 0x20 + 0x70 + (0x10 * VXBFCount) + PositionHelper.PadValue((VertexArrayReference.VertexAttributes.Count + 4), 16));
            VertexBindingObjectReference.Write(writer, allocatedDataPosition + 0x20);
            for (int i = 0; i < VXBFCount; i++)
            {
                VertexBufferReferences[i].Write(writer, allocatedDataPosition + 0x20 + 0x70 + (0x10 * i));
            }
            VertexArrayReference.Write(writer, allocatedDataPosition + 0x20 + 0x70 + (0x10 * VXBFCount));
            IndexBufferReference.Write(writer, allocatedDataPosition + 0x20 + 0x70 + (0x10 * VXBFCount) + PositionHelper.PadValue((VertexArrayReference.VertexAttributes.Count + 4), 16));


        }

        public VXSTData()
        {
            VertexBindingObjectReference = new VXBOData();
            VertexBufferReferences = new List<VXBFData>();
            VertexArrayReference = new VXARData();
            IndexBufferReference = new IXBFData();
        }
    }
    public class VXST : GPRSection
    {
        public override string Type => "VXST";
        public VXSTData Data;
        public override void Read(BinaryReader reader, int heapStringOffset, int heapDataOffset, int heapVSBufferOffset, int heapMeshBufferOffset, int heapPSBufferOffset)
        {
            int nameOffset = reader.ReadInt32();
            Name = StringReader.ReadNullTerminatedStringAtOffset(reader, heapStringOffset + nameOffset);
            ReservedNameHash = reader.ReadInt32();
            int dataOffset = reader.ReadInt32();
            int dataSize = reader.ReadInt32();
            // Unused in VXAR
            int bufferOffset = reader.ReadInt32();
            int bufferSize = reader.ReadInt32();

            Buffer = (BufferName)reader.ReadInt32();
            // Data
            Data.Read(reader, heapDataOffset + dataOffset, heapDataOffset);
        }

        public override void Write(BinaryWriter writer, int allocatedHeapStringOffset, int allocatedHeapDataOffset, int allocatedHeapBufferOffset)
        {
            writer.Write(new char[4] { 'V', 'X', 'S', 'T' });
            writer.Write(allocatedHeapStringOffset);
            writer.Write(ReservedNameHash);
            // write name string to table
            long cur = writer.BaseStream.Position;
            writer.Seek(allocatedHeapStringOffset, SeekOrigin.Begin);
            writer.Write(Name.ToCharArray());
            writer.Seek((int)cur, SeekOrigin.Begin);
            // heap data
            writer.Write(allocatedHeapDataOffset);
            writer.Write(0x20);
            writer.Write(-1);
            writer.Write(0);
            writer.Write(0);
            // write Data
            cur = writer.BaseStream.Position;
            writer.Seek(allocatedHeapDataOffset, SeekOrigin.Begin);
            Data.Write(writer, allocatedHeapDataOffset);
            writer.Seek((int)cur, SeekOrigin.Begin);
        }

        public VXST() : base()
        {
            Name = "";
            Data = new VXSTData();
            Buffer = BufferName.VertexShader;
        }
    }
}
