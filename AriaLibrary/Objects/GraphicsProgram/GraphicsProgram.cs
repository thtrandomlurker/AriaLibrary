using AriaLibrary.Helpers;
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

        public void Read(BinaryReader reader)
        {
            long gprMagic = reader.ReadInt64();
            if (gprMagic != 0x0000000000525047)
            {
                throw new InvalidDataException($"Attempted to read GPR but found {gprMagic} instead.");
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

        public void Write(BinaryWriter writer)
        {
            using (BinaryWriter heapWriter = new BinaryWriter(new MemoryStream()))
                using (BinaryWriter vsWriter = new BinaryWriter(new MemoryStream()))
                    using (BinaryWriter meshWriter = new BinaryWriter(new MemoryStream()))
            {
                writer.Write(new char[8] { 'G', 'P', 'R', '\0', '\0', '\0', '\0', '\0' });
                writer.Write(new char[4] { 'G', 'X', 'M', '\0' });
                // write 0 until we know the data size.
                // NOTE: GXM data size is the sum of the header size, HEAP size, VSBuffer size, Mesh buffer size, and PS Buffer size (Assumed due to a 3rd possible buffer that's always unused iirc)
                writer.Write(0);
                writer.Write(0x20081001);
                // header size. should always be 0x30
                writer.Write(0x30);
                // Heap Offset. should also always be 0x30
                writer.Write(0x30);
                // Heap size. Write 0 until we know the size
                writer.Write(0);
                // Vertex Shader Buffer offset. Write 0 until we know the position
                writer.Write(0);
                // Vertex Shader Buffer size. Write 0 until we know the size.
                writer.Write(0);
                // Mesh Buffer offset. Write 0 until we know the position
                writer.Write(0);
                // Mesh Buffer size. Write 0 until we know the size.
                writer.Write(0);
                // Pixel Shader Buffer offset. We probably won't use it.
                writer.Write(-1);
                // Pixel Shader Buffer size. We probably won't use it
                writer.Write(0);
                // align to write the HEAP
                PositionHelper.AlignWriter(writer, 0x10);
                Heap.Write(heapWriter, vsWriter, meshWriter);
                // align everything for safety
                PositionHelper.AlignWriter(heapWriter, 0x10);
                PositionHelper.AlignWriter(vsWriter, 0x10);
                PositionHelper.AlignWriter(meshWriter, 0x10);
                // copy the heap
                heapWriter.Seek(0, SeekOrigin.Begin);
                heapWriter.BaseStream.CopyTo(writer.BaseStream);
                PositionHelper.AlignWriter(writer, 0x800);  // data after the heap seems to always be aligned to 0x800
                // and the vs
                int vsPos = vsWriter.BaseStream.Length == 0 ? -1 : (int)writer.BaseStream.Position - 0x10;
                vsWriter.Seek(0, SeekOrigin.Begin);
                vsWriter.BaseStream.CopyTo(writer.BaseStream);
                PositionHelper.AlignWriter(writer, 0x80);  // data after the vs buffer seems to always be aligned to 0x80
                // and the mesh
                int meshPos = (int)writer.BaseStream.Position - 0x10;
                meshWriter.Seek(0, SeekOrigin.Begin);
                meshWriter.BaseStream.CopyTo(writer.BaseStream);
                // set the output values
                writer.Seek(0xC, SeekOrigin.Begin);
                writer.Write(0x30 + (int)(heapWriter.BaseStream.Length + vsWriter.BaseStream.Length + meshWriter.BaseStream.Length));
                writer.Seek(0x1C, SeekOrigin.Begin);
                writer.Write((int)heapWriter.BaseStream.Length);
                writer.Write(vsPos);
                writer.Write((int)vsWriter.BaseStream.Length);
                writer.Write(meshPos);
                writer.Write((int)meshWriter.BaseStream.Length);
            }
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

        public void Save(string filePath)
        {
            Stream file = File.Create(filePath);
            using (BinaryWriter writer = new BinaryWriter(file))
            {
                Write(writer);
            }
        }
        public void Save(Stream file)
        {
            using (BinaryWriter writer = new BinaryWriter(file))
            {
                Write(writer);
            }
        }
        public void Save(Stream file, bool leaveOpen)
        {
            using (BinaryWriter writer = new BinaryWriter(file, Encoding.UTF8, leaveOpen))
            {
                Write(writer);
            }
        }

        public GraphicsProgram()
        {
            Heap = new HEAP();
        }
    }
}
