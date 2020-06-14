using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Extensions
{
    public static class SpecificArrayEx
    {
        #region ##### IEnumerable #####

        public static string ToHex(this IEnumerable<byte> bytes)
        {
            return bytes.Select(b => b.ToString("X2")).Aggregate(" ");
        }

        public static IEnumerable<int> SkipNegative(this IEnumerable<int> sequence)
        {
            foreach (var item in sequence)
            {
                if (item > 0)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> GetCorresponding<T, TKey>(this IEnumerable<T> sequence, Func<T, TKey> keyExtractor, params TKey[] keys)
        {
            var tmp = sequence.Select(v => (Item: v, Key: keyExtractor(v))).MakeCached();

            foreach (var key in keys)
            {
                yield return tmp.First(inf => Equals(inf.Key, key)).Item;
            }
        }

        public static IEnumerable<T> Get<T>(this IEnumerable<T> sequence, IEnumerable<int> indexes)
        {
            indexes = indexes.MakeCached();

            var i = 0;
            foreach (var item in sequence)
            {
                if (indexes.Contains(i))
                {
                    yield return item;
                }

                i++;
            }
        }

        public static bool AllTrue(this IEnumerable<bool> sequence)
        {
            return sequence.All(v => v);
        }

        /// <summary>
        /// Creates new or overwrites old
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns><paramref name="sequence"/></returns>
        public static IEnumerable<byte> SaveToFile(this IEnumerable<byte> sequence, string directory, string fileName)
        {
            return SaveToFile(sequence, Path.Combine(directory, fileName));
        }
        public static IEnumerable<byte> SaveToFile(this IEnumerable<byte> sequence, string path)
        {
            IOUtils.TryCreateFileOrNull(path, sequence.ToArray()).Close();

            return sequence;
        }

        public static FindResult<T> FindMax<T>(this IEnumerable<T> sequence)
        {
            var max = sequence.Max();

            return sequence.Find(max);
        }
        public static FindResult<T> FindMax<T, TKey>(this IEnumerable<T> sequence, Func<T, TKey> keyExtractor)
        {
            var max = sequence.Max(keyExtractor);

            return sequence.Find(v => Equals(keyExtractor(v), max));
        }

        public static T FindClosestValueOrDefault<T>(this IEnumerable<T> sequence, Func<T, double> keySelector, double key)
        {
            return sequence
                .OrderBy(v => (keySelector(v) - key).Abs())
                .FirstOrDefault();
        }
        public static FindResult<T> FindClosest<T>(this IEnumerable<T> sequence, Func<T, double> keySelector, double key)
        {
            return sequence
                .Select((v, i) => new { V = v, P = keySelector(v), I = i })
                .OrderBy(pi => (pi.P - key).Abs())
                .Select(pi => new FindResult<T>(pi.I, pi.V))
                .FirstOrDefault() ?? new FindResult<T>();
        }
        public static FindResult<double> FindClosestPoint<T>(this IEnumerable<T> sequence, Func<T, double> pointSelector, double point)
        {
            return sequence
                .Select((v, i) => new { P = pointSelector(v), I = i })
                .OrderBy(pi => (pi.P - point).Abs())
                .Select(pi => new FindResult<double>(pi.I, pi.P))
                .FirstOrDefault() ?? new FindResult<double>();
        }
        public static FindResult<double> FindClosestPoint(this IEnumerable<double> sequence, double point)
        {
            return sequence.FindClosestPoint(p => p, point);
        }
        public static FindResult<int> FindClosestPoint(this IEnumerable<int> sequence, int point)
        {
            return sequence
                .Select(i => (double)i)
                .FindClosestPoint(point)
                .Cast(d => d.Round());
        }

        public static IEnumerable<double> NegativeToZero(this IEnumerable<double> sequence) 
        {
            return sequence.Select(p => p < 0 ? 0 : p);
        }
        public static T NegativeToZeroSelf<T>(this T sequence)
            where T : IList<double>
        {
            for (int i = 0; i < sequence.Count; i++)
            {
                var v = sequence[i];
                sequence[i] = v < 0 ? 0 : v;
            }

            return sequence;
        }
        public static IEnumerable<double> PositiveToZero(this IEnumerable<double> sequence)
        {
            return sequence.Select(p => p > 0 ? 0 : p);
        }

        #region ##### ToXXX #####

        public static string ToClipboard(this IEnumerable<char> str)
        {
            return str.AsString().ToClipboard();
        }

        public static IEnumerable<double> ToDoubles(this IEnumerable<int> sequence)
        {
            return sequence.Select(v => (double)v);
        }
        public static IEnumerable<double> ToDoubles(this IEnumerable<float> sequence)
        {
            return sequence.Select(v => (double)v);
        }
        public static IEnumerable<int> ToIntegers(this IEnumerable<double> sequence)
        {
            return sequence.Select(v => v.Round());
        }
        public static IEnumerable<int> ToIntegers(this IEnumerable<float> sequence)
        {
            return sequence.Select(v => v.Round());
        }

        public static IEnumerable<TTo> ChangeType<TTo, TFrom>(this IEnumerable<TFrom> sequence)
            where TTo : struct
            where TFrom : struct
        {
            return sequence
                .Select(v => Convert.ChangeType(v, typeof(TTo)))
                .Cast<TTo>();
        }


        public static IEnumerable<string> ToExcelTable(this IEnumerable<double> sequence, bool useComma = false)
        {
            var rows = sequence.Select((v, i) => $"{i}\t{v.ToStringInvariant()}{Global.NL}");
            if (useComma)
            {
                rows = rows.Select(r => r.Replace('.', ','));
            }

            return rows;
        }
        public static IEnumerable<string> ToExcelTable<T>(this IEnumerable<T> sequence, params Func<T, string>[] columnsExtractors)
        {
            return sequence.Select((v, i) => columnsExtractors.Select(ce => ce(v)).Aggregate((acc, val) => acc + '\t' + val));
        }
        public static Table ToTable<T>(this IEnumerable<T> sequence, params Func<T, string>[] columnsExtractors)
        {
            var cells = sequence.Select((v, i) => columnsExtractors.Select(ce => ce(v))).Flatten().ToArray();
            return new Table(cells, cells.Length / columnsExtractors.Length, columnsExtractors.Length);
        }

        //public static string ToASCIIString(this IEnumerable<byte> sequence)
        //{
        //    return Encoding.ASCII.GetString(sequence.ToArray());
        //}
        //public static string ToUTF8String(this IEnumerable<byte> sequence)
        //{
        //    return Encoding.UTF8.GetString(sequence.ToArray());
        //}

        public static IEnumerable<T> GetValues<T>(this IEnumerable<FindResult<T>> sequence)
        {
            return sequence.Where(fi => fi.Found).Select(fi => fi.Value);
        }
        public static IEnumerable<int> GetIndexes<T>(this IEnumerable<FindResult<T>> sequence)
        {
            return sequence.Where(fi => fi.Found).Select(fi => fi.Index);
        }

        public static string ToBase64(this IEnumerable<byte> sequence)
        {
            return Convert.ToBase64String(sequence.ToArray());
        }

        public static MemoryStream ToMemoryStream(this IEnumerable<byte> sequence)
        {
            return new MemoryStream(sequence.ToArray());
        }

        public static MemoryStream ToExpandableMemoryStream(this IEnumerable<byte> sequence)
        {
            var ms = new MemoryStream();
            ms.Write(sequence);

            return ms;
        }

        #endregion

        /// <summary>
        /// Just aggregates <paramref name="sequence"/>'s characters into string.
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static string Aggregate(this IEnumerable<char> sequence)
        {   
            return sequence == null 
                ? null
                : new string(sequence.ToArray()); 
        }
        //public static string Aggregate(this IEnumerable<string> sequence)
        //{
        //    return sequence.Aggregate(Global.NL);
        //}
        public static string Aggregate(this IEnumerable<string> sequence, string separator)
        {
            var sb = new StringBuilder();
            if (sequence.IsEmpty())
            {
                return "";
            }
            sequence.ForEach(s => sb.Append(s).Append(separator));
            sb.Remove(sb.Length - separator.Length, separator.Length);

            return sb.ToString();
        }

        public static int AggregateBySum(this IEnumerable<byte> sequence)
        {
            return sequence.Aggregate(0, (last, val) => val + last);
        }
        public static double AggregateBySum(this IEnumerable<double> sequence)
        {
            return sequence.Aggregate(0D, (last, val) => val + last);
        }
        public static int AggregateByMul(this IEnumerable<int> sequence)
        {
            return sequence.Aggregate(1, (last, val) => val * last);
        }
        public static double AggregateByMul(this IEnumerable<double> sequence)
        {
            return sequence.Aggregate(1D, (last, val) => val * last);
        }

        public static IEnumerable<TValue> SelectKVPValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> sequence)
        {
            return sequence.Select(kvp => kvp.Value);
        }
        public static IEnumerable<TKey> SelectKVPKey<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> sequence)
        {
            return sequence.Select(kvp => kvp.Key);
        }

        #endregion

        #region ##### ICollection #####

        #endregion

        #region ##### IList #####

        public static T InsertZeros<T>(this T a1, int position, int count)
            where T : IList<double>
        {
            for (int i = 0; i < count; i++)
            {
                a1.Insert(position, 0);
            }

            return a1;
        }

        public static IList<double?> ToNullable(this IList<double> sourceArray, IList<bool> nullMask)
        {
            ThrowUtils.ThrowIf_NullArgument(nullMask);

            return sourceArray
                .ToArray()
                .ToNullable(nullMask.ToArray())
                .ToList();
        }

        public static IList<bool> InvertValues(this IList<bool> sourceArray)
        {
            for (int i = 0; i < sourceArray.Count; i++)
            {
                sourceArray[i] = !sourceArray[i];
            }

            return sourceArray;
        }

        public static T NaNToZero<T>(this T numbers)
             where T : IList<double>
        {
            return numbers.ReplaceNaN(0);
        }
        public static T ReplaceNaN<T>(this T numbers, double newValue)
            where T : IList<double>
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                double n = numbers[i];
                numbers[i] = n.IsNaN() ? newValue : n;
            }

            return numbers;
        }

        public static IEnumerable<double> NormalizeByPeak(this IEnumerable<double> values, double requiredPeak = 1)
        {
            //double coef = requiredPeak / values.Max();
            double coef = requiredPeak / Math.Max(values.Max(), -values.Min());

            // In order to defer .Max() evaluation
            foreach (var value in values.Select(v => v * coef))
            {
                yield return value;
            }
        }
        public static IEnumerable<double> NormalizeBySum(this IEnumerable<double> values, double requiredSum = 1)
        {
            double coef = requiredSum / values.Sum();

            // In order to defer .Sum() evaluation
            foreach (var value in values.Select(v => v * coef))
            {
                yield return value;
            } 
        }

        public static T NormalizeBySumSelf<T>(this T values, double requiredSum = 1)
            where T : IList<double>
        {
            double sum = values.Sum();

            return values
                .DivEachSelf(sum)
                .MulEachSelf(requiredSum);
        }
        public static T NormalizeByPeakSelf<T>(this T values, double requiredPeak = 1)
            where T : IList<double>
        {
            //double coef = requiredPeak / values.Max();
            double coef = requiredPeak / Math.Max(values.Max(), -values.Min());

            return values
                .MulEachSelf(coef);
        }

        public static T ShiftToZeroSelf<T>(this T values)
            where T : IList<double>
        {
            return values.ShiftToZeroSelf(out double _);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="shiftMade">This number was added to each value</param>
        /// <returns></returns>
        public static T ShiftToZeroSelf<T>(this T values, out double shiftMade)
            where T : IList<double>
        {
            shiftMade = -values.Min();

            return values.AddEachSelf(shiftMade);
        }

        #endregion

        #region ##### Array #####

        public static double?[] ToNullable(this double[] sourceArray, bool[] nullMask)
        {
            ThrowUtils.ThrowIf_True(nullMask.Length != sourceArray.Length, "Cannot convert to nullable array because nullMask.Length != sourceArray.Length");

            double?[] nullableArray = new double?[sourceArray.Length];
            for (int i = 0; i < nullableArray.Length; i++)
            {
                nullableArray[i] = nullMask[i] ? null : (double?)sourceArray[i];
            }

            return nullableArray;
        }

        public static IEnumerable<object> ToEnumerable(this Array array)
        {
            foreach (var item in array)
            {
                yield return item;
            }
        }

        #endregion

        #region ##### BitArray #####

        public static bool[] ToArray(this BitArray bitArray)
        {
            var arr = new bool[bitArray.Length];
            for (int i = 0; i < bitArray.Length; i++)
            {
                arr[i] = bitArray[i];
            }

            return arr;
        }
        public static List<bool> ToList(this BitArray bitArray)
        {
            var arr = new List<bool>(bitArray.Length);
            for (int i = 0; i < bitArray.Length; i++)
            {
                arr[i] = bitArray[i];
            }

            return arr;
        }
        /// <summary>
        /// Just yields values without creating a copy (so they can change between enumerations)
        /// </summary>
        /// <param name="bitArray"></param>
        /// <returns></returns>
        public static IEnumerable<bool> ToEnumerable(this BitArray bitArray)
        {
            for (int i = 0; i < bitArray.Length; i++)
            {
                yield return bitArray[i];
            }
        }

        #endregion
    }
}
