using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;
using StringReader = AriaLibrary.IO.StringReader;

namespace AriaLibrary.Objects.GraphicsProgram.Nodes
{
    public enum VertexAttributeDataType : int
    {
        UnsignedByteIndex = 0,
        SignedByteIndex = 1,
        UnsignedByteNormalized = 4,
        SignedByteNormalized = 5,
        HalfFloat = 8,
        Float = 9
    }
    public class VertexAttribute
    {
        public int Offset;
        public int VertexBufferIndex;
        public int Count;
        public VertexAttributeDataType DataType;

        public void Read(BinaryReader reader)
        {
            Offset = reader.ReadInt32();
            VertexBufferIndex = reader.ReadInt32();
            Count = reader.ReadInt32();
            DataType = (VertexAttributeDataType)reader.ReadInt32();
        }
        public void Write(BinaryWriter writer)
        {
            writer.Write(Offset);
            writer.Write(VertexBufferIndex);
            writer.Write(Count);
            writer.Write((int)DataType);
        }
    }
    public class VXARData
    {
        public List<VertexAttribute> VertexAttributes;

        public void Read(BinaryReader reader, int dataPosition)
        {
            long cur = reader.BaseStream.Position;
            reader.BaseStream.Seek(dataPosition, SeekOrigin.Begin);

            int attributeCount = reader.ReadInt32();
            for (int i = 0; i < attributeCount; i++)
            {
                VertexAttribute attr = new VertexAttribute();
                attr.Read(reader);
                VertexAttributes.Add(attr);
            }

            reader.BaseStream.Seek(cur, SeekOrigin.Begin);
        }

        public void Write(BinaryWriter writer, int allocatedDataPosition)
        {
            writer.Seek(allocatedDataPosition, SeekOrigin.Begin);
            writer.Write(VertexAttributes.Count);
            foreach (var attr in VertexAttributes)
            {
                attr.Write(writer);
            }
        }

        public VXARData()
        {
            VertexAttributes = new List<VertexAttribute>();
        }
    }
    public class VXAR : GPRSection
    {
        public override string Type => "VXAR";
        public VXARData Data;
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
            Data = new VXARData();
            Data.Read(reader, heapDataOffset + dataOffset);
        }

        public override void Write(BinaryWriter writer, int allocatedHeapStringOffset, int allocatedHeapDataOffset, int allocatedHeapBufferOffset)
        {
            writer.Write(new char[4] { 'V', 'X', 'A', 'R' });
            writer.Write(allocatedHeapStringOffset);
            writer.Write(ReservedNameHash);
            // write name string to table
            long cur = writer.BaseStream.Position;
            writer.Seek(allocatedHeapStringOffset, SeekOrigin.Begin);
            writer.Write(Name.ToCharArray());
            writer.Seek((int)cur, SeekOrigin.Begin);
            // heap data
            writer.Write(allocatedHeapDataOffset);
            writer.Write(PositionHelper.PadValue((Data.VertexAttributes.Count * 16) + 4, 16));
            writer.Write(-1);
            writer.Write(0);
            writer.Write(0);
            // write Data
            cur = writer.BaseStream.Position;
            writer.Seek(allocatedHeapDataOffset, SeekOrigin.Begin);
            Data.Write(writer, allocatedHeapDataOffset);
            writer.Seek((int)cur, SeekOrigin.Begin);
        }

        public VXAR() : base()
        {
            Name = "";
            Data = new VXARData();
            Buffer = BufferName.VertexShader;
        }
    }
}
