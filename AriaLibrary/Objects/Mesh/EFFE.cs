using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Mesh
{
    public class EFFE : MeshBlock
    {
        public override string Type => "EFFE";
        public int EffectID;
        public int EffectName;
        public int EffectFileName;
        public int EffectType;
        public TPAS TPAS;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            EffectID = reader.ReadInt32();
            EffectName = reader.ReadInt32();
            EffectFileName = reader.ReadInt32();
            EffectType = reader.ReadInt32();
            // Skip TPAS magic
            reader.BaseStream.Seek(4, SeekOrigin.Current);
            TPAS.Read(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'E', 'F', 'F', 'E' });
            writer.Write(0x30);
            writer.Write(EffectID);
            writer.Write(EffectName);
            writer.Write(EffectFileName);
            writer.Write(EffectType);
            TPAS.Write(writer);
        }

        public EFFE()
        {
            TPAS = new TPAS();
        }
    }
}
