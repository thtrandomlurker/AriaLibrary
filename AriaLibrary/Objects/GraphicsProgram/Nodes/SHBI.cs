using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using StringReader = AriaLibrary.IO.StringReader;

namespace AriaLibrary.Objects.GraphicsProgram.Nodes
{
    public class SHBIData
    {
        public int U00;
        public int U04;
        public int NumInputs;
        public string InputName;
        public int U10;
        public int U14;
        public int U18;
        public int U1C;

        public void Read(BinaryReader reader, int heapDataOffset, int heapStringOffset)
        {
            long cur = reader.BaseStream.Position;
            reader.BaseStream.Seek(heapDataOffset, SeekOrigin.Begin);
            U00 = reader.ReadInt32();
            U04 = reader.ReadInt32();
            NumInputs = reader.ReadInt32();
            InputName = StringReader.ReadNullTerminatedStringAtOffset(reader, reader.ReadInt32() + heapStringOffset);
            U10 = reader.ReadInt32();
            U14 = reader.ReadInt32();
            U18 = reader.ReadInt32();
            U1C = reader.ReadInt32();
            reader.BaseStream.Seek(cur, SeekOrigin.Begin);
        }

        public void Write(BinaryWriter dataWriter, BinaryWriter stringWriter, ref Dictionary<string, int> stringPosMap)
        {
            dataWriter.Write(U00);
            dataWriter.Write(U04);
            dataWriter.Write(NumInputs);
            if (stringPosMap.TryGetValue(InputName, out int value))
                dataWriter.Write(value);
            else
            {
                dataWriter.Write((int)stringWriter.BaseStream.Position);
                stringPosMap.Add(InputName, (int)stringWriter.BaseStream.Position);
                stringWriter.Write(InputName.ToCharArray());
                stringWriter.Write('\0');

            }
            dataWriter.Write(U10);
            dataWriter.Write(U14);
            dataWriter.Write(U18);
            dataWriter.Write(U1C);
        }

        public SHBIData(string inputName = "")
        {
            InputName = inputName;
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

        public override void Write(BinaryWriter heapWriter, BinaryWriter stringWriter, BinaryWriter dataWriter, BinaryWriter bufferWriter, ref Dictionary<string, int> stringPosMap)
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
            heapWriter.Write(0x20);
            heapWriter.Write(-1);
            heapWriter.Write(0);
            heapWriter.Write(0);
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
