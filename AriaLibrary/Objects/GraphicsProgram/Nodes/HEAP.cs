using System;
using System.Collections.Generic;
using System.Linq;
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
            for (int i = 0; i < 5; i++)
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

        public HEAP()
        {
            Sections = new List<GPRSection>();
        }
    }
}
