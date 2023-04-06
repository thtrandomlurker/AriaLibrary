using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AriaLibrary.Helpers;

namespace AriaLibrary.Objects.Nodes
{
    public class SSTV : NodeBlock
    {
        public override string Type => "SSTV";
        public int TextureSlot;
        public int TextureSourcePath;
        public int TextureName;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            TextureSlot = reader.ReadInt32();
            TextureSourcePath = reader.ReadInt32();
            TextureName = reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'S', 'S', 'T', 'V' });
            writer.Write(0x0C);
            writer.Write(TextureSlot);
            writer.Write(TextureSourcePath);
            writer.Write(TextureName);
        }

        public SSTV()
        {
        }
    }
}
