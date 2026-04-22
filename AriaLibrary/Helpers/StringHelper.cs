using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Helpers
{
    public static class StringHelper
    {
        private static readonly uint[] mCrc32Lookup = Enumerable.Range(0, 256).Select(i =>
        {
            uint crc = (uint)i;
            for (int j = 0; j < 8; j++)
            {
                crc = (crc >> 1) ^ (0xEDB88320 & ~((crc & 1) - 1));
            }
            return crc;
        }).ToArray();

        public static uint GetBRNTStringHash(string str)
        {
            uint seed;
            uint calc;
            uint remain_positive;
            uint result;

            seed = 0x38abe8f9;
            result = 0x12a3fe2d;
            foreach (var chr in str)
            {
                remain_positive = 0;
                calc = seed + ((byte)chr * (uint)0x11763 ^ result);
                if ((calc & 0x80000000) != 0)
                {
                    remain_positive = 0x7fffffff;
                }
                seed = result;
                result = calc - remain_positive;
            }
            return result;
        }

        public static uint GetBindPoseStringHash(string str)
        {
            uint mask = 0xFFFFFFFF;
            for (int i = 0; i < str.Length; ++i)
            {
                mask = (mask >> 8) ^ mCrc32Lookup[(str[i] ^ mask) & 0xFF];
            }
            return mask;
        }
    }
}
