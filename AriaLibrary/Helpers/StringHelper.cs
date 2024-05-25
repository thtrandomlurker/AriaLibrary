using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriaLibrary.Helpers
{
    public static class StringHelper
    {
        public static uint GetStringHash(string str)
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
    }
}
