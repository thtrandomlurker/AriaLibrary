using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StringReader = AriaLibrary.IO.StringReader;
using System.Numerics;
using AriaLibrary.Helpers;
using Microsoft.VisualBasic;

namespace AriaLibrary.Objects.GraphicsProgram.Nodes
{
    public class SHCOData
    {
        public List<Vector4> Constants;

        public void Read(BinaryReader reader, int heapDataOffset)
        {
            long cur = reader.BaseStream.Position;
            reader.BaseStream.Seek(heapDataOffset, SeekOrigin.Begin);
            int constantCount = reader.ReadInt32();
            for (int i = 0; i < constantCount; i++)
            {
                Constants.Add(new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
            }

            reader.BaseStream.Seek(cur, SeekOrigin.Begin);
        }

        public void Write(BinaryWriter dataWriter)
        {
            dataWriter.Write(Constants.Count);
            foreach (Vector4 constant in Constants)
            {
                dataWriter.Write(constant.X);
                dataWriter.Write(constant.Y);
                dataWriter.Write(constant.Z);
                dataWriter.Write(constant.W);
            }
            PositionHelper.AlignWriter(dataWriter, 0x10);
        }

        public SHCOData()
        {
            Constants = new List<Vector4>();
        }
    }
    public class SHCO: GPRSection
    {
        public override string Type => "SHCO";
        public SHCOData Data;

        public override void Read(BinaryReader reader, int heapStringOffset, int heapDataOffset, int heapVSBufferOffset, int heapMeshBufferOffset, int heapPSBufferOffset, string platform)
        {
            int nameOffset = reader.ReadInt32();
            Name = StringReader.ReadNullTerminatedStringAtOffset(reader, heapStringOffset + nameOffset);
            ReservedNameHash = reader.ReadInt32();
            int dataOffset = reader.ReadInt32();
            int dataSize = reader.ReadInt32();
            // Unused in SHCO
            int bufferOffset = reader.ReadInt32();
            int bufferSize = reader.ReadInt32();

            Buffer = (BufferName)reader.ReadInt32();
            // Data
            Data = new SHCOData();
            Data.Read(reader, heapDataOffset + dataOffset);
        }

        public override void Write(BinaryWriter heapWriter, BinaryWriter stringWriter, BinaryWriter dataWriter, BinaryWriter bufferWriter, ref Dictionary<string, int> stringPosMap, ref List<int> sectionDataPositions, ref int curDataPositionIdx)
        {
            heapWriter.Write(new char[4] { 'S', 'H', 'C', 'O' });
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
            heapWriter.Write(PositionHelper.PadValue(0x04 + (0x10 * Data.Constants.Count), 0x10));
            heapWriter.Write(-1);
            heapWriter.Write(0);
            heapWriter.Write(0);
            // write Data
            Data.Write(dataWriter);
        }

        public SHCO() : base()
        {
            Name = "";
            Data = new SHCOData();
            Buffer = BufferName.VertexShader;
        }
    }
}
