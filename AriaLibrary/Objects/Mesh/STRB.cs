using AriaLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Objects.Mesh
{
    // String Buffer
    public class STRB : MeshBlock
    {
        public override string Type => "STRB";
        public int StringCount;
        public STRL StringList;

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            StringCount = reader.ReadInt32();
            // Skip the STRL magic
            reader.BaseStream.Seek(4, SeekOrigin.Current);
            StringList.Read(reader);
            // sanity check to sync the length of the StringList with the length in the STRB
            StringList.Strings = StringList.Strings.Take(StringCount).ToList();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'S', 'T', 'R', 'B' });
            writer.Write(12 + StringList.GetSize());
            writer.Write(StringCount);
            StringList.Write(writer);
        }

       public STRB()
        {
            StringList = new STRL();
        }
    }
}
