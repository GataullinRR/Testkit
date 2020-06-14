using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using System.Collections;

namespace Utilities
{
    public static class ConvertUtils
    {
        public static BitArray CharsToBitArray(IEnumerable<char> binaryValues)
        {
            if (!binaryValues.All(c => " 01".Contains(c)))
            {
                throw new ArgumentException();
            }

            var bits = binaryValues
                .Where(c => "01".Contains(c))
                .Select(c => c == '1')
                .ToArray();

            return new BitArray(bits);
        }

        public static byte ParseHex(char hexChar)
        {
            hexChar = char.ToUpper(hexChar);

            int result = (int)hexChar < (int)'A' ?
                ((int)hexChar - (int)'0') :
                10 + ((int)hexChar - (int)'A');
            return result.ToByte();
        }
    }
}
