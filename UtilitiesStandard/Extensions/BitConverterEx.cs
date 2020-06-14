using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utilities.Extensions
{
    public static class BitConverterEx
    {
        public static UInt16 ToUInt16(this IEnumerable<byte> uint16Bytes, bool reverseBytes)
        {
            return BitConverter.ToUInt16(uint16Bytes.reverse(reverseBytes), 0);
        }
        public static UInt32 ToUInt32(this IEnumerable<byte> uint32Bytes, bool reverseBytes)
        {
            return BitConverter.ToUInt32(uint32Bytes.reverse(reverseBytes), 0);
        }
        public static Int16 ToInt16(this IEnumerable<byte> int16Bytes, bool reverseBytes)
        {
            return BitConverter.ToInt16(int16Bytes.reverse(reverseBytes), 0);
        }
        public static Int32 ToInt32(this IEnumerable<byte> int32Bytes, bool reverseBytes)
        {
            return BitConverter.ToInt32(int32Bytes.reverse(reverseBytes), 0);
        }

        static byte[] reverse(this IEnumerable<byte> bytes, bool reverseBytes)
        {
            return bytes.Reverse().ToArray();
        }
    }
}
