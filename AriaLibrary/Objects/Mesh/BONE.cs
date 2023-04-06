using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Mesh
{
    public class BONE : MeshBlock
    {
        public override string Type => "BONE";
        public int Name;
        public List<BOIF> BoneInformation;
        public List<IMTX> InverseMatrices;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            long basePos = reader.BaseStream.Position;
            Name = reader.ReadInt32();
            while (reader.BaseStream.Position < basePos + dataSize)
            {
                char[] magic = reader.ReadChars(4);
                switch (new string(magic))
                {
                    case "BOIF":
                        BOIF boif = new BOIF();
                        boif.Read(reader);
                        BoneInformation.Add(boif);
                        break;
                    case "IMTX":
                        IMTX imtx = new IMTX();
                        imtx.Read(reader);
                        InverseMatrices.Add(imtx);
                        break;
                    default:
                        throw new InvalidDataException($"Error parsing BONE section: Expected BOIF or IMTX, got {new string(magic)}");
                }
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'B', 'O', 'N', 'E' });
            writer.Write(0x04 + (0x10 * BoneInformation.Count()) + (0x38 * InverseMatrices.Count()));
            writer.Write(Name);
            foreach (BOIF boif in BoneInformation)
                boif.Write(writer);
            foreach (IMTX imtx in InverseMatrices)
                imtx.Write(writer);
        }

        public BONE()
        {
            BoneInformation = new List<BOIF>();
            InverseMatrices = new List<IMTX>();
        }
    }
}
