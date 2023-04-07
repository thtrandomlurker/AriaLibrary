using AriaLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using StringReader = AriaLibrary.IO.StringReader;

namespace AriaLibrary.Objects.GraphicsProgram.Nodes
{
    public class HEAP
    {
        public string? Name;
        public int ReservedNameHash;
        public List<GPRSection> Sections;

        public void Read(BinaryReader reader, int heapVSBufferOffset, int heapMeshBufferOffset, int heapPSBufferOffset)
        {
            int basePos = (int)reader.BaseStream.Position;
            int heapMagic = reader.ReadInt32();
            if (heapMagic != 0x50414548)
                throw new InvalidDataException("Heap is not HEAP.");
            int heapUnk = reader.ReadInt32();
            int heapSectionsOffset = reader.ReadInt32();
            int heapStringOffset = reader.ReadInt32();
            int heapDataOffset = reader.ReadInt32();
            int heapModelNameOffset = reader.ReadInt32();
            Name = StringReader.ReadNullTerminatedStringAtOffset(reader, basePos + heapStringOffset + heapModelNameOffset);
            ReservedNameHash = reader.ReadInt32();
            int heapSectionCount = reader.ReadInt32();
            Sections.Capacity = heapSectionCount;
            // test read first 55
            for (int i = 0; i < 4; i++)
            {
                string sectionType = new string(reader.ReadChars(4));

                switch (sectionType)
                {
                    case "VXBO":
                        VXBO vxbo = new VXBO();
                        vxbo.Read(reader, basePos + heapStringOffset, basePos + heapDataOffset, heapVSBufferOffset, heapMeshBufferOffset, heapPSBufferOffset);
                        Sections.Add(vxbo);
                        break;
                    case "VXAR":
                        VXAR vxar = new VXAR();
                        vxar.Read(reader, basePos + heapStringOffset, basePos + heapDataOffset, heapVSBufferOffset, heapMeshBufferOffset, heapPSBufferOffset);
                        Sections.Add(vxar);
                        break;
                    case "IXBF":
                        IXBF ixbf = new IXBF();
                        ixbf.Read(reader, basePos + heapStringOffset, basePos + heapDataOffset, heapVSBufferOffset, heapMeshBufferOffset, heapPSBufferOffset);
                        Sections.Add(ixbf);
                        break;
                    case "VXBF":
                        VXBF vxbf = new VXBF();
                        vxbf.Read(reader, basePos + heapStringOffset, basePos + heapDataOffset, heapVSBufferOffset, heapMeshBufferOffset, heapPSBufferOffset);
                        Sections.Add(vxbf);
                        break;
                    case "VXST":
                        VXST vxst = new VXST();
                        vxst.Read(reader, basePos + heapStringOffset, basePos + heapDataOffset, heapVSBufferOffset, heapMeshBufferOffset, heapPSBufferOffset);
                        Sections.Add(vxst);
                        break;
                    default:
                        throw new InvalidDataException($"Unknown section type {sectionType}");
                }
            }
        }

        public void Write(BinaryWriter heapWriter, BinaryWriter vsWriter, BinaryWriter meshWriter)
        {
            // use a writer for the strings as many sections contain data pointing to the stringStream
            using (BinaryWriter stringWriter = new BinaryWriter(new MemoryStream()))
                using (BinaryWriter dataWriter = new BinaryWriter(new MemoryStream()))
            {
                Dictionary<string, int> stringPosMap = new Dictionary<string, int>();
                heapWriter.Write(new char[4] { 'H', 'E', 'A', 'P' });
                heapWriter.Write(0);
                // Sections offset should always be 0x20
                heapWriter.Write(0x20);
                // Strings offset. this is simple
                heapWriter.Write(0x20 + (0x20 * Sections.Count));
                // Data offset. we can't do anything yet. write 0
                heapWriter.Write(0);
                // Model name offset. Can't do anything with this either yet. write 0
                heapWriter.Write(0);
                // this is always 0
                heapWriter.Write(0);
                heapWriter.Write(Sections.Count);
                foreach (var section in Sections)
                {
                    if (section.Buffer == BufferName.VertexShader)
                    {
                        section.Write(heapWriter, stringWriter, dataWriter, vsWriter, ref stringPosMap);
                    }
                    else if (section.Buffer == BufferName.Mesh)
                    {
                        section.Write(heapWriter, stringWriter, dataWriter, meshWriter, ref stringPosMap);
                    }
                }
                // sections written.
                heapWriter.Seek(0x14, SeekOrigin.Begin);
                heapWriter.Write((int)stringWriter.BaseStream.Position);
                if (Name != null)
                {
                    stringWriter.Write(Name.ToCharArray());
                    stringWriter.Write('\0');
                }
                // return to the end of the heap
                heapWriter.Seek(0, SeekOrigin.End);
                // align everything
                PositionHelper.AlignWriter(stringWriter, 0x10);
                PositionHelper.AlignWriter(dataWriter, 0x10);
                // finish up with the string writer
                stringWriter.Seek(0, SeekOrigin.Begin);
                stringWriter.BaseStream.CopyTo(heapWriter.BaseStream);
                // finish up with the data writer
                int dataPosition = (int)heapWriter.BaseStream.Position;
                dataWriter.Seek(0, SeekOrigin.Begin);
                dataWriter.BaseStream.CopyTo(heapWriter.BaseStream);
                heapWriter.Seek(0x10, SeekOrigin.Begin);
                heapWriter.Write(dataPosition);
                heapWriter.Seek(0, SeekOrigin.Begin);
            }
        }

        public HEAP()
        {
            Sections = new List<GPRSection>();
        }
    }
}
