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
            char[] buf = new char[0];
            foreach (var str in Strings)
            {
                buf.Concat(str.ToCharArray());
                buf.Append('\0');
            }
            int size = PositionHelper.PadValue(buf.Length, 4);
            writer.Write(size);
            writer.Write(buf);
            PositionHelper.AlignWriter(writer, 4);
        }

        public STRL()
        {
            Strings = new List<string>();
        }
    }
}
