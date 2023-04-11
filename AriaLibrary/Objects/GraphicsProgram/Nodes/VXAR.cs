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
        UnsignedByte = 0,
        SignedByte = 1,
        UnsignedShort = 2,
        SignedShort = 3,
        UnsignedByteNormalized = 4,
        SignedByteNormalized = 5,
        UnsignedShortNormalized = 6,
        SignedShortNormalized = 7,
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
        public void Write(BinaryWriter dataWriter)
        {
            dataWriter.Write(Offset);
            dataWriter.Write(VertexBufferIndex);
            dataWriter.Write(Count);
            dataWriter.Write((int)DataType);
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

        public void Write(BinaryWriter dataWriter)
        {
            dataWriter.Write(VertexAttributes.Count);
            foreach (var attr in VertexAttributes)
            {
                attr.Write(dataWriter);
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

        public override void Write(BinaryWriter heapWriter, BinaryWriter stringWriter, BinaryWriter dataWriter, BinaryWriter bufferWriter, ref Dictionary<string, int> stringPosMap, ref List<int> sectionDataPositions, ref int curDataPositionIdx)
        {
            heapWriter.Write(new char[4] { 'V', 'X', 'A', 'R' });
            // deal with the name now
            if (stringPosMap.TryGetValue(Name, out int value))
                heapWriter.Write(value);
            else
            {
                heapWriter.Write((int)stringWriter.BaseStream.Position);
                stringPosMap.Add(Name, (int)stringWriter.BaseStream.Position);
                stringWriter.Write(Name.ToCharArray());
                stringWriter.Write('\0');

            }
            heapWriter.Write(ReservedNameHash);
            // heap data
            heapWriter.Write((int)dataWriter.BaseStream.Position);
            heapWriter.Write(PositionHelper.PadValue((Data.VertexAttributes.Count * 16) + 4, 16));
            heapWriter.Write(-1);
            heapWriter.Write(0);
            heapWriter.Write(0);
            sectionDataPositions.Add((int)dataWriter.BaseStream.Position);
            // write Data
            Data.Write(dataWriter);
            PositionHelper.AlignWriter(dataWriter, 0x10);
        }

        public VXAR() : base()
        {
            Name = "";
            Data = new VXARData();
            Buffer = BufferName.VertexShader;
        }
    }
}
