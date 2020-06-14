using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities
{
    public static class ThrowUtils
    {
        public static void ThrowArgumentException(ArgumentError error)
        {
            throw new ArgumentUniformException(error);
        }

        #region ##### THROW #####

        public static void ThrowIf_NullArgument(params object[] objs)
        {
            if (objs == null)
                return;

            for (int i = 0; i < objs.Length; i++)
            {
                object o = objs[i];

                if (o == null)
                {
                    throw new ArgumentNullException($"Argument {i + 1} of {objs.Length} cannot have NULL value");
                }
            }
        }

        public static void ThrowIf_Null_InvalidLength<T>(int minLength, int maxLength, params IEnumerable<T>[] collections)
        {
            ThrowUtils.ThrowIf_Negative(minLength, maxLength);
            ThrowUtils.ThrowIf_IntervalInvalid(minLength, maxLength, true);

            if (collections == null)
                return;

            for (int i = 0; i < collections.Length; i++)
            {
                IEnumerable<T> c = collections[i];

                if (c == null)
                {
                    throw new ArgumentNullException($"Argument {i + 1} of {collections.Length} cannot have NULL value");
                }
                else
                {
                    int count = c.Count();
                    if (count > maxLength || count < minLength)
                    {
                        throw new ArgumentException($"Argument {i + 1} of {collections.Length} have invalid length: {count}");
                    }
                }
            }
        }

        public static void ThrowIf_True(bool b)
        {
            ThrowIf_True(b, "Unexpected TRUE condition");
        }
        public static void ThrowIf_False(bool b)
        {
            ThrowIf_False(b, "Unexpected FALSE condition");
        }

        public static void ThrowIf_True(bool b, string message)
        {
            if (b)
            {
                throw new ArgumentException("Argument cannot have TRUE value: " + message);
            }
        }
        public static void ThrowIf_False(bool b, string message)
        {
            if (!b)
            {
                throw new ArgumentException("Argument cannot have FALSE value: " + message);
            }
        }

        public static void ThrowIf_ContainsDublicate(IEnumerable<object> objs)
        {
            if (objs.Distinct().Count() != objs.Count())
                throw new ArgumentException("Argument cannot have duplicate values");
        }

        public static void ThrowIf_ContainsNull(IEnumerable<object> objs)
        {
            if (objs.Contains(null))
                throw new ArgumentException("Argument cannot have null values");
        }

        public static void ThrowIf_ZeroLength(string argName, params object[] objs)
        {
            if (objs.Count() == 0)
                throw new ArgumentException("Argument \"{0}\" cannot have zero length".Format(argName as object));
        }

        public static void ThrowIf_ZeroLength(params object[] objs)
        {
            if (objs.Count() == 0)
                throw new ArgumentException("Argument cannot have zero length");
        }

        public static void ThrowIf_ZeroLength(IEnumerable<object> objs)
        {
            if (objs.Count() == 0)
                throw new ArgumentException("Argument cannot have zero length");
        }

        public static void ThrowIf_Null_ZeroLen(params object[] objs)
        {
            ThrowIf_NullArgument(objs);
            ThrowIf_ZeroLength(objs);
        }

        public static void ThrowIf_Null_ZeroLen_CNull(IEnumerable<object> objs)
        {
            ThrowIf_NullArgument(objs);
            ThrowIf_ZeroLength(objs);
            ThrowIf_ContainsNull(objs);
        }

        public static void ThrowIf_Null_ZeroLen_CNull_CDublicate(IEnumerable<object> objs)
        {
            ThrowIf_Null_ZeroLen_CNull(objs);
            ThrowIf_ContainsDublicate(objs);
        }

        public static void ThrowIf_FileNotExist(params string[] paths)
        {
            foreach (string path in paths)
                if (!File.Exists(path))
                    throw new FileNotFoundException("File \"{0}\" not found".Format(path as object));
        }

        #endregion

        #region ##### NUMERIC THROW #####

        public static void ThrowIf_InvalidNumber(double number, string numberName = "UNKNOWN")
        {
            bool invalid = double.IsInfinity(number) || double.IsNaN(number);
            string msg = "Number has infinite or NaN value. Name: \"{0}\"".Format(numberName as object);
            ThrowIf_True(invalid, msg);
        }

        public static void ThrowIf_IntervalInvalid(double from, double to, bool zeroLengthAllowed)
        {
            if (from > to)
            {
                throw new ArgumentOutOfRangeException("Interval cannot be from {0} to {1} because first number less than second.".Format(from, to));
            }
            else if ((from == to) && !zeroLengthAllowed)
            {
                throw new ArgumentOutOfRangeException("Interval cannot be from {0} to {1} because its length equal zero.".Format(from, to));
            }
        }

        public static void ThrowIf_IntervalNotContains(double from, double to, double value, bool strictFrom, bool strictTo)
        {
            if (value > to || (value == to && strictTo) || value < from || (value == from && strictFrom))
            {
                throw new ArgumentOutOfRangeException("Interval does not contains {0}".Format(value));
            }
        }

        public static void ThrowIf_OutOfRange(double value, double from, double to, bool strictFrom = false, bool strictTo = false)
        {
            if (value < from || (value == from && strictFrom) ||
                value > to || (value == to && strictTo))
            {
                throw new ArgumentOutOfRangeException("Value not in range: {0}<{1}{2}<{3}{4}"
                    .Format(from, strictFrom ? "" : "=", value, strictTo ? "" : "=", to));
            }
        }

        public static void ThrowIf_Zero(params double[] numbers)
        {
            for (int i = 0; i < numbers.Length; i++)
            {
                double n = numbers[i];

                if (n == 0)
                    throw new ArgumentOutOfRangeException("Argument number \"{0}\" cannot have a zero value".Format(i));
            }
        }

        public static void ThrowIf_Negative(params double[] numbers)
        {
            for (int i = 0; i < numbers.Length; i++)
            {
                double n = numbers[i];

                if (n < 0)
                {
                    throw new ArgumentOutOfRangeException($"Argument {i + 1} of {numbers.Length} cannot have negative value: {n}");
                }
            }
        }

        public static void ThrowIf_Negative_Zero(params double[] numbers)
        {
            ThrowIf_Negative(numbers);
            ThrowIf_Zero(numbers);
        }

        #endregion
    }
}
