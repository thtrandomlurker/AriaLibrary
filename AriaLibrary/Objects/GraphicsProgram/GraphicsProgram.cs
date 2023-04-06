using AriaLibrary.Objects.GraphicsProgram.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Objects.GraphicsProgram
{
    public class GraphicsProgram
    {
        public HEAP Heap;

        public void Read(BinaryReader reader, int basePosition = 0)
        {
            long gprMagic = reader.ReadInt64();
            if (gprMagic != 0x0000000000525047)
            {
                throw new InvalidDataException($"Attempted to read GPR but found {gprMagic} instead. Reported base position is at {basePosition}.");
            }
            int gxmMagic = reader.ReadInt32();
            int gxmCombinedSize = reader.ReadInt32();
            int flags = reader.ReadInt32();
            int headerSize = reader.ReadInt32();
            int heapOffset = reader.ReadInt32();
            int heapSize = reader.ReadInt32();
            int heapVSBufferOffset = reader.ReadInt32();
            int heapVSBufferSize = reader.ReadInt32();
            int heapMeshBufferOffset = reader.ReadInt32();
            int heapMeshBufferSize = reader.ReadInt32();
            int heapPSBufferOffset = reader.ReadInt32();
            int heapPSBufferSize = reader.ReadInt32();
            reader.BaseStream.Seek(heapOffset+16, SeekOrigin.Begin);
            Heap.Read(reader, 16 + heapVSBufferOffset, 16 + heapMeshBufferOffset, 16 + heapPSBufferOffset);
        }

        public void Load(string filePath)
        {
            Stream file = File.OpenRead(filePath);
            using (BinaryReader reader = new BinaryReader(file))
            {
                Read(reader);
            }
        }
        public void Load(Stream file)
        {
            using (BinaryReader reader = new BinaryReader(file))
            {
                Read(reader);
            }
        }

        public GraphicsProgram()
        {
            Heap = new HEAP();
        }
    }
}
