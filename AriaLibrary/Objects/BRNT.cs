using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using AriaLibrary.Helpers;

public class Bone
{
    public uint BoneNameHash;
    public string BoneName;
    public short BoneID;
    public short BoneParent;
    public short U18;
    public short SkinID;
    public short ChildID;
    public short U1E;
    public short PossibleFlags;
    public short U22;
    public short U24;
    public short U26;
    public Vector3 Translation;
    public Vector3 Rotation;
    public Vector3 Scale;
    public int U4C;
    public int U50;
    public int U54;

    public void Read(BinaryReader reader)
    {
        BoneNameHash = reader.ReadUInt32();
        BoneName = new string(reader.ReadChars(16)).Split("\0")[0];
        BoneID = reader.ReadInt16();
        BoneParent = reader.ReadInt16();
        U18 = reader.ReadInt16();
        SkinID = reader.ReadInt16();
        ChildID = reader.ReadInt16();
        U1E = reader.ReadInt16();
        PossibleFlags = reader.ReadInt16();
        U22 = reader.ReadInt16();
        U24 = reader.ReadInt16();
        U26 = reader.ReadInt16();
        Translation = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        Rotation = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        Scale = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        U4C = reader.ReadInt32();
        U50 = reader.ReadInt32();
        U54 = reader.ReadInt32();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(BoneNameHash);
        writer.Write(BoneName.ToCharArray());
        writer.Write(new char[16 - BoneName.Length]);
        writer.Write(BoneID);
        writer.Write(BoneParent);
        writer.Write(U18);
        writer.Write(SkinID);
        writer.Write(ChildID);
        writer.Write(U1E);
        writer.Write(PossibleFlags);
        writer.Write(U22);
        writer.Write(U24);
        writer.Write(U26);
        writer.Write(Translation.X);
        writer.Write(Translation.Y);
        writer.Write(Translation.Z);
        writer.Write(Rotation.X);
        writer.Write(Rotation.Y);
        writer.Write(Rotation.Z);
        writer.Write(Scale.X);
        writer.Write(Scale.Y);
        writer.Write(Scale.Z);
        writer.Write(U4C);
        writer.Write(U50);
        writer.Write(U54);
    }
}

namespace AriaLibrary.Objects
{
    public class BRNT
    {
        public List<Bone> Bones;
        public int NumRiggedBones => Bones.Where(x => x.SkinID != -1).Count();

        public void Read(BinaryReader reader)
        {
            string magic = new string(reader.ReadChars(16));
            if (magic != "BRNTREx86Ver2.00")
                throw new InvalidDataException();
            int numBones = reader.ReadInt32();
            int numRiggedBones = reader.ReadInt32();
            Console.WriteLine(numRiggedBones);
            reader.BaseStream.Seek(0x20, SeekOrigin.Begin);
            for (int i = 0; i < numBones; i++)
            {
                Bone bone = new Bone();
                bone.Read(reader);
                Bones.Add(bone);
            }
            Console.WriteLine(NumRiggedBones);
            Console.WriteLine(numRiggedBones == NumRiggedBones ? "I was right." : "I was wrong");
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write("BRNTREx86Ver2.00".ToCharArray());
            writer.Write(Bones.Count);
            writer.Write(NumRiggedBones);
            PositionHelper.AlignWriter(writer, 0x10);
            foreach (var bone in Bones)
            {
                bone.Write(writer);
            }
        }

        public void Load(string filePath)
        {
            Stream file = File.OpenRead(filePath);
            Load(file);
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
            Save(file);
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

        public BRNT()
        {
            Bones = new List<Bone>();
        }
    }
}
