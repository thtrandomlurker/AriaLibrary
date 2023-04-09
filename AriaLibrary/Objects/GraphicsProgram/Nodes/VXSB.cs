using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AriaLibrary.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using StringReader = AriaLibrary.IO.StringReader;

namespace AriaLibrary.Objects.GraphicsProgram.Nodes
{
    public class VXSBData
    {
        public int U00;
        public VXSHData VertexShaderData;
        public SHBIData? ShaderBind0;
        public SHBIData? ShaderBind1;
        public void Read(BinaryReader reader, int dataOffset, int heapDataOffset, int heapStringOffset)
        {
            long cur = reader.BaseStream.Position;
            reader.BaseStream.Seek(dataOffset, SeekOrigin.Begin);

            U00 = reader.ReadInt32();
            int vertexShaderDataOffset = reader.ReadInt32();
            int shaderBind0Offset = reader.ReadInt32();
            int shaderBind1Offset = reader.ReadInt32();
            VertexShaderData.Read(reader, heapDataOffset + vertexShaderDataOffset, heapStringOffset);
            if (shaderBind0Offset != -1)
            {
                ShaderBind0 = new SHBIData();
                ShaderBind0.Read(reader, heapDataOffset + shaderBind0Offset, heapStringOffset);
            }
            if (shaderBind1Offset != -1)
            {
                ShaderBind1 = new SHBIData();
                ShaderBind1.Read(reader, heapDataOffset + shaderBind1Offset, heapStringOffset);
            }

            reader.BaseStream.Seek(cur, SeekOrigin.Begin);
        }

        public void Write(BinaryWriter dataWriter, BinaryWriter stringWriter, ref Dictionary<string, int> stringPosMap, ref List<int> sectionDataPositions, ref int curDataPositionIdx)
        {
            dataWriter.Write(U00);
            dataWriter.Write(sectionDataPositions[curDataPositionIdx]);
            curDataPositionIdx++;
            if (ShaderBind0 != null)
            {
                dataWriter.Write(sectionDataPositions[curDataPositionIdx]);
                curDataPositionIdx++;
            }
            else
            {
                dataWriter.Write(-1);
            }
            if (ShaderBind1 != null)
            {
                dataWriter.Write(sectionDataPositions[curDataPositionIdx]);
                curDataPositionIdx++;
            }
            else
            {
                dataWriter.Write(-1);
            }
        }
        public VXSBData()
        {
            VertexShaderData = new VXSHData();
        }
    }
    public class VXSB : GPRSection
    {
        public override string Type => "VXSB";
        public VXSBData Data;
        public override void Read(BinaryReader reader, int heapStringOffset, int heapDataOffset, int heapVSBufferOffset, int heapMeshBufferOffset, int heapPSBufferOffset)
        {
            int nameOffset = reader.ReadInt32();
            Name = StringReader.ReadNullTerminatedStringAtOffset(reader, heapStringOffset + nameOffset);
            ReservedNameHash = reader.ReadInt32();
            int dataOffset = reader.ReadInt32();
            int dataSize = reader.ReadInt32();
            // Unused in VXSB
            int bufferOffset = reader.ReadInt32();
            int bufferSize = reader.ReadInt32();

            Buffer = (BufferName)reader.ReadInt32();
            // Data
            Data.Read(reader, heapDataOffset + dataOffset, heapDataOffset, heapStringOffset);
        }

        public override void Write(BinaryWriter heapWriter, BinaryWriter stringWriter, BinaryWriter dataWriter, BinaryWriter bufferWriter, ref Dictionary<string, int> stringPosMap, ref List<int> sectionDataPositions, ref int curDataPositionIdx)
        {
            heapWriter.Write(new char[4] { 'V', 'X', 'S', 'B' });
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
            Data.Write(dataWriter, stringWriter, ref stringPosMap, ref sectionDataPositions, ref curDataPositionIdx);
        }

        public VXSB() : base()
        {
            Name = "";
            Data = new VXSBData();
            Buffer = BufferName.VertexShader;
        }
    }
}
