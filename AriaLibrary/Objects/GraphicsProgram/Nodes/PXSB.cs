using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AriaLibrary.Helpers;
using StringReader = AriaLibrary.IO.StringReader;

namespace AriaLibrary.Objects.GraphicsProgram.Nodes
{
    public class PXSBData
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

        public void Write(BinaryWriter dataWriter, BinaryWriter stringWriter, ref Dictionary<string, int> stringPosMap)
        {
            int basePos = (int)dataWriter.BaseStream.Position;
            dataWriter.Write(U00);
            dataWriter.Write(basePos + 0x10);
            dataWriter.Write(basePos + 0x20 + (0x10 * VertexShaderData.Uniforms.Count()));
            dataWriter.Write(basePos + 0x20 + (0x10 * VertexShaderData.Uniforms.Count()) + 0x20);
            VertexShaderData.Write(dataWriter, stringWriter, ref stringPosMap);
            if (ShaderBind0 != null)
            {
                ShaderBind0.Write(dataWriter, stringWriter, ref stringPosMap);
            }
            else
                dataWriter.Write(-1);
            if (ShaderBind1 != null)
            {
                ShaderBind1.Write(dataWriter, stringWriter, ref stringPosMap);
            }
            else
                dataWriter.Write(-1);
            PositionHelper.AlignWriter(dataWriter, 0x10);
        }
        public PXSBData()
        {
            VertexShaderData = new VXSHData();
        }
    }
    public class PXSB : GPRSection
    {
        public override string Type => "PXSB";
        public PXSBData Data;
        public override void Read(BinaryReader reader, int heapStringOffset, int heapDataOffset, int heapVSBufferOffset, int heapMeshBufferOffset, int heapPSBufferOffset)
        {
            int nameOffset = reader.ReadInt32();
            Name = StringReader.ReadNullTerminatedStringAtOffset(reader, heapStringOffset + nameOffset);
            ReservedNameHash = reader.ReadInt32();
            int dataOffset = reader.ReadInt32();
            int dataSize = reader.ReadInt32();
            // Unused in PXSB
            int bufferOffset = reader.ReadInt32();
            int bufferSize = reader.ReadInt32();

            Buffer = (BufferName)reader.ReadInt32();
            // Data
            Data.Read(reader, heapDataOffset + dataOffset, heapDataOffset, heapStringOffset);
        }

        public override void Write(BinaryWriter heapWriter, BinaryWriter stringWriter, BinaryWriter dataWriter, BinaryWriter bufferWriter, ref Dictionary<string, int> stringPosMap)
        {
            heapWriter.Write(new char[4] { 'P', 'X', 'S', 'B' });
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

        public PXSB() : base()
        {
            Name = "";
            Data = new PXSBData();
            Buffer = BufferName.VertexShader;
        }
    }
}
