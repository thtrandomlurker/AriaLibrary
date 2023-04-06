using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using StringReader = AriaLibrary.IO.StringReader;

namespace AriaLibrary.Objects.GraphicsProgram.Nodes
{
    public class VXBFData
    {
        public int U00;
        public int U04;
        public int VertexCount;
        public int VertexStride;

        public void Read(BinaryReader reader, int dataPosition)
        {
            long cur = reader.BaseStream.Position;
            reader.BaseStream.Seek(dataPosition, SeekOrigin.Begin);

            U00 = reader.ReadInt32();
            U04 = reader.ReadInt32();
            VertexCount = reader.ReadInt32();
            VertexStride = reader.ReadInt32();

            reader.BaseStream.Seek(cur, SeekOrigin.Begin);
        }

        public void Write(BinaryWriter writer, int allocatedDataPosition)
        {
            writer.Seek(allocatedDataPosition, SeekOrigin.Begin);
            writer.Write(U00);
            writer.Write(U04);
            writer.Write(VertexCount);
            writer.Write(VertexStride);
        }
    }
    public class VXBF : GPRSection
    {
        public override string Type => "VXBF";
        public VXBFData Data;
        public override void Read(BinaryReader reader, int heapStringOffset, int heapDataOffset, int heapVSBufferOffset, int heapMeshBufferOffset, int heapPSBufferOffset)
        {
            int nameOffset = reader.ReadInt32();
            Name = StringReader.ReadNullTerminatedStringAtOffset(reader, heapStringOffset + nameOffset);
            ReservedNameHash = reader.ReadInt32();
            int dataOffset = reader.ReadInt32();
            int dataSize = reader.ReadInt32();
            int bufferOffset = reader.ReadInt32();
            int bufferSize = reader.ReadInt32();
            Buffer = (BufferName)reader.ReadInt32();
            int cur = (int)reader.BaseStream.Position;
            // Data
            Data = new VXBFData();
            Data.Read(reader, heapDataOffset + dataOffset);
            // Buffer
            switch (Buffer)
            {
                case BufferName.Mesh:
                    reader.BaseStream.Seek(heapMeshBufferOffset + bufferOffset, SeekOrigin.Begin);
                    BufferData = reader.ReadBytes(bufferSize);
                    break;
                case BufferName.VertexShader:
                    reader.BaseStream.Seek(heapVSBufferOffset + bufferOffset, SeekOrigin.Begin);
                    BufferData = reader.ReadBytes(bufferSize);
                    break;
                case BufferName.PixelShader:
                    reader.BaseStream.Seek(heapPSBufferOffset + bufferOffset, SeekOrigin.Begin);
                    BufferData = reader.ReadBytes(bufferSize);
                    break;
                default:
                    throw new InvalidDataException($"Invalid buffer name in IXBF: {(int)Buffer}");
            }
            reader.BaseStream.Seek(cur, SeekOrigin.Begin);
        }

        public override void Write(BinaryWriter writer, int allocatedHeapStringOffset, int allocatedHeapDataOffset, int allocatedHeapBufferOffset)
        {
            writer.Write(new char[4] { 'V', 'X', 'B', 'F' });
            writer.Write(allocatedHeapStringOffset);
            writer.Write(ReservedNameHash);
            // write name string to table
            long cur = writer.BaseStream.Position;
            writer.Seek(allocatedHeapStringOffset, SeekOrigin.Begin);
            writer.Write(Name.ToCharArray());
            writer.Seek((int)cur, SeekOrigin.Begin);
            // heap data
            writer.Write(allocatedHeapDataOffset);
            writer.Write(0x10);
            writer.Write(allocatedHeapBufferOffset);
            writer.Write(BufferData.Length);
            writer.Write((int)Buffer);
            // write Data
            cur = writer.BaseStream.Position;
            writer.Seek(allocatedHeapDataOffset, SeekOrigin.Begin);
            Data.Write(writer, allocatedHeapDataOffset);
            writer.Seek((int)cur, SeekOrigin.Begin);
            // write Buffer
            cur = writer.BaseStream.Position;
            writer.Seek(allocatedHeapBufferOffset, SeekOrigin.Begin);
            writer.Write(BufferData);
            writer.Seek((int)cur, SeekOrigin.Begin);
        }

        public VXBF() : base()
        {
            BufferData = new byte[0];
            Name = "";
            Data = new VXBFData();
            Buffer = BufferName.Mesh;
        }
    }
}
