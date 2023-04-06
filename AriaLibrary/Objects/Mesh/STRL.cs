using AriaLibrary.Helpers;
using AriaLibrary.IO;
using StringReader = AriaLibrary.IO.StringReader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Objects.Mesh
{
    // String List
    public class STRL : MeshBlock
    {
        public override string Type => "STRL";
        public List<string> Strings;
        public int GetSize()
        {
            int s = 0;
            foreach (var str in Strings)
            {
                s += str.ToCharArray().Length;
            }
            s += Strings.Count;
            return PositionHelper.PadValue(s, 4);
        }

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            long start = reader.BaseStream.Position;
            while (reader.BaseStream.Position != start + dataSize)
            {
                Strings.Add(StringReader.ReadNullTerminatedString(reader));
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'S', 'T', 'R', 'L' });
            writer.Write(GetSize());
            foreach (var str in Strings)
            {
                writer.Write(str.ToCharArray());
                writer.Write((byte)0);
            }
            PositionHelper.AlignWriter(writer, 4);
        }

        public STRL()
        {
            Strings = new List<string>();
        }
    }
}
