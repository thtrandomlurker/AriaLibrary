using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Helpers
{
    public static class PositionHelper
    {
        public static int PadValue(int val, int alignment, bool ignoreCurrentAlignment = false)
        {
            if (val % alignment != 0 || ignoreCurrentAlignment)
                return val + (alignment - (val % alignment));
            return val;
        }
        public static void AlignWriter(BinaryWriter writer, int alignment, bool ignoreCurrentAlignment = false)
        {
            if (writer.BaseStream.Position % alignment != 0 || ignoreCurrentAlignment)
            {
                int len = alignment - ((int)writer.BaseStream.Position % alignment);
                writer.Write(new byte[len]);
            }
        }
    }
}
