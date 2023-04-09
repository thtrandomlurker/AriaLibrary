using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Objects.GraphicsProgram.Nodes
{
    public enum BufferName
    {
        Mesh,
        VertexShader,
        PixelShader
    }
    public abstract class GPRSection: IGPRSection
    {
        public abstract string Type { get; }
        public string Name;
        public int ReservedNameHash;
        public BufferName Buffer;
        public byte[] BufferData;
        public abstract void Read(BinaryReader reader, int heapStringOffset, int heapDataOffset, int heapVSBufferOffset, int heapMeshBufferOffset, int heapPSBufferOffset);
        public abstract void Write(BinaryWriter heapWriter, BinaryWriter stringWriter, BinaryWriter dataWriter, BinaryWriter bufferWriter, ref Dictionary<string, int> stringPosMap, ref List<int> sectionDataPositions, ref int curDataPositionIdx);

        public GPRSection()
        {
            Name = "";
            Buffer = BufferName.VertexShader;
            BufferData = new byte[0];
        }
    }
}
