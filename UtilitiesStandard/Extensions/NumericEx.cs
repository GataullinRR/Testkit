using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Utilities.Types;

namespace Utilities.Extensions
{
    public class NumericExtensionsContextEnumerable<T> : EnumerableProxyBase<T>
    {
        public NumericExtensionsContextEnumerable(IEnumerable<T> @base) : base(@base)
        {

        }
    }

    public static class NumericEx
    {
        public static NumericExtensionsContextEnumerable<T> NumericExtension<T>(this IEnumerable<T> sequence)
        {
            return new NumericExtensionsContextEnumerable<T>(sequence);
        }

        #region ##### ToStringInvariant #####

        public static string ToStringInvariant(this sbyte number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this byte number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this short number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this ushort number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this int number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this uint number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this long number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this ulong number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this double number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this float number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this decimal number)
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToStringInvariant(this int number, string format)
        {
            return number.ToString(format, CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this double number, string format)
        {
            return number.ToString(format, CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this float number, string format)
        {
            return number.ToString(format, CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this decimal number, string format)
        {
            return number.ToString(format, CultureInfo.InvariantCulture);
        }

        #endregion

        #region ##### ToXXX #####

        public static char ToChar(this short value) => Convert.ToChar(value);
        public static char ToChar(this int value) => Convert.ToChar(value);

        public static byte ToByte(this decimal value) => Convert.ToByte(value);
        public static byte ToByte(this double value) => Convert.ToByte(value);
        public static byte ToByte(this float value) => Convert.ToByte(value);
        public static byte ToByte(this short value) => Convert.ToByte(value);
        public static byte ToByte(this char value) => Convert.ToByte(value);
        public static byte ToByte(this int value) => Convert.ToByte(value);

        public static short ToInt16(this decimal value) => Convert.ToInt16(value);
        public static short ToInt16(this double value) => Convert.ToInt16(value);
        public static short ToInt16(this float value) => Convert.ToInt16(value);
        public static short ToInt16(this short value) => Convert.ToInt16(value);
        public static short ToInt16(this char value) => Convert.ToInt16(value);
        public static short ToInt16(this byte value) => Convert.ToInt16(value);
        public static short ToInt16(this uint value) => Convert.ToInt16(value);
        public static short ToInt16(this int value) => Convert.ToInt16(value);

        public static int ToInt32(this decimal value) => Convert.ToInt32(value);
        public static int ToInt32(this double value) => Convert.ToInt32(value);
        public static int ToInt32(this float value) => Convert.ToInt32(value);
        public static int ToInt32(this short value) => Convert.ToInt32(value);
        public static int ToInt32(this char value) => Convert.ToInt32(value);
        public static int ToInt32(this byte value) => Convert.ToInt32(value);
        public static int ToInt32(this uint value) => Convert.ToInt32(value);
        public static int ToInt32(this long value) => Convert.ToInt32(value);

        public static uint ToUInt32(this decimal value) =>  Convert.ToUInt32(value);
        public static uint ToUInt32(this double value) =>  Convert.ToUInt32(value);
        public static uint ToUInt32(this float value) =>  Convert.ToUInt32(value);
        public static uint ToUInt32(this int value) =>  Convert.ToUInt32(value);
        public static uint ToUInt32(this short value) =>  Convert.ToUInt32(value);
        public static uint ToUInt32(this char value) =>  Convert.ToUInt32(value);
        public static uint ToUInt32(this byte value) =>  Convert.ToUInt32(value);
        public static uint ToUInt32(this int val, bool negativeToZero)
        {
            return negativeToZero ? (val < 0 ? 0 : (uint)val) : (uint)Math.Abs(val);
        }

        public static long ToInt64(this decimal value) => Convert.ToInt64(value);
        public static long ToInt64(this double value) => Convert.ToInt64(value);
        public static long ToInt64(this float value) => Convert.ToInt64(value);
        public static long ToInt64(this short value) => Convert.ToInt64(value);
        public static long ToInt64(this char value) => Convert.ToInt64(value);
        public static long ToInt64(this byte value) => Convert.ToInt64(value);
        public static long ToInt64(this uint value) => Convert.ToInt64(value);

        public static UInt16 ToUInt16(this decimal value) => Convert.ToUInt16(value);
        public static UInt16 ToUInt16(this double value) => Convert.ToUInt16(value);
        public static UInt16 ToUInt16(this float value) => Convert.ToUInt16(value);
        public static UInt16 ToUInt16(this int value) => Convert.ToUInt16(value);
        public static UInt16 ToUInt16(this char value) => Convert.ToUInt16(value);
        public static UInt16 ToUInt16(this byte value) => Convert.ToUInt16(value);
        public static UInt16 ToUInt16(this int val, bool negativeToZero)
        {
            return negativeToZero ? (val < 0 ? (UInt16)0 : (UInt16)val) : (UInt16)Math.Abs(val);
        }

        public static float ToSingle(this decimal value) => Convert.ToSingle(value);
        public static float ToSingle(this double value) => Convert.ToSingle(value);
        public static float ToSingle(this short value) => Convert.ToSingle(value);
        public static float ToSingle(this char value) => Convert.ToSingle(value);
        public static float ToSingle(this byte value) => Convert.ToSingle(value);

        public static double ToDouble(this decimal value) => Convert.ToDouble(value);
        public static double ToDouble(this float value) => Convert.ToDouble(value);
        public static double ToDouble(this short value) => Convert.ToDouble(value);
        public static double ToDouble(this char value) => Convert.ToDouble(value);
        public static double ToDouble(this byte value) => Convert.ToDouble(value);

        #endregion

        #region ##### IsXXX #####

        public static bool IsInteger(this double val)
        {
            return val.Round() == val;
        }
        public static bool IsInteger(this float value)
        {
            return value == value.Round();
        }

        public static bool IsNaN(this float val)
        {
            return float.IsNaN(val);
        }
        public static bool IsNaN(this double val)
        {
            return double.IsNaN(val);
        }

        public static bool IsNotNaN(this float val)
        {
            return !float.IsNaN(val);
        }
        public static bool IsNotNaN(this double val)
        {
            return !double.IsNaN(val);
        }

        public static bool IsPositive(this int value)
        {
            return value >= 0;
        }
        public static bool IsPositive(this long value)
        {
            return value >= 0;
        }

        public static bool IsNegative(this int value)
        {
            return value < 0;
        }
        public static bool IsNegative(this long value)
        {
            return value < 0;
        }

        public static int NegativeToZero(this int val)
        {
            return val < 0 ? 0 : val;
        }
        public static double NegativeToZero(this double val)
        {
            return val < 0 ? 0 : val;
        }
        public static long NegativeToZero(this long val)
        {
            return val < 0 ? 0 : val;
        }

        public static double PositiveToZero(this double val)
        {
            return val < 0 ? 0 : val;
        }
        public static int PositiveToZero(this int val)
        {
            return val > 0 ? 0 : val;
        }
        public static long PositiveToZero(this long val)
        {
            return val > 0 ? 0 : val;
        }

        public static bool IsRangeInvalid(this int value, int rangeFrom, int rangeTo,
            bool strictFrom = false, bool strictTo = false)
        {
            return !value.IsRangeValid(rangeFrom, rangeTo, strictFrom, strictTo);
        }
        public static bool IsRangeValid(this int value, int rangeFrom, int rangeTo,
            bool strictFrom = false, bool strictTo = false)
        {
            bool isFromValid = value > rangeFrom || (value >= rangeFrom && !strictFrom);
            bool isToValid = value < rangeTo || (value <= rangeTo && !strictTo);
            return isFromValid && isToValid;
        }
        public static bool IsRangeValid(this double value, double rangeFrom, double rangeTo,
            bool strictFrom = false, bool strictTo = false)
        {
            bool isFromValid = value > rangeFrom || (value >= rangeFrom && !strictFrom);
            bool isToValid = value < rangeTo || (value <= rangeTo && !strictTo);
            return isFromValid && isToValid;
        }

        #endregion

        #region ##### Math opperations #####

        public static int FindClosestDivider(this int number, int desiredDivider)
        {
            if (number <= 0)
            {
                throw new ArgumentException();
            }

            return number
                .FindDividers()
                .OrderBy(d => (d - desiredDivider).Abs())
                .FirstItem();
        }
        public static IEnumerable<int> FindDividers(this int number)
        {
            if (number <= 0)
            {
                throw new ArgumentException();
            }

            for (int i = 1; i <= number; i++)
            {
                if (number % i == 0)
                {
                    yield return i;
                }
            }
        }

        public static double Round(this double val, int decimals) => Math.Round(val, decimals);
        public static float Round(this float val, int decimals) => (float)Math.Round(val, decimals);
        public static int Round(this double val) => (int)Math.Round(val);
        public static int Round(this float val) => (int)Math.Round(val);

        /// <summary>
        /// Converts 0.1 to 1
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int Ceiling(this double val) => (int)Math.Ceiling(val);
        public static int Ceiling(this float val) => (int)Math.Ceiling(val);

        /// <summary>
        /// Converts 0.9 to 0
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int Floor(this double val) => (int)Math.Floor(val);
        public static int Floor(this float val) => (int)Math.Floor(val);

        public static double Pow(this double val, double power)
        {
            return Math.Pow(val, power);
        }
        public static float Pow(this float val, double power)
        {
            return Math.Pow(val, power).ToSingle();
        }
        public static int Pow(this int val, double power)
        {
            return Math.Pow(val, power).ToInt32();
        }
        public static long Pow(this long val, double power)
        {
            return Math.Pow(val, power).ToInt64();
        }
        public static double Root(this double val, double power)
        {
            return Math.Pow(val, 1 / power);
        }
        public static float Root(this float val, double power)
        {
            return Math.Pow(val, 1 / power).ToSingle();
        }
        public static int Root(this int val, double power)
        {
            return Math.Pow(val, 1 / power).ToInt32();
        }

        public static int Sign(this double val)
        {
            return Math.Sign(val);
        }
        public static int Sign(this float val)
        {
            return Math.Sign(val);
        }
        public static int Sign(this int val)
        {
            return Math.Sign(val);
        }

        public static double Abs(this double val)
        {
            return Math.Abs(val);
        }
        public static float Abs(this float val)
        {
            return Math.Abs(val);
        }
        public static int Abs(this int val)
        {
            return Math.Abs(val);
        }

        #endregion

        #region ##### Basic math opperations #####

        public static byte Add(this byte val1, byte val2) => (byte)(val1 + val2);
        public static int Add(this int val1, int val2) => val1 + val2;
        public static double Add(this double val1, double val2) => val1 + val2;

        public static byte Sub(this byte val1, byte val2) => (byte)(val1 - val2);
        public static int Sub(this int val1, int val2) => val1 - val2;
        public static double Sub(this double val1, double val2) => val1 - val2;

        public static int Mul(this int val1, int val2) => val1 * val2;
        public static float Mul(this float val1, float val2) => val1 * val2;
        public static double Mul(this double val1, double val2) => val1 * val2;

        public static int Div(this int val1, int val2) => val1 / val2;
        public static float Div(this float val1, float val2) => val1 / val2;
        public static double Div(this double val1, double val2) => val1 / val2;

        #endregion

        #region ##### BitXXX #####

        static readonly byte[] _bitReverseTable =
        {
            0x00, 0x80, 0x40, 0xc0, 0x20, 0xa0, 0x60, 0xe0,
            0x10, 0x90, 0x50, 0xd0, 0x30, 0xb0, 0x70, 0xf0,
            0x08, 0x88, 0x48, 0xc8, 0x28, 0xa8, 0x68, 0xe8,
            0x18, 0x98, 0x58, 0xd8, 0x38, 0xb8, 0x78, 0xf8,
            0x04, 0x84, 0x44, 0xc4, 0x24, 0xa4, 0x64, 0xe4,
            0x14, 0x94, 0x54, 0xd4, 0x34, 0xb4, 0x74, 0xf4,
            0x0c, 0x8c, 0x4c, 0xcc, 0x2c, 0xac, 0x6c, 0xec,
            0x1c, 0x9c, 0x5c, 0xdc, 0x3c, 0xbc, 0x7c, 0xfc,
            0x02, 0x82, 0x42, 0xc2, 0x22, 0xa2, 0x62, 0xe2,
            0x12, 0x92, 0x52, 0xd2, 0x32, 0xb2, 0x72, 0xf2,
            0x0a, 0x8a, 0x4a, 0xca, 0x2a, 0xaa, 0x6a, 0xea,
            0x1a, 0x9a, 0x5a, 0xda, 0x3a, 0xba, 0x7a, 0xfa,
            0x06, 0x86, 0x46, 0xc6, 0x26, 0xa6, 0x66, 0xe6,
            0x16, 0x96, 0x56, 0xd6, 0x36, 0xb6, 0x76, 0xf6,
            0x0e, 0x8e, 0x4e, 0xce, 0x2e, 0xae, 0x6e, 0xee,
            0x1e, 0x9e, 0x5e, 0xde, 0x3e, 0xbe, 0x7e, 0xfe,
            0x01, 0x81, 0x41, 0xc1, 0x21, 0xa1, 0x61, 0xe1,
            0x11, 0x91, 0x51, 0xd1, 0x31, 0xb1, 0x71, 0xf1,
            0x09, 0x89, 0x49, 0xc9, 0x29, 0xa9, 0x69, 0xe9,
            0x19, 0x99, 0x59, 0xd9, 0x39, 0xb9, 0x79, 0xf9,
            0x05, 0x85, 0x45, 0xc5, 0x25, 0xa5, 0x65, 0xe5,
            0x15, 0x95, 0x55, 0xd5, 0x35, 0xb5, 0x75, 0xf5,
            0x0d, 0x8d, 0x4d, 0xcd, 0x2d, 0xad, 0x6d, 0xed,
            0x1d, 0x9d, 0x5d, 0xdd, 0x3d, 0xbd, 0x7d, 0xfd,
            0x03, 0x83, 0x43, 0xc3, 0x23, 0xa3, 0x63, 0xe3,
            0x13, 0x93, 0x53, 0xd3, 0x33, 0xb3, 0x73, 0xf3,
            0x0b, 0x8b, 0x4b, 0xcb, 0x2b, 0xab, 0x6b, 0xeb,
            0x1b, 0x9b, 0x5b, 0xdb, 0x3b, 0xbb, 0x7b, 0xfb,
            0x07, 0x87, 0x47, 0xc7, 0x27, 0xa7, 0x67, 0xe7,
            0x17, 0x97, 0x57, 0xd7, 0x37, 0xb7, 0x77, 0xf7,
            0x0f, 0x8f, 0x4f, 0xcf, 0x2f, 0xaf, 0x6f, 0xef,
            0x1f, 0x9f, 0x5f, 0xdf, 0x3f, 0xbf, 0x7f, 0xff
        };
        public static byte BitReverse(this byte current)
        {
            return _bitReverseTable[current];
        }

        public static byte BitLShift(this byte current, int shift)
        {
            return (byte)(current << shift);
        }
        public static byte BitRShift(this byte current, int shift)
        {
            return (byte)(current >> shift);
        }
        public static byte BitXOR(this byte current, byte mask)
        {
            return (byte)(current ^ mask);
        }
        public static byte BitAND(this byte current, byte mask)
        {
            return (byte)(current & mask);
        }
        public static byte BitOR(this byte current, byte mask)
        {
            return (byte)(current | mask);
        }
        public static byte BitNOT(this byte current)
        {
            return (byte)(~current);
        }

        public static IEnumerable<bool> Bits(this int current, bool fromMSB = true)
        {
            if (!fromMSB)
            {
                for (int i = 0; i < 32; i++)
                {
                    yield return (current & (1 << i)) != 0;
                }
            }
            else
            {
                for (int i = 31; i >= 0; i--)
                {
                    yield return (current & (1 << i)) != 0;
                }
            }
        }

        #endregion

        public static bool IsAlmostEqual(this double current, double value, double epsilon)
        {
            return current < (value + epsilon) && current > (value - epsilon);
        }

        public static T Exchange<T>(this T current, T exchangingValue, T newValue)
        {
            return current.Equals(exchangingValue) ? newValue : current;
        }

        public static float ApplyRange(this float value, float from, float to)
        {
            return (float)((double)value).ApplyRange(from, to);
        }
        public static int ApplyRange(this int value, int from, int to)
        {
            return (int)((double)value).ApplyRange(from, to);
        }
        public static double ApplyRange(this double value, double from, double to)
        {
            ThrowUtils.ThrowIf_IntervalInvalid(from, to, true);

            if (value < from)
            {
                return from;
            }
            else if (value > to)
            {
                return to;
            }
            else
            {
                return value;
            }
        }

#warning code repetition
        public static byte[] SerializeAsNumber(this ushort value, bool msbFirst)
        {
            var result = BitConverter.GetBytes(value);
            var shouldReverse = BitConverter.IsLittleEndian && !msbFirst;
            if (shouldReverse)
            {
                result = result.Reverse().ToArray();
            }

            return result;
        }       
        public static byte[] SerializeAsNumber(this int value, bool msbFirst)
        {
            var result = BitConverter.GetBytes(value);
            var shouldReverse = BitConverter.IsLittleEndian && !msbFirst;
            if (shouldReverse)
            {
                result = result.Reverse().ToArray();
            }

            return result;
        }

        public static ushort DeserializeUInt16(this IEnumerable<byte> serialized, bool msbFirst)
        {
            var shouldReverse = BitConverter.IsLittleEndian && msbFirst;
            serialized = serialized.Take(2);
            if (shouldReverse)
            {
                serialized = serialized.Reverse();
            }
         
            return BitConverter.ToUInt16(serialized.ToArray(), 0);
        }
        public static int DeserializeInt32(this IEnumerable<byte> serialized, bool msbFirst)
        {
            var shouldReverse = BitConverter.IsLittleEndian && msbFirst;
            serialized = serialized.Take(4);
            if (shouldReverse)
            {
                serialized = serialized.Reverse();
            }

            return BitConverter.ToInt32(serialized.ToArray(), 0);
        }
    }
}