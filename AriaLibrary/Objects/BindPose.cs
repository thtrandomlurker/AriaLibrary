using AriaLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Objects
{
    public struct BoneRelation {
        public short ID;
        public short ParentID;
    }
    public struct BindInfo
    {
        public Vector4 Rotation;
        public Vector4 Translation;
        public Vector4 Scale;
    }
    public class BindPose
    {
        public BoneRelation[] BoneHierarchy;
        public BindInfo[] BoneBindInfos;
        public short[] BoneOrderList;
        public List<string> BoneNames;
        public uint[] BoneHashes;
        public void Read(BinaryReader reader)
        {
            string magic = new string(reader.ReadChars(4));
            if (magic != "60SE")
            {
                throw new InvalidDataException("Not valid 60SE file.");
            }

            int fileSize = reader.ReadInt32();
            int boneNameTableLength = reader.ReadInt32();
            int hashTableLength = reader.ReadInt32();
            int numBones = reader.ReadInt32();
            int unk_1 = reader.ReadInt32();
            int distToMatrices = reader.ReadInt32();
            int matrixOffset = (int)reader.BaseStream.Position - 4 + distToMatrices;
            int distToSkinBoneList = reader.ReadInt32();
            int skinBoneListOffset = (int)reader.BaseStream.Position - 4 + distToSkinBoneList;
            int distToNameHashTable = reader.ReadInt32();
            int nameHashTableOffset = (int)reader.BaseStream.Position - 4 + distToNameHashTable;
            int distToUnkData = reader.ReadInt32();
            int unkDataOffset = (int)reader.BaseStream.Position - 4 + distToUnkData;
            int distToUnkData2 = reader.ReadInt32();
            int unkData2Offset = (int)reader.BaseStream.Position - 4 + distToUnkData2;
            int distToUnkAfterParentData = reader.ReadInt32();
            int unkAfterParentDataOffset = (int)reader.BaseStream.Position - 4 + distToUnkAfterParentData;
            int distToBoneNameTable = reader.ReadInt32();
            int boneNameTableOffset = (int)reader.BaseStream.Position - 4 + distToBoneNameTable;

            reader.BaseStream.Seek(8, SeekOrigin.Current);
            int boneHierarchyLength = reader.ReadInt32();

            BoneHierarchy = new BoneRelation[boneHierarchyLength];
            for (int i = 0; i < boneHierarchyLength; i++)
            {
                BoneHierarchy[i] = new BoneRelation
                {
                    ID = reader.ReadInt16(),
                    ParentID = reader.ReadInt16()
                };
            }

            reader.BaseStream.Seek(matrixOffset, SeekOrigin.Begin);

            BoneBindInfos = new BindInfo[numBones];

            for (int i = 0; i < numBones; i++)
            {
                BoneBindInfos[i] = new BindInfo
                {
                    Rotation = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    Translation = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                    Scale = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
                };
            }

            reader.BaseStream.Seek(skinBoneListOffset, SeekOrigin.Begin);
            BoneOrderList = new short[numBones];
            for (int i = 0; i < numBones; i++)
            {
                BoneOrderList[i] = reader.ReadInt16();
            }

            reader.BaseStream.Seek(boneNameTableOffset, SeekOrigin.Begin);

            int nameTableUnk1 = reader.ReadInt32(); // 1
            int nameTableUnk2 = reader.ReadInt32(); // C
            int nameTableUnk3 = reader.ReadInt32(); // C
            int nameTableUnk4 = reader.ReadInt32(); // C
            int nameTableUnk5 = reader.ReadInt32(); // 0x67791C33
            int nameTableSize = reader.ReadInt32();
            int nameOffsetSize = reader.ReadInt32();

            for (int i = 0; i < numBones; i++)
            {
                int c = (int)reader.BaseStream.Position;
                int distToName;
                switch (nameOffsetSize)
                {
                    case 1:
                        distToName = reader.ReadByte();
                        break;
                    case 2:
                        distToName = reader.ReadInt16();
                        break;
                    case 4:
                        distToName = reader.ReadInt32();
                        break;
                    case 8:
                        distToName = (int)reader.ReadInt64();
                        break;
                    default:
                        throw new InvalidDataException("Invalid name offset size in name table.");
                }
                int nameOffset = c + distToName;
                long currentPos = reader.BaseStream.Position;
                reader.BaseStream.Seek(nameOffset, SeekOrigin.Begin);
                string name = AriaLibrary.IO.StringReader.ReadNullTerminatedString(reader);
                reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);
                BoneNames.Add(name);
            }

            reader.BaseStream.Seek(nameHashTableOffset, SeekOrigin.Begin);
            BoneHashes = new uint[numBones];
            for (int i = 0; i < numBones; i++)
            {
                uint nameHash = reader.ReadUInt32();
                BoneHashes[i] = nameHash;
            }
        }
        public void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { '6', '0', 'S', 'E' });
            writer.Write(0);
            writer.Write(PositionHelper.PadValue((0x1C + (0x4 * BoneNames.Count) + (BoneNames.Sum(x => x.Length + 1))), 16));
            writer.Write(PositionHelper.PadValue(BoneHashes.Length * 4, 16));
            writer.Write(BoneNames.Count);
            writer.Write(1);
            int matrixPosition = PositionHelper.PadValue((int)writer.BaseStream.Position + 0x28 + (4 * BoneHierarchy.Length), 16);
            writer.Write(matrixPosition - (int)writer.BaseStream.Position);
            int orderPosition = matrixPosition + (0x30 * BoneNames.Count);
            writer.Write(orderPosition - (int)writer.BaseStream.Position);
            int nameHashTablePosition = PositionHelper.PadValue(orderPosition + PositionHelper.PadValue((BoneOrderList.Length * 2), 16) + (0x1C + (0x4 * BoneNames.Count) + (BoneNames.Sum(x => x.Length + 1))), 16);
            writer.Write(nameHashTablePosition - (int)writer.BaseStream.Position);
            writer.Write(nameHashTablePosition + (4 * BoneHashes.Length) - (int)writer.BaseStream.Position);
            writer.Write(nameHashTablePosition + (4 * BoneHashes.Length) - (int)writer.BaseStream.Position);
            writer.Write(orderPosition + PositionHelper.PadValue((BoneOrderList.Length * 2), 4) - (int)writer.BaseStream.Position);
            writer.Write(orderPosition + PositionHelper.PadValue((BoneOrderList.Length * 2), 16) - (int)writer.BaseStream.Position);
            writer.Write(0);
            writer.Write(0);
            writer.Write(BoneHierarchy.Length);
            foreach (var ent in BoneHierarchy)
            {
                writer.Write(ent.ID);
                writer.Write(ent.ParentID);
            }
            while (writer.BaseStream.Position < matrixPosition)
            {
                writer.Write((byte)0);
            }
            foreach (var mat in BoneBindInfos)
            {
                writer.Write(mat.Rotation.X);
                writer.Write(mat.Rotation.Y);
                writer.Write(mat.Rotation.Z);
                writer.Write(mat.Rotation.W);
                writer.Write(mat.Translation.X);
                writer.Write(mat.Translation.Y);
                writer.Write(mat.Translation.Z);
                writer.Write(mat.Translation.W);
                writer.Write(mat.Scale.X);
                writer.Write(mat.Scale.Y);
                writer.Write(mat.Scale.Z);
                writer.Write(mat.Scale.W);
            }
            while (writer.BaseStream.Position < orderPosition)
            {
                writer.Write((byte)0);
            }
            foreach (var boneID in BoneOrderList)
            {
                writer.Write(boneID);
            }
            while (writer.BaseStream.Position % 16 != 0)
            {
                writer.Write((byte)0);
            }
            // name table
            writer.Write(1);
            writer.Write(12);
            writer.Write(12);
            writer.Write(12);
            writer.Write(0x67791C33);
            writer.Write(PositionHelper.PadValue((0x4 * BoneNames.Count) + (BoneNames.Sum(x => x.Length + 1)), 4));
            writer.Write(4);
            int stringBufferPos = (int)writer.BaseStream.Position + (4 * BoneNames.Count);
            for (int i = 0; i < BoneNames.Count; i++)
            {
                int distToName = stringBufferPos - (int)writer.BaseStream.Position;
                writer.Write(distToName);
                stringBufferPos += BoneNames[i].Length + 1;
            }
            foreach (var name in BoneNames)
            {
                writer.Write(Encoding.ASCII.GetBytes(name));
                writer.Write((byte)0);
            }
            while (writer.BaseStream.Position % 16 != 0)
            {
                writer.Write((byte)0);
            }
            foreach (var hash in BoneHashes)
            {
                writer.Write(hash);
            }
            PositionHelper.AlignWriter(writer, 16);
            int size = (int)writer.BaseStream.Position;
            writer.Seek(4, SeekOrigin.Begin);
            writer.Write(size);
        }

        public BindPose()
        {
            BoneHierarchy = Array.Empty<BoneRelation>();
            BoneBindInfos = Array.Empty<BindInfo>();
            BoneOrderList = Array.Empty<short>();
            BoneNames = new List<string>();
            BoneHashes = Array.Empty<uint>();
        }
    }
}
