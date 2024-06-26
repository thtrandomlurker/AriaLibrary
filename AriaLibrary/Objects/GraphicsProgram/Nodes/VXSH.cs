﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AriaLibrary.Helpers;
using StringReader = AriaLibrary.IO.StringReader;

namespace AriaLibrary.Objects.GraphicsProgram.Nodes
{
    public class VertexShaderUniform
    {
        public string Name;
        public int ResourceIndex;
        public int U08;
        public int Size;
        public void Read(BinaryReader reader, int heapStringPosition)
        {
            Name = StringReader.ReadNullTerminatedStringAtOffset(reader, reader.ReadInt32() + heapStringPosition);
            ResourceIndex = reader.ReadInt32();
            U08 = reader.ReadInt32();
            Size = reader.ReadInt32();
        }

        public void Write(BinaryWriter dataWriter, BinaryWriter stringWriter, ref Dictionary<string, int> stringPosMap)
        {
            if (stringPosMap.TryGetValue(Name, out int value))
                dataWriter.Write(value);
            else
            {
                dataWriter.Write((int)stringWriter.BaseStream.Position);
                stringPosMap.Add(Name, (int)stringWriter.BaseStream.Position);
                stringWriter.Write(Name.ToCharArray());
                stringWriter.Write('\0');
            }
            dataWriter.Write(ResourceIndex);
            dataWriter.Write(U08);
            dataWriter.Write(Size);
        }

        public VertexShaderUniform(string name="")
        {
            Name = name;
        }
    }

    public class VXSHData
    {
        public int U00;
        public int U04;
        public int U08;
        public int U0C;
        public int U10;
        public int U14;
        public List<VertexShaderUniform> Uniforms;
        public int U1C;

        public void Read(BinaryReader reader, int dataPosition, int heapStringPosition, string platform)
        {
            long cur = reader.BaseStream.Position;
            reader.BaseStream.Seek(dataPosition, SeekOrigin.Begin);
            U00 = reader.ReadInt32();
            U04 = reader.ReadInt32();
            if (platform == "DX11")
            {
                U08 = reader.ReadInt32();
                U0C = reader.ReadInt32();
                U10 = reader.ReadInt32();
                U14 = reader.ReadInt32();
            }
            int uniformCount = reader.ReadInt32();
            U1C = reader.ReadInt32();
            for (int i = 0; i < uniformCount; i++)
            {
                VertexShaderUniform uniform = new VertexShaderUniform();
                uniform.Read(reader, heapStringPosition);
                Uniforms.Add(uniform);
            }
            reader.BaseStream.Seek(cur, SeekOrigin.Begin);
        }

        public void Write(BinaryWriter dataWriter, BinaryWriter stringWriter, ref Dictionary<string, int> stringPosMap)
        {
            dataWriter.Write(U00);
            dataWriter.Write(U04);
            dataWriter.Write(Uniforms.Count);
            dataWriter.Write(U0C);
            foreach (var uniform in Uniforms)
            {
                uniform.Write(dataWriter, stringWriter, ref stringPosMap);
            }
            PositionHelper.AlignWriter(dataWriter, 0x10);
        }

        public VXSHData()
        {
            Uniforms = new List<VertexShaderUniform>();
        }
    }

    public class VXSH : GPRSection
    {
        public override string Type => "VXSH";
        public VXSHData Data;
        public override void Read(BinaryReader reader, int heapStringOffset, int heapDataOffset, int heapVSBufferOffset, int heapMeshBufferOffset, int heapPSBufferOffset, string platform)
        {
            int nameOffset = reader.ReadInt32();
            Name = StringReader.ReadNullTerminatedStringAtOffset(reader, heapStringOffset + nameOffset);
            ReservedNameHash = reader.ReadInt32();
            int dataOffset = reader.ReadInt32();
            int dataSize = reader.ReadInt32();
            // Shader program
            int bufferOffset = reader.ReadInt32();
            int bufferSize = reader.ReadInt32();
            Buffer = (BufferName)reader.ReadInt32();
            int cur = (int)reader.BaseStream.Position;
            // Data
            Data.Read(reader, heapDataOffset + dataOffset, heapStringOffset, platform);
            if (bufferOffset != -1)
            {
                switch (Buffer)
                {
                    case BufferName.Mesh:
                        reader.BaseStream.Seek(heapMeshBufferOffset + bufferOffset, SeekOrigin.Begin);
                        BufferData = reader.ReadBytes(bufferSize);
                        break;
                    case BufferName.VertexShader:
                        reader.BaseStream.Seek(heapVSBufferOffset + bufferOffset, SeekOrigin.Begin);
                        BufferData = reader.ReadBytes(bufferSize);
                        break;
                    case BufferName.PixelShader:
                        reader.BaseStream.Seek(heapPSBufferOffset + bufferOffset, SeekOrigin.Begin);
                        BufferData = reader.ReadBytes(bufferSize);
                        break;
                    default:
                        throw new InvalidDataException($"Invalid buffer name in VXSH: {(int)Buffer}");
                }
            }
            reader.BaseStream.Seek(cur, SeekOrigin.Begin);
        }

        public override void Write(BinaryWriter heapWriter, BinaryWriter stringWriter, BinaryWriter dataWriter, BinaryWriter bufferWriter, ref Dictionary<string, int> stringPosMap, ref List<int> sectionDataPositions, ref int curDataPositionIdx)
        {
            heapWriter.Write(new char[4] { 'V', 'X', 'S', 'H' });
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
            heapWriter.Write(0x10 + (0x10 * Data.Uniforms.Count));
            heapWriter.Write((int)bufferWriter.BaseStream.Position);
            heapWriter.Write(BufferData.Length);
            heapWriter.Write((int)Buffer);
            sectionDataPositions.Add((int)dataWriter.BaseStream.Position);
            // write Data
            Data.Write(dataWriter, stringWriter, ref stringPosMap);
            PositionHelper.AlignWriter(dataWriter, 0x10);
            // write buffer
            bufferWriter.Write(BufferData);
            PositionHelper.AlignWriter(bufferWriter, 0x10);
        }

        public VXSH() : base()
        {
            Name = "";
            Data = new VXSHData();
            Buffer = BufferName.VertexShader;
        }
    }
}
