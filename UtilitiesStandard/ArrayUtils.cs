using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Utilities.Extensions;

namespace Utilities
{
    public static class ArrayUtils
    {
        #region ##### IEnumerable #####

        public class CompareResult
        {
            public bool IsMatch { get; private set; }
            public List<int> MismatchPositions { get; private set; }
            public Dictionary<int, string> MismatchInfo { get; private set; }
            public string Commentary { get; private set; }

            public CompareResult
                (bool isMatch, List<int> mismatchPositions, Dictionary<int, string> mismatchInfo, string commentary)
            {
                IsMatch = isMatch;
                MismatchPositions = mismatchPositions;
                MismatchInfo = mismatchInfo;
                Commentary = commentary;
            }

            public override string ToString()
            {
                var info = new StringBuilder();
                MismatchInfo.ForEach(kvp => info.AppendLine($"[{kvp.Key}]: {kvp.Value}"));
                info.AppendLine($"Commenatry: {Commentary}");

                return info.ToString();
            }
        }

        public static CompareResult Compare<T>
            (IEnumerable<T> array1, IEnumerable<T> array2, Func<T, T, bool> equalityComparer)
        {
            if (array1 == null || array2 == null)
            {
                throw new ArgumentNullException("array1 == null | array2 == null");
            }

            List<T> a1 = array1.ToList();
            List<T> a2 = array2.ToList();
            int endIndex = Math.Min(a1.Count, a2.Count);

            List<int> mismatchPositions = new List<int>();
            Dictionary<int, string> mismatchInfo = new Dictionary<int, string>();
            for (int i = 0; i < endIndex; i++)
            {
                T a1Val = a1[i];
                T a2Val = a2[i];
                if (!equalityComparer(a1Val, a2Val))
                {
                    mismatchPositions.Add(i);
                    mismatchInfo.Add(i, $"{a1Val} != {a2Val}");
                }
            }
            bool isMatch = a1.Count == a2.Count && mismatchPositions.Count == 0;
            string commentary = a1.Count != a2.Count ?
                "a1.Count != a2.Count({0}!={1})".Format(a1.Count, a2.Count) : "";
            return new CompareResult(isMatch, mismatchPositions, mismatchInfo, commentary);
        }
        public static CompareResult Compare(IEnumerable<double> array1, IEnumerable<double> array2, double epsilon)
        {
            if (array1 == null || array2 == null)
            {
                throw new ArgumentNullException("array1 == null | array2 == null");
            }

            List<double> a1 = array1.ToList();
            List<double> a2 = array2.ToList();
            int endIndex = Math.Min(a1.Count, a2.Count);

            List<int> mismatchPositions = new List<int>();
            Dictionary<int, string> mismatchInfo = new Dictionary<int, string>();
            for (int i = 0; i < endIndex; i++)
            {
                double a1Val = a1[i];
                double a2Val = a2[i];
                double err = (a1Val - a2Val).Abs();
                if (err > epsilon) 
                {
                    mismatchPositions.Add(i);
                    mismatchInfo.Add(i, "a1={0}, a2={1}, Delta={2}".Format(a1Val, a2Val, err));
                }
            }
            bool isMatch = a1.Count == a2.Count && mismatchPositions.Count == 0;
            string commentary = a1.Count != a2.Count ? 
                "a1.Count != a2.Count({0}!={1})".Format(a1.Count, a2.Count) : "";
            return new CompareResult(isMatch, mismatchPositions, mismatchInfo, commentary);
        }

        public static CompareResult Compare(string str1, string str2)
        {
            if (str1 == null || str2 == null)
            {
                throw new ArgumentNullException("str1 == null || str2 == null");
            }

            int endIndex = Math.Min(str1.Length, str2.Length);

            List<int> mismatchPositions = new List<int>();
            Dictionary<int, string> mismatchInfo = new Dictionary<int, string>();
            for (int i = 0; i < endIndex; i++)
            {
                if (str1[i] != str2[i])
                {
                    mismatchPositions.Add(i);
                    mismatchInfo.Add(i, "str1={0}, str2={1}".Format(str1[i], str2[i]));
                }
            }
            bool isMatch = str1.Length == str2.Length && mismatchPositions.Count == 0;
            string commentary = str1.Length != str2.Length ? 
                "str1.Length != str2.Length ({0}!={1})".Format(str1.Length, str2.Length) : "";
            return new CompareResult(isMatch, mismatchPositions, mismatchInfo, commentary);
        }

        public static IEnumerable<T> ConcatSequences<T>(params IEnumerable<T>[] arrays)
        {
            return ConcatAllSequences((IEnumerable<IEnumerable<T>>)arrays);
        }
        public static IEnumerable<T> ConcatAllSequences<T>(IEnumerable<IEnumerable<T>> arrays)
        {
            foreach (var array in arrays)
            {
                foreach (var item in array)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<double> Range(double from, double step, double to)
        {
            for (double v = from; v < to; v += step)
            {
                yield return v;
            }
        }
        /// <summary>
        /// [from; to)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="step"></param>
        /// <param name="to">Exclusive!</param>
        /// <returns></returns>
        public static IEnumerable<int> Range(int from, int step, int to)
        {
            for (int v = from; v < to; v += step)
            {
                yield return v;
            }
        }

        #endregion

        #region ##### ICollection #####

        #endregion

        #region ##### IList #####

        public static List<TElement> Overwrite<TElement>(List<TElement> destination, List<TElement> source)
        {
            return Overwrite<List<TElement>, TElement>(destination, source, 0);
        }
        public static List<TElement> Overwrite<TElement>(List<TElement> destination, List<TElement> source, int destinationOffset)
        {
            return Overwrite<List<TElement>, TElement>(destination, source, destinationOffset);
        }
        public static List<TElement> Overwrite<TElement>(List<TElement> destination, List<TElement> source, int destinationOffset, int count)
        {
            return Overwrite<List<TElement>, TElement> (destination, source, destinationOffset, count);
        }
        public static T Overwrite<T, TElement>(T destination, T source, int destinationOffset)
            where T : IList<TElement>
        {
            for (int i = destinationOffset, j = 0; j < source.Count; i++, j++)
            {
                destination[i] = source[j];
            }

            return destination;
        }
        public static T Overwrite<T, TElement>(T destination, T source, int destinationOffset, int count)
            where T : IList<TElement>
        {
            for (int i = destinationOffset, j = 0; j < count; i++, j++)
            {
                destination[i] = source[j];
            }

            return destination;
        }

        public static T OverwriteWithExpansion<T, TElement>(T destination, T source, int destinationOffset)
            where T : IList<TElement>
        {
            var overlap = source.Count - (destination.Count - destinationOffset);
            destination.Add(default(TElement), overlap);

            for (int i = destinationOffset, j = 0; j < source.Count; i++, j++)
            {
                destination[i] = source[j];
            }

            return destination;
        }
        public static T OverwriteWithExpansion<T, TElement>(T destination, T source, int destinationOffset, int sourceOffset)
            where T : IList<TElement>
        {
            var overlap = (source.Count - sourceOffset) - (destination.Count - destinationOffset);
            if (overlap > 0)
            {
                destination.Add(default(TElement), overlap);
            }

            int di = destinationOffset;
            int si = sourceOffset;
            for (; si < source.Count; di++, si++)
            {
                destination[di] = source[si];
            }

            return destination;
        }
        public static T OverwriteWithExpansion<T>(T destination, T source, int destinationOffset, int sourceOffset)
            where T : IList<double>
        {
            return OverwriteWithExpansion<T, double>(destination, source, destinationOffset, sourceOffset);
        }

        public static T SetAll<T, TElement>(T array, TElement value)
            where T : IList<TElement>
        {
            for (int i = 0; i < array.Count; i++)
            {
                array[i] = value;
            }

            return array;
        }

        public static void TruncateByShorter<T>(params IList<T>[] collections)
        {
            int minCount = collections.Min(c => c.Count);
            for (int i = 0; i < collections.Length; i++)
            {
                collections[i].TruncateByCount(minCount);
            }
        }

        public static List<T> Merge<T>(Func<T[], T> mergeFunc, params IList<T>[] collections)
        {
            if (collections.Length == 0)
            {
                return new List<T>();
            }
            int count = collections[0].Count;
            ThrowUtils.ThrowIf_True(collections.Any(c => c.Count != count));

            T[] tmp = new T[collections.Length];
            List<T> result = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < collections.Length; j++)
                {
                    tmp[j] = collections[j][i];
                }
                result.Add(mergeFunc(tmp));
            }

            return result;
        }

        #endregion

        #region ##### IDictionary #####

        public static void AddItems<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary, params KeyValuePair<TKey, TValue>[] items)
        {
            dictionary.AddRange(items);
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

        //public static T CreateJaggedArray<T>(params int[] lengths)
        //{
        //    var type = typeof(T);
        //    var elementType = getRootElementType();
        //    var depth = getDepth();

        //    var reverseLengths = lengths.Reverse().ToArray();
        //    Array array = Array.CreateInstance(elementType, reverseLengths[0]);
        //    for (int i = 1; i < depth; i++)
        //    {
        //        var length = reverseLengths[i];
        //        var tmpArray = Array.CreateInstance(array.GetType(), length);
        //        for (int k = 0; k < length; k++)
        //        {
        //            var tmp =
        //            Array.Copy(array, tmpArray, 1);
        //            length.SetValue(Array.Copy(array, tmpArray))
        //        }
        //    }

        //    Type getRootElementType()
        //    {
        //        var t = type;
        //        while (t.GetElementType().IsArray)
        //        {
        //            t = t.GetElementType();
        //        }

        //        return t.GetElementType();
        //    }
        //    int getDepth()
        //    {
        //        var i = 1;
        //        var t = type;
        //        while (t.GetElementType().IsArray)
        //        {
        //            t = t.GetElementType();
        //            i++;
        //        }

        //        return i;
        //    }
        //}

        #endregion

        #region ##### List #####

        public static List<T> CreateList<T>(int count, T initial, Func<T, T> factory)
        {
            ThrowUtils.ThrowIf_Negative(count);

            List<T> collection = new List<T>();
            var last = initial;
            for (int i = 0; i < count; i++)
            {
                var current = factory(last);
                collection.Add(current);
                last = current;
            }

            return collection;
        }

        public static List<T> CreateList<T>(T element, int count)
        {
            ThrowUtils.ThrowIf_Negative(count);

            List<T> collection = new List<T>();
            for (int i = 0; i < count; i++)
            {
                collection.Add(element);
            }

            return collection;
        }

        public static List<double> CreateListByRange(double from, double step, int count)
        {
            ThrowUtils.ThrowIf_Negative(count);

            List<double> collection = new List<double>();
            for (int i = 0; i < count; i++)
            {
                collection.Add(from + step * i);
            }

            return collection;
        }
        public static List<int> CreateListByRange(int from, int step, int count)
        {
            ThrowUtils.ThrowIf_Negative(count);

            List<int> collection = new List<int>();
            for (int i = 0; i < count; i++)
            {
                collection.Add(from + step * i);
            }

            return collection;
        }

        public static List<T> CreateListByElements<T>(params T[] objs)
        {
            return new List<T>(objs);
        }

        #endregion
    }
}
