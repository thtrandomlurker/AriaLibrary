using AriaLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using StringReader = AriaLibrary.IO.StringReader;

namespace AriaLibrary.Objects.GraphicsProgram.Nodes
{
    public class ParameterCount
    {
        public string ParameterName;
        public int ParameterResourceIndex;
        public int ParameterArraySize;
        public void Read(BinaryReader reader, int heapStringOffset)
        {
            ParameterName = StringReader.ReadNullTerminatedStringAtOffset(reader, heapStringOffset + reader.ReadInt32());
            ParameterResourceIndex = reader.ReadInt32();
            ParameterArraySize = reader.ReadInt32();
        }
        public void Write(BinaryWriter dataWriter, BinaryWriter stringWriter, ref Dictionary<string, int> stringPosMap)
        {
            // deal with the name now
            if (stringPosMap.TryGetValue(ParameterName, out int value))
                dataWriter.Write(value);
            else
            {
                dataWriter.Write((int)stringWriter.BaseStream.Position);
                stringPosMap.Add(ParameterName, (int)stringWriter.BaseStream.Position);
                stringWriter.Write(ParameterName.ToCharArray());
                stringWriter.Write('\0');
            }
            dataWriter.Write(ParameterResourceIndex);
            dataWriter.Write(ParameterArraySize);
        }

        public ParameterCount()
        {
            ParameterName = "";
        }
    }
    public class SHBIData
    {
        public int U00;
        public int U04;
        public List<ParameterCount> Parameters;

        public void Read(BinaryReader reader, int heapDataOffset, int heapStringOffset)
        {
            long cur = reader.BaseStream.Position;
            reader.BaseStream.Seek(heapDataOffset, SeekOrigin.Begin);
            U00 = reader.ReadInt32();
            U04 = reader.ReadInt32();
            int parameterCount = reader.ReadInt32();

            for (int i = 0; i < parameterCount; i++)
            {
                ParameterCount input = new ParameterCount();
                input.Read(reader, heapStringOffset);
                Parameters.Add(input);
            }

            reader.BaseStream.Seek(cur, SeekOrigin.Begin);
        }

        public void Write(BinaryWriter dataWriter, BinaryWriter stringWriter, ref Dictionary<string, int> stringPosMap)
        {
            dataWriter.Write(U00);
            dataWriter.Write(U04);
            dataWriter.Write(Parameters.Count);
            foreach (var input in Parameters)
            {
                input.Write(dataWriter, stringWriter, ref stringPosMap);
            }
            PositionHelper.AlignWriter(dataWriter, 0x10);
        }

        public SHBIData(string inputName = "")
        {
            Parameters = new List<ParameterCount>();
        }
    }
    public class SHBI : GPRSection
    {
        public override string Type => "SHBI";
        public SHBIData Data;

        public override void Read(BinaryReader reader, int heapStringOffset, int heapDataOffset, int heapVSBufferOffset, int heapMeshBufferOffset, int heapPSBufferOffset)
        {
            int nameOffset = reader.ReadInt32();
            Name = StringReader.ReadNullTerminatedStringAtOffset(reader, heapStringOffset + nameOffset);
            ReservedNameHash = reader.ReadInt32();
            int dataOffset = reader.ReadInt32();
            int dataSize = reader.ReadInt32();
            // Unused in SHBI
            int bufferOffset = reader.ReadInt32();
            int bufferSize = reader.ReadInt32();

            Buffer = (BufferName)reader.ReadInt32();
            // Data
            Data = new SHBIData();
            Data.Read(reader, heapDataOffset + dataOffset, heapStringOffset);
        }

        public override void Write(BinaryWriter heapWriter, BinaryWriter stringWriter, BinaryWriter dataWriter, BinaryWriter bufferWriter, ref Dictionary<string, int> stringPosMap, ref List<int> sectionDataPositions, ref int curDataPositionIdx)
        {
            heapWriter.Write(new char[4] { 'S', 'H', 'B', 'I' });
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
            heapWriter.Write(PositionHelper.PadValue(0x0C + (Data.Parameters.Count * 0x0C), 0x10));
            heapWriter.Write(-1);
            heapWriter.Write(0);
            heapWriter.Write(0);
            sectionDataPositions.Add((int)dataWriter.BaseStream.Position);
            // write Data
            Data.Write(dataWriter, stringWriter, ref stringPosMap);
        }

        public SHBI() : base()
        {
            Name = "";
            Data = new SHBIData();
            Buffer = BufferName.VertexShader;
        }
    }
}
