using AriaLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Objects.Nodes
{
    // String Buffer
    public class STRB : NodeBlock
    {
        public override string Type => "STRB";
        public int StringCount { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public STRL StringList { get; set; }

        public override void Read(BinaryReader reader)
        {
            int dataSize = reader.ReadInt32();
            StringCount = reader.ReadInt32();
            // Skip the STRL magic
            reader.BaseStream.Seek(4, SeekOrigin.Current);
            StringList.Read(reader);
            while (StringList.Strings.Count > StringCount)
                StringList.Strings.RemoveAt(StringCount);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(new char[4] { 'S', 'T', 'R', 'B' });
            writer.Write(12 + StringList.GetSize());
            writer.Write(StringList.Strings.Count);
            StringList.Write(writer);
        }

       public STRB()
        {
            StringList = new STRL();
        }
    }
}
