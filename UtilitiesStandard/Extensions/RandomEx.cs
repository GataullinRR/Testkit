using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Extensions
{
    public static class RandomEx
    {
        static readonly string[] ENWords = Utilities.Properties.Resources.ENWords5000
            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        static readonly string[] RUWords = Utilities.Properties.Resources.RUWords5000
            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        public static byte[] NextBytes(this Random rnd, int count)
        {
            var bytes = new byte[count];
            rnd.NextBytes(bytes);

            return bytes;
        }

        public static double[] NextDoubles(this Random rnd, int count, double from, double to)
        {
            var doubles = new double[count];
            for (int i = 0; i < count; i++)
            {
                doubles[i] = rnd.NextDouble(from, to);
            }

            return doubles;
        }

        public static T NextObjFrom<T>(this Random rnd, params T[] objects)
        {
            var i = rnd.Next(objects.Length);
            return objects[i];
        }

        public static bool NextBool(this Random rnd)
        {
            return rnd.NextDouble() > 0.5;
        }
        public static long NextLong(this Random rnd, long to)
        {
            return (rnd.NextDouble() * to).ToInt64();
        }
        public static long NextLong(this Random rnd)
        {
            var longBytes = rnd.NextBytes(sizeof(long));
         
            return BitConverter.ToInt64(longBytes, 0);
        }

        public static DateTimeOffset NextDateTime(this Random rnd, DateTimeOffset from, DateTimeOffset to)
        {
            if (to <= from)
            {
                throw new ArgumentException();
            }

            var offset = rnd.NextDouble(0, (to - from).TotalMilliseconds);
            return from.AddMilliseconds(offset);
        }
        public static TimeSpan NextTimeSpan(this Random rnd, TimeSpan from, TimeSpan to)
        {
            if (to <= from)
            {
                throw new ArgumentException();
            }

            var offset = rnd.NextLong((to - from).Ticks);
            return from.Add(TimeSpan.FromTicks(offset));
        }

        public static int NextSign(this Random rnd)
        {
            return rnd.NextBool() ? 1 : -1;
        }

        public static double NextDouble(this Random rnd, double from, double to)
        {
            if (from > to)
            {
                throw new ArgumentOutOfRangeException();
            }

            return rnd.NextDouble() * (to - from) + from;
        }

        public static byte NextByte(this Random rnd)
        { 
            unchecked
            {
                return (byte)rnd.Next();
            };
        }
        public static byte NextByte(this Random rnd, byte from, byte to)
        {
            unchecked
            {
                return (byte)rnd.Next(from, to);
            };
        }

        public static uint NextUInt(this Random rnd)
        {
            unchecked
            {
                return (uint)rnd.Next();
            };
        }

        public static T NextElementFromList<T>(this Random rnd, IList<T> elements)
        {
            return elements[rnd.Next(0, elements.Count)];
        }
        public static T NextElementFrom<T>(this Random rnd, params T[] elements)
        {
            return elements[rnd.Next(0, elements.Length)];
        }
        public static string NextENWord(this Random rnd)
        {
            return ENWords[rnd.Next(0, ENWords.Length)];
        }
        public static string NextRUWord(this Random rnd)
        {
            return RUWords[rnd.Next(0, RUWords.Length)];
        }
        public static string[] NextENRUWordPair(this Random rnd)
        {
            var index = rnd.Next(0, RUWords.Length);
            return new[] { ENWords[index], RUWords[index] };
        }
        public static int NextPercent(this Random rnd)
        {
            return rnd.Next(0, 101);
        }
        public static List<float> NextSingles(this Random rnd, int count)
        {
            List<float> result = new List<float>(count);

            for (int i = 0; i < count; i++)
            {
                var val = rnd.NextDouble().ToSingle();
                result.Add(val);
            }

            return result;
        }
        public static List<double> NextDoubles(this Random rnd, int count)
        {
            List<double> result = new List<double>(count);

            for (int i = 0; i < count; i++)
            {
                result.Add(rnd.NextDouble());
            }

            return result;
        }

        public static IEnumerable<T> NextArray<T>(this Random rnd, IEnumerable<T> elements, int count)
        {
            if (elements is IList<T> == false)
            {
                elements = elements.ToArray();
            }

            var eCount = elements.Count();
            for (int i = 0; i < count; i++)
            {
                yield return elements.ElementAt(rnd.Next(eCount));
            }
        }

        public static IEnumerable<T> NextArray<T>(this Random rnd, IEnumerable<T> elements, int count, int maxNumOfSameElementsInRow)
        {
            if (maxNumOfSameElementsInRow < 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (elements is IList<T> == false)
            {
                elements = elements.ToArray();
            }
            if (elements.Count() < 2 && count > maxNumOfSameElementsInRow)
            {
                throw new ArgumentOutOfRangeException();
            }

            var eCount = elements.Count();
            var lastI = 0;
            var eICount = 0;
            for (int i = 0; i < count; i++)
            {
                var eI = rnd.Next(eCount);
                if (eI == lastI)
                {
                    eICount++;
                }
                else
                {
                    eICount = 0;
                }

                if (eICount > maxNumOfSameElementsInRow)
                {
                    i--;
                    continue;
                }
                else
                {
                    yield return elements.ElementAt(eI);
                    lastI = eI;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="from">Включая</param>
        /// <param name="to">Не включая</param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<int> NextUnique(this Random rnd, int from, int to, int count)
        {
            if (count > to - from)
            {
                throw new ArgumentException
                    ("Кол-во элементов больше, чем кол-во уникальных значений (count > to - from)", nameof(count));
            }

            var result = new List<int>();
            while (result.Count < count)
            {
                for (int i = 0; i < count; i++)
                {
                    int index = rnd.Next(0, to - from);
                    result.Add(index);
                }
                result = result.Distinct().ToList();
            }

            result.RemoveRange(0, result.Count - count);
            return result
                .Select(v => v + from)
                .ToList();
        }
    }
}
