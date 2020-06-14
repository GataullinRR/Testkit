using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;

namespace Utilities.Extensions
{
    public static class StringEx
    {
        public static byte[] ToByteArray(this string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }


        public static bool IsNotNullOrWhiteSpace(this string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        public static string Capitalize(this string str)
        {
            return str.Length > 0
                ? char.ToUpper(str[0]) + str.Substring(1)
                : str;
        }

        public static string Repeat(this string str, int numOfTimes)
        {
            var sb = new StringBuilder(str.Length * numOfTimes);
            foreach (var i in numOfTimes.Range())
            {
                sb.Append(str);
            }

            return sb.ToString();
        }

        public static string[] Split(this string str, string separator, bool removeEmptyEntries = true)
        {
            return str
                .Split(new[] { separator }, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        /// <summary>
        /// => UTF8 bytes => Base64 string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToBase64(this string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        public static byte[] FromBase64(this string str)
        {
            return Convert.FromBase64String(str);
        }
        public static byte[] GetASCIIBytes(this IEnumerable<char> str)
        {
            return str.GetBytes(Encoding.ASCII);
        }
        public static byte[] GetUTF8Bytes(this IEnumerable<char> str)
        {
            return str.GetBytes(Encoding.UTF8);
        }
        public static byte[] GetBytes(this IEnumerable<char> str, Encoding encoding)
        {
            return str.Aggregate().GetBytes(encoding);
        }
        public static byte[] GetBytes(this string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        public static bool IsASCII(this string str)
        {
            return Encoding.UTF8.GetByteCount(str) == str.Length;
        }

        /// <summary>
        /// Разворачивает объекты реализующие IEnumerable
        /// </summary>
        /// <param name="format"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static string FlattenFormat(this string format, params object[] objs)
        {
            var flat = new List<object>();
            flattern(objs);

            return string.Format(format, flat.ToArray());

            void flattern(object obj)
            {
                if (obj is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        flattern(item);
                    }
                }
                else
                {
                    flat.Add(obj);
                }
            }
        }

        public static string Format(this string format, params object[] objs)
        {
            return string.Format(format, objs);
        }
        public static string Format(this string format, IFormatProvider provider, params object[] objs)
        {
            return string.Format(provider, format, objs);
        }

        public static StringBuilder AppendFormatLine(this StringBuilder sb, string format, params object[] objs)
        {
            return sb.AppendFormat(format + Environment.NewLine, objs);
        }

        public static string Between(this string str,
            char firstSign, char lastSign, bool includeSigns = false, bool greedy = true)
        {
            return str.Between(firstSign.ToString(), lastSign.ToString(), includeSigns, greedy);
        }
        public static string Between(this string str, 
            string firstSign, string lastSign, bool includeSigns = false, bool greedy = true)
        {
            int from = str.IndexOf(firstSign) + (includeSigns ? 0 : firstSign.Length);
            int to = -1;
            if (from != -1)
            {
                to = greedy
                    ? str.LastIndexOf(lastSign) + (includeSigns ? lastSign.Length : 0)
                    : str.IndexOf(lastSign, from) + (includeSigns ? lastSign.Length : 0);
            }
            if (from != -1 && to != -1)
            {
                int fixedFrom = from > to ? to : from;
                int fixedTo = from > to ? from : to;
                int count = fixedTo - fixedFrom;
                return str.Substring(fixedFrom, count);
            }
            else
            {
                return null;
            }
        }

        public static string RemoveFirst(this string str, string entity)
        {
            var findResult = str.Find(entity);
            if (findResult.Found)
            {
                return str.Remove(findResult.Index, entity.Length);
            }
            else
            {
                return str;
            }
        }
        public static string Remove(this string str, string entity)
        {
            return str.Replace(entity, "");
        }
        public static string Remove(this string str, params string[] entities)
        {
            foreach (var entity in entities)
            {
                str = str.Remove(entity);
            }

            return str;
        }
    }
}
