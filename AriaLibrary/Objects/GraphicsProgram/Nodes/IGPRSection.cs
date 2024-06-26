﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Objects.GraphicsProgram.Nodes
{
    public interface IGPRSection
    {
        string Type { get; }

        void Read(BinaryReader reader, int heapStringOffset, int heapDataOffset, int heapVSBufferOffset, int heapMeshBufferOffset, int heapPSBufferOffset, string platform);
        void Write(BinaryWriter heapWriter, BinaryWriter stringWriter, BinaryWriter dataWriter, BinaryWriter bufferWriter, ref Dictionary<string, int> stringPosMap, ref List<int> sectionDataPositions, ref int curDataPositionIdx);
    }
}
