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

        public void Write(BinaryWriter writer, int allocatedHeapDataOffset)
        {
            writer.Seek(allocatedHeapDataOffset, SeekOrigin.Begin);
            foreach (float val in Floats)
            {
                writer.Write(val);
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

        public override void Write(BinaryWriter writer, int allocatedHeapStringOffset, int allocatedHeapDataOffset, int allocatedHeapBufferOffset)
        {
            writer.Write(new char[4] { 'V', 'X', 'B', 'O' });
            writer.Write(allocatedHeapStringOffset);
            writer.Write(ReservedNameHash);
            // write name string to table
            long cur = writer.BaseStream.Position;
            writer.Seek(allocatedHeapStringOffset, SeekOrigin.Begin);
            writer.Write(Name.ToCharArray());
            writer.Seek((int)cur, SeekOrigin.Begin);
            // heap data
            writer.Write(allocatedHeapDataOffset);
            writer.Write(0x70);
            writer.Write(-1);
            writer.Write(0);
            writer.Write(0);
            // write Data
            cur = writer.BaseStream.Position;
            writer.Seek(allocatedHeapDataOffset, SeekOrigin.Begin);
            Data.Write(writer, allocatedHeapDataOffset);
            writer.Seek((int)cur, SeekOrigin.Begin);
        }

        public VXBO() : base()
        {
            Name = "";
            Data = new VXBOData();
            Buffer = BufferName.VertexShader;
        }
    }
}
