using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.IO
{
    static class StringReader
    {
        public static string ReadNullTerminatedString(BinaryReader reader)
        {
            string ret = "";
            while (true)
            {
                char b = reader.ReadChar();
                if (b == '\0')
                {
                    return ret;
                }
                else
                {
                    ret += b;
                }
            }
        }
        public static string ReadNullTerminatedStringAtOffset(BinaryReader reader, int offset)
        {
            long cur = reader.BaseStream.Position;
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            string ret = "";
            while (true)
            {
                char b = reader.ReadChar();
                if (b == '\0')
                {
                    reader.BaseStream.Seek(cur, SeekOrigin.Begin);
                    return ret;
                }
                else
                {
                    ret += b;
                }
            }
        }
    }
}
