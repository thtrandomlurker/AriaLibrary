using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StringReader = AriaLibrary.IO.StringReader;

namespace AriaLibrary.Objects.GraphicsProgram.Nodes
{
    public class IXBFData
    {
        public int U00;
        public int U04;
        public int U08;
        public int U0C;

        public void Read(BinaryReader reader, int dataPosition)
        {
            long cur = reader.BaseStream.Position;
            reader.BaseStream.Seek(dataPosition, SeekOrigin.Begin);

            U00 = reader.ReadInt32();
            U04 = reader.ReadInt32();
            U08 = reader.ReadInt32();
            U0C = reader.ReadInt32();

            reader.BaseStream.Seek(cur, SeekOrigin.Begin);
        }

        public void Write(BinaryWriter writer, int allocatedDataPosition)
        {
            writer.Seek(allocatedDataPosition, SeekOrigin.Begin);
            writer.Write(U00);
            writer.Write(U04);
            writer.Write(U08);
            writer.Write(U0C);
        }
    }
    public class IXBF : GPRSection
    {
        public override string Type => "IXBF";
        public IXBFData Data;
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
            Data = new IXBFData();
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
            writer.Write(new char[4] { 'I', 'X', 'B', 'F' });
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

        public IXBF() : base()
        {
            BufferData = new byte[0];
            Name = "";
            Data = new IXBFData();
            Buffer = BufferName.Mesh;
        }
    }
}
