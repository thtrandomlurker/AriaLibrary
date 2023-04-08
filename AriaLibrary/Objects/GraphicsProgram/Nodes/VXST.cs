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

        public void Write(BinaryWriter dataWriter)
        {
            int basePos = (int)dataWriter.BaseStream.Position;
            dataWriter.Write(U00);
            dataWriter.Write(basePos + 0x20);
            dataWriter.Write(U08);
            dataWriter.Write(FaceIndexCount);
            dataWriter.Write(basePos + 0x20 + 0x70);
            dataWriter.Write(VXBFCount);
            dataWriter.Write(basePos + 0x20 + 0x70 + (0x10 * VXBFCount));
            dataWriter.Write(basePos + 0x20 + 0x70 + (0x10 * VXBFCount) + PositionHelper.PadValue((VertexArrayReference.VertexAttributes.Count + 4), 16));
            VertexBindingObjectReference.Write(dataWriter);
            foreach (var vxbf in VertexBufferReferences)
            {
                vxbf.Write(dataWriter);
            }
            VertexArrayReference.Write(dataWriter);
            IndexBufferReference.Write(dataWriter);
            PositionHelper.AlignWriter(dataWriter, 0x10);

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
            // Unused in VXST
            int bufferOffset = reader.ReadInt32();
            int bufferSize = reader.ReadInt32();

            Buffer = (BufferName)reader.ReadInt32();
            // Data
            Data.Read(reader, heapDataOffset + dataOffset, heapDataOffset);
        }

        public override void Write(BinaryWriter heapWriter, BinaryWriter stringWriter, BinaryWriter dataWriter, BinaryWriter bufferWriter, ref Dictionary<string, int> stringPosMap)
        {
            heapWriter.Write(new char[4] { 'V', 'X', 'S', 'T' });
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
            heapWriter.Write(0x20);
            heapWriter.Write(-1);
            heapWriter.Write(0);
            heapWriter.Write(0);
            // write Data
            Data.Write(dataWriter);
        }

        public VXST() : base()
        {
            Name = "";
            Data = new VXSTData();
            Buffer = BufferName.VertexShader;
        }
    }
}
