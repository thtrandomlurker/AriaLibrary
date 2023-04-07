using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StringReader = AriaLibrary.IO.StringReader;

namespace AriaLibrary.Objects.GraphicsProgram.Nodes
{
    public class VXBOData
    {
        public float[] Floats;

        public void Read(BinaryReader reader, int heapDataOffset)
        {
            long cur = reader.BaseStream.Position;
            reader.BaseStream.Seek(heapDataOffset, SeekOrigin.Begin);

            Floats = new float[4 * 7];
            for (int i = 0; i < 4 * 7; i++)
                Floats[i] = reader.ReadSingle();

            reader.BaseStream.Seek(cur, SeekOrigin.Begin);
        }

        public void Write(BinaryWriter dataWriter)
        {
            foreach (float val in Floats)
            {
                dataWriter.Write(val);
            }
        }

        public VXBOData()
        {
            Floats = new float[4 * 7];
        }
    }
    public class VXBO: GPRSection
    {
        public override string Type => "VXBO";
        public VXBOData Data;

        public override void Read(BinaryReader reader, int heapStringOffset, int heapDataOffset, int heapVSBufferOffset, int heapMeshBufferOffset, int heapPSBufferOffset)
        {
            int nameOffset = reader.ReadInt32();
            Name = StringReader.ReadNullTerminatedStringAtOffset(reader, heapStringOffset + nameOffset);
            ReservedNameHash = reader.ReadInt32();
            int dataOffset = reader.ReadInt32();
            int dataSize = reader.ReadInt32();
            // Unused in VXBO
            int bufferOffset = reader.ReadInt32();
            int bufferSize = reader.ReadInt32();

            Buffer = (BufferName)reader.ReadInt32();
            // Data
            Data = new VXBOData();
            Data.Read(reader, heapDataOffset + dataOffset);
        }

        public override void Write(BinaryWriter heapWriter, BinaryWriter stringWriter, BinaryWriter dataWriter, BinaryWriter bufferWriter, ref Dictionary<string, int> stringPosMap)
        {
            heapWriter.Write(new char[4] { 'V', 'X', 'B', 'O' });
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
            heapWriter.Write(0x70);
            heapWriter.Write(-1);
            heapWriter.Write(0);
            heapWriter.Write(0);
            // write Data
            Data.Write(dataWriter);
        }

        public VXBO() : base()
        {
            Name = "";
            Data = new VXBOData();
            Buffer = BufferName.VertexShader;
        }
    }
}
