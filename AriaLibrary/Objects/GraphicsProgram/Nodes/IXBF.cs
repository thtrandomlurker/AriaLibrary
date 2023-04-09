using AriaLibrary.Helpers;
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

        public void Write(BinaryWriter dataWriter)
        {
            dataWriter.Write(U00);
            dataWriter.Write(U04);
            dataWriter.Write(U08);
            dataWriter.Write(U0C);
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

        public override void Write(BinaryWriter heapWriter, BinaryWriter stringWriter, BinaryWriter dataWriter, BinaryWriter bufferWriter, ref Dictionary<string, int> stringPosMap, ref List<int> sectionDataPositions, ref int curDataPositionIdx)
        {
            heapWriter.Write(new char[4] { 'I', 'X', 'B', 'F' });
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
            heapWriter.Write(0x10);
            heapWriter.Write((int)bufferWriter.BaseStream.Position);
            heapWriter.Write(BufferData.Length);
            heapWriter.Write((int)Buffer);
            sectionDataPositions.Add((int)dataWriter.BaseStream.Position);
            // write Data
            Data.Write(dataWriter);
            PositionHelper.AlignWriter(dataWriter, 0x10);
            // write Buffer
            bufferWriter.Write(BufferData);
            PositionHelper.AlignWriter(bufferWriter, 0x10);
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
