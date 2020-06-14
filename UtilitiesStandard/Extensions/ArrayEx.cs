using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Utilities.Types;

namespace Utilities.Extensions
{
    public static class ArrayEx
    {
        #region ##### Type convertions #####

        public static List<TOut> ChangeType<TIn, TOut>(this IList<TIn> array)
             where TIn : IConvertible
        {
            List<TOut> result = new List<TOut>(array.Count);
            for (int i = 0; i < array.Count; i++)
            {
                TOut newVal = (TOut)array[i].ToType(typeof(TOut), null);
                result.Add(newVal);
            }

            return result;
        }

        #endregion

        #region ##### IEnumerable #####

        public static IEnumerable<T> CastSafe<T>(this IEnumerable<object> context)
            where T : class
        {
            return context
                .Select(e => e as T)
                .SkipNulls();
        }

        public static IEnumerable<T> StoreTo<T>(this IEnumerable<T> sequence, IList<T> storage)
        {
            storage.AddRange(sequence.ToArray());

            return sequence;
        }

#warning bad implementation
        public static IEnumerable<T> Zip<T, TA, TB, TC>(this IEnumerable<TA> a, IEnumerable<TB> b, IEnumerable<TC> c, Func<TA, TB, TC, T> aggregator)
        {
            a = a.ToArray();
            b = b.ToArray();
            c = c.ToArray();

            if (a.Count() != b.Count() || b.Count() != c.Count())
            {
                throw new ArgumentOutOfRangeException("Count does not match");
            }
            else
            {
                foreach (var i in a.Count().Range())
                {
                    yield return aggregator(a.ElementAt(i), b.ElementAt(i), c.ElementAt(i));
                }
            }
        }

        public static IEnumerable<T> DublicateEach<T>(this IEnumerable<T> sequence, int numOfDublicates)
        {
            foreach (var item in sequence)
            {
                for (int i = 0; i < numOfDublicates; i++)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> Max<T>(this IEnumerable<T> sequence, int numOfMaxElements)
        {
            var list = sequence.ToList();
            for (int i = 0; i < numOfMaxElements; i++)
            {
                var element = list.Max();
                list.Remove(element);

                yield return element;
            }
        }

        public static IEnumerable<T> ToEmptyIfTrue<T>(this IEnumerable<T> sequence, bool condition)
        {
            return condition ? Enumerable.Empty<T>() : sequence;
        }

        public static IEnumerable<T> SkipFirstItem<T>(this IEnumerable<T> sequence)
        {
            return sequence.Skip(1);
        }
        public static IEnumerable<T> SkipNulls<T>(this IEnumerable<T> sequence)
        {
            return sequence.Where(v => v != null);
        }

        public static IEnumerable<object> ToGenericEnumerable(this IEnumerable sequence)
        {
            foreach (var item in sequence)
            {
                yield return item;
            }
        }

        public static Enumerator<T> StartEnumeration<T>(this IEnumerable<T> sequence)
        {
            return new Enumerator<T>(sequence);
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="sequence"></param>
        ///// <returns>2D matrix with one column</returns>
        //public static T[,] To2DArray<T>(this IEnumerable<T> sequence)
        //{
        //    return sequence.Select(v => (IEnumerable<T>)new[] { v }).To2DArray();
        //}
        public static T[,] To2DArray<T>(this IEnumerable<IEnumerable<T>> sequence)
        {
            var jagged = sequence.ToJaggedArray();
            var matrix2D = new T[jagged.Select(row => row.Count()).EmptyToNull()?.EnsureSame() ?? 0, jagged.Length];
            for (int col = 0; col < matrix2D.GetColumnsLength(); col++)
            {
                for (int row = 0; row < matrix2D.GetRowsLength(); row++)
                {
                    matrix2D[col, row] = jagged[row][col];
                }
            }

            return matrix2D;
        }
        public static T[][] ToJaggedArray<T>(this IEnumerable<IEnumerable<T>> sequence)
        {
            return sequence.Select(arr => arr.ToArray()).ToArray();
        }

        public static T EnsureSame<T>(this IEnumerable<T> sequence)
        {
            return sequence.Distinct().Single();
        }
        public static T EnsureSameOrDefault<T>(this IEnumerable<T> sequence, T defaultValue)
        {
            return sequence.Distinct().FirstOrDefault(defaultValue);
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> sequence)
        {
            return sequence.OrderBy(v => v);
        }

        public static bool CountNotLessOrEqual<T>(this IEnumerable<T> sequence, int count)
        {
            foreach (var item in sequence)
            {
                count--;
                if (count == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// If <paramref name="sequence"/> is in memory collection (like List or Array), method will return
        /// <paramref name="sequence"/>. Otherwise <paramref name="sequence"/> will be wrapped to 
        /// <see cref="CachedEnumerableDecorator{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static IEnumerable<T> MakeCached<T>(this IEnumerable<T> sequence)
        {
            return (sequence is IList<T> || sequence is IReadOnlyList<T>) 
                ? sequence 
                : new CachedEnumerableDecorator<T>(sequence);
        }

        public static IReadOnlyList<T> ToReadonlyList<T>(this IEnumerable<T> sequence)
        {
            return sequence.ToList().AsReadOnly();
        }

        /// <summary>
        /// If there are no enough elements in <see cref="sequence"/> exception will be thrown. 
        /// Extra elements will be ignored.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="numOfElementsOnGroup"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> GroupBy<T>(this IEnumerable<T> sequence, IEnumerable<int> groupsSizes)
        {
            groupsSizes = groupsSizes.MakeCached();
            var sequenceEnumerator = sequence.StartEnumeration();
            var groupSizesEnumerator = groupsSizes.StartEnumeration();

            foreach (var _ in groupsSizes.Count().Range())
            {
                var groupSize = groupSizesEnumerator.AdvanceOrThrow();
                var group = sequenceEnumerator.AdvanceRangeOrThrow().Take(groupSize).ToArray();

                yield return group;
            }
        }
        /// <summary>
        /// If <paramref name="sequence"/>.Count() % <paramref name="numOfElementsGroup"/> != 0 
        /// last elements will be skipped
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="numOfElementsGroup"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> GroupBy<T>(this IEnumerable<T> sequence, int numOfElementsGroup)
        {
            var group = new T[numOfElementsGroup];
            int i = 0;
            foreach (var items in sequence)
            {
                group[i++] = items;

                if (i == numOfElementsGroup)
                {
                    yield return group;
                    group = new T[numOfElementsGroup];
                    i = 0;
                }
            }
        }

        //public static IEnumerable<IEnumerable<T>> GroupByShifted<T>(this IEnumerable<T> sequence, int numOfElementsGroup)
        //{
        //    var group = new T[numOfElementsGroup];
        //    int i = 0;
        //    foreach (var items in sequence)
        //    {
        //        group[i++] = items;

        //        if (i == numOfElementsGroup)
        //        {
        //            yield return group;
        //            group = new T[numOfElementsGroup];
        //            i = 0;
        //        }
        //    }
        //}

        public static IEnumerable<IEnumerable<T>> GroupBy<T>(this IEnumerable<T> sequence, IEnumerable<T> startOfGroupMarker)
        {
            sequence = sequence.MakeCached();
            startOfGroupMarker = startOfGroupMarker.MakeCached();
            var groupStarts = new DisplaceCollection<int>(2);
            var end = sequence.Count() - startOfGroupMarker.Count();
            for (int i = 0; i <= end; i++)
            {
                var found = true;
                for (int k = 0; k < startOfGroupMarker.Count(); k++)
                {
                    if (!sequence.ElementAt(i + k).Equals(startOfGroupMarker.ElementAt(k)))
                    {
                        found = false;
                    }
                }

                if (found)
                {
                    if (found)
                    {
                        groupStarts.Add(i);
                    }

                    if (groupStarts.Count == 2)
                    {
                        yield return sequence.GetRangeSafe(groupStarts[0], groupStarts[1] - groupStarts[0]);
                    }
                }
            }
            groupStarts.Add(sequence.Count());
            if (groupStarts.Count == 2)
            {
                yield return sequence.GetRangeSafe(groupStarts[0], groupStarts[1] - groupStarts[0]);
            }
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> sequence)
        {
            foreach (var items in sequence)
            {
                foreach (var item in items)
                {
                    yield return item;
                }
            }
        }

        public static T LastItem<T>(this IEnumerable<T> sequence)
        {
            if (sequence.IsEmpty())
            {
                throw new ArgumentOutOfRangeException("There is no last element. The sequence is empty.");
            }

            return sequence.TakeLast(1).Single();
        }

        public static T FirstItem<T>(this IEnumerable<T> sequence)
        {
            foreach (var item in sequence)
            {
                return item;
            }

            throw new ArgumentException();
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> sequence, T defaultValue)
        {
            foreach (var item in sequence)
            {
                return item;
            }

            return defaultValue;
        }

        public static T FirstOrAnyOrDefault<T>(this IEnumerable<T> sequence, Func<T, bool> predicate)
        {
            T any = default;
            foreach (var item in sequence)
            {
                any = item;
                if (predicate(item))
                {
                    return item;
                }
            }

            return any;
        }

        public static bool IsNotEmpty<T>(this IEnumerable<T> sequence)
        {
            return !sequence.IsEmpty();
        }
        public static bool IsEmpty<T>(this IEnumerable<T> sequence)
        {
            foreach (var item in sequence)
            {
                return false;
            }
            return true;
        }
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> sequence)
        {
            return !sequence.IsNullOrEmpty();
        }
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
            {
                return true;
            }

            foreach (var item in sequence)
            {
                return false;
            }
            return true;
        }

        #region ##### To opperators #####

        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> sequence)
        {
            return new ObservableCollection<T>(sequence);
        }

        #endregion

        #region ##### GetXXX #####

        public static List<T> GetCopy<T>(this IEnumerable<T> array)
            where T : struct
        {
            return new List<T>(array);
        }

        /// <summary>
        /// The length of returned sequence can be less than <paramref name="count"/>.
        /// Uses optimization for <see cref="IList{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="from"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetRangeSafe<T>(this IEnumerable<T> sequence, int from, int count)
        {
            if (sequence is IList<T> asIList)
            {
                var end = Math.Min(asIList.Count, from + count);
                for (int i = from; i < end; i++)
                {
                    yield return asIList[i];
                }
            }
            else
            {
                foreach (var item in sequence.Skip(from).Take(count))
                {
                    yield return item;
                } 
            }
        }
        public static IEnumerable<T> GetRangeAroundSafe<T>(this IEnumerable<T> sequence, int center, int count)
        {
            var from = center - count / 2;
            count -= from.NegativeToZero() - from;
            from = from.NegativeToZero();

            return sequence.GetRangeSafe(from, count);
        }
        public static List<T> GetRangeTill<T>(this IEnumerable<T> sequence, int from, int to)
        {
            if (to < from)
            {
                throw new ArgumentOutOfRangeException();
            }

            return new List<T>(sequence.Skip(from).Take(to - from));
        }
        public static IEnumerable<T> GetRange<T>(this IEnumerable<T> sequence, int from, int count)
        {
            if (sequence is IList<T> asIList)
            {
                for (int i = from; i < from + count; i++)
                {
                    yield return asIList[i];
                }
            }
            else
            {
                foreach (var item in sequence.Skip(from).Take(count))
                {
                    yield return item;
                    count--;
                }

                if (count != 0)
                {
                    throw new IndexOutOfRangeException($"{count} elements were missed from the sequence");
                }
            }
        }

        #endregion

        #region ##### AsXXX #####

        /// <summary>
        /// Just aggregates the values into the string with empty separator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string AsString<T>(this IEnumerable<T> values)
        {
            return values.AsString("");
        }
        public static string AsString<T>(this IEnumerable<T> values, string splitter)
        {
            StringBuilder sb = new StringBuilder();
            values.ForEach((v, i) => sb.AppendFormat("{0}{1}", v, splitter));

            return sb.ToString();
        }
        public static string AsString<T>(this IEnumerable<T> values, char splitter)
        {
            return values.AsString(splitter.ToString());
        }

        public static string AsMultilineString<T>(this IEnumerable<T> values)
        {
            return AsMultilineString(values, v => v.ToString(), 0);
        }
        public static string AsMultilineString<T>(this IEnumerable<T> values, int indentWidth)
        {
            return AsMultilineString(values, v => v.ToString(), indentWidth);
        }
        public static string AsMultilineString<T>(this IEnumerable<T> values, int indentWidth, bool writeIndex)
        {
            return AsMultilineString(values, v => v.ToString(), indentWidth, writeIndex);
        }
        public static string AsMultilineString<T>(
            this IEnumerable<T> values, Func<T, string> toStringConverter, int indentWidth)
        {
            return AsMultilineString(values, toStringConverter, indentWidth, true);
        }
        public static string AsMultilineString<T>(
            this IEnumerable<T> values, Func<T, string> toStringConverter, int indentWidth, bool writeIndex)
        {
            StringBuilder sb = new StringBuilder();
            string indent = new string('\t', indentWidth);
            if (writeIndex)
            {
                values.ForEach((v, i) => sb.AppendFormatLine("{0}[{1}]: {2}", indent, i, toStringConverter(v)));
            }
            else
            {
                values.ForEach((v, i) => sb.AppendFormatLine("{0}{1}", indent, toStringConverter(v)));
            }
            return sb.ToString();
        }

        public static string AsString(this IEnumerable<byte> arr, Encoding encoding)
        {
            var array = arr.ToArray();
            return encoding.GetString(array, 0, array.Length);
        }

        #endregion

        public static IEnumerable<T> TakeWhileInclusive<T>(this IEnumerable<T> sequence, Func<T, bool> predicate)
        {
            var isFirst = true;
            foreach (var e in sequence)
            {
                if (predicate(e))
                {
                    yield return e;
                }
                else if (!isFirst)
                {
                    yield return e;
                    yield break;
                }
                else
                {
                    yield break;
                }

                isFirst = false;
            }
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> sequence, T value)
        {
            return sequence.Concat(new T[] { value });
        }

        public static IEnumerable<T> ConcatIfNotNull<T>(this IEnumerable<T> sequence, T value)
        {
            return value == null 
                ? sequence 
                : sequence.Concat(value);
        }

        public static IEnumerable<FindResult<T>> FindAllMismatches<T>(this IEnumerable<T> sequence1, IEnumerable<T> sequence2)
        {
            sequence1 = sequence1.MakeCached();
            sequence2 = sequence2.MakeCached();

            if (sequence1.Count() != sequence2.Count())
            {
                throw new ArgumentException("Sequences must have same size");
            }

            for (int i = 0; i < sequence1.Count(); i++)
            {
                var e1 = sequence1.ElementAt(i);
                var e2 = sequence2.ElementAt(i);
                if (!Equals(e1, e2))
                {
                    yield return new FindResult<T>(i, e1);
                }
            }
        }

        public static FindResult<T> First<T>(this IEnumerable<T> sequence, Func<T, int, bool> predicate)
        {
            var i = 0;
            foreach (var item in sequence)
            {
                if (predicate(item, i))
                {
                    return new FindResult<T>(i, item);
                }
                i++;
            }

            throw new InvalidOperationException("Sequence contains no matching element");
        }

        public static IEnumerable<T> EmptyToNull<T>(this IEnumerable<T> sequence)
        {
            return sequence.IsEmpty()
                ? null
                : sequence;
        }
        public static string EmptyToNull(this string str)
        {
            return str.IsEmpty()
                ? null
                : str;
        }

        public static IEnumerable<T> NullToEmpty<T>(this IEnumerable<T> sequence)
        {
            return sequence ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// Doesn't throw an exception if 
        /// <paramref name="sequence"/>.Count() < <paramref name="count"/> when <paramref name="strictCount"/> is False
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> TakeFromEnd<T>(this IEnumerable<T> sequence, int count, bool strictCount = false)
        {
            var buffer = new DisplaceCollection<T>(count);
            foreach (var item in sequence)
            {
                buffer.Add(item);
            }

            if (strictCount && buffer.Count != count)
            {
                throw new ArgumentException("There are no enough elements in the sequence");
            }
            else
            {
                for (int i = 0; i < buffer.Count; i++)
                {
                    yield return buffer[i];
                }
            }
        }

        /// <summary>
        /// Не выбрасывает исключение если sequence.Count <= count
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> SkipFromEnd<T>(this IEnumerable<T> sequence, int count)
        {
            var future = sequence.Take(count).ToArray();
            if (future.Length != count)
            {
                yield break;
            }
            else if (count == 0)
            {
                foreach (var item in sequence)
                {
                    yield return item;
                }
            }
            else
            {
                foreach (var item in sequence.Skip(count))
                {
                    yield return future[0];
                    future.MoveLeftSelf(1);
                    future[count - 1] = item;
                }
            }
        }
        public static IEnumerable<T> SkipFromEndWhile<T>(this IEnumerable<T> sequence, Func<T, bool> predicate)
        {
            int maxPosition = 0;
            var tmpPosition = 0;
            foreach (var item in sequence)
            {
                if (!predicate(item))
                {
                    maxPosition = tmpPosition;
                }

                tmpPosition++;
            }

            int currPosition = 0;
            foreach (var item in sequence)
            {
                if (currPosition > maxPosition)
                {
                    yield break;
                }
                else
                {
                    yield return item;
                }

                currPosition++;
            }
        }

        /// <summary>
        /// Не меняет порядок следования элементов.
        /// Выбрасывает исключение если sequence.Count < count
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> sequence, int count)
        {
            if (count == 0)
            {
                yield break;
            }

            var past = sequence.Take(count).ToArray();
            if (past.Length != count)
            {
                throw new ArgumentOutOfRangeException("past.Length != count");
            }
            else
            {
                foreach (var item in sequence)
                {
                    past.MoveLeftSelf(1);
                    past[count - 1] = item;
                }
                foreach (var item in past)
                {
                    yield return item;
                }
            }
        }
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> sequence, int count, bool throwIfNotEnough)
        {
            if (count == 0)
            {
                yield break;
            }

            var past = sequence.Take(count).ToArray();
            if (past.Length != count && throwIfNotEnough)
            {
                throw new ArgumentOutOfRangeException("past.Length != count");
            }
            else
            {
                foreach (var item in sequence)
                {
                    past.MoveLeftSelf(1);
                    past[past.Length - 1] = item;
                }
                foreach (var item in past)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// This method does not use <seealso cref="object.GetHashCode"/>, so it can be slow.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEnumerable<IGrouping<TKey, T>> Group<T, TKey>(this IEnumerable<T> sequence, 
            Func<T, TKey> keyExtractor, Func<TKey, TKey, bool> comparer)
        {
            return sequence.GroupBy(v => keyExtractor(v), v => v, new IEqualityComparerAction<TKey>(comparer));
        }
        /// <summary>
        /// This method does not use <seealso cref="object.GetHashCode"/>, so it can be slow.
        /// Consider using <seealso cref="Distinct{T}(IEnumerable{T}, Func{T, T, bool}, Func{T, int})"/>
        /// instead.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> sequence, Func<T, T, bool> comparer)
        {
            return sequence.Distinct(new IEqualityComparerAction<T>(comparer));
        }
        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> sequence, Func<T, T, bool> comparer, Func<T, int> getHashCode)
        {
            return sequence.Distinct(new IEqualityComparerAction<T>(comparer, getHashCode));
        }
        /// <summary>
        /// Uses type's default IEqualityComparer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="keyExtractor"></param>
        /// <returns></returns>
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> sequence, Func<T, TKey> keyExtractor)
        {
            var keyComparer = EqualityComparer<TKey>.Default;
            var comparer = new IEqualityComparerAction<T>((a, b) => keyComparer.Equals(keyExtractor(a), keyExtractor(b)));
            return sequence.Distinct(comparer);
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach(var e in sequence)
            {
                action(e);
            }

            return sequence;
        }
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> sequence, Action<T, int> action)
        {
            int index = 0;

            foreach (var elem in sequence)
            {
                action(elem, index);
                index++;
            }

            return sequence;
        }

        #region ##### FindXXX #####

        public static int FindIndex<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.Find(predicate).Index;
        }
        public static int FindIndex<T>(this IEnumerable<T> source, IEnumerable<T> what)
        {
            return source.Find(what).Index;
        }
        public static IEnumerable<int> FindAllIndexes<T>(this IEnumerable<T> source, IEnumerable<T> what)
        {
            return source.FindAll(what).Select(fi => fi.Index);
        }
        public static IEnumerable<int> FindAllIndexes<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source
                .Select((v, i) => (v, i))
                .Where(v => predicate(v.v))
                .Select(v => v.i);
        }

        public static FindResult<T> Find<T>(this IEnumerable<T> array, T value)
        {
            return array.Find(new[] { value });
        }
        public static IEnumerable<FindResult<T>> FindAll<T>(this IEnumerable<T> array, T value)
        {
            return array.FindAll(new[] { value });
        }
        public static FindResult<T> Find<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            int i = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    return new FindResult<T>(i, item);
                }
                i++;
            }

            return new FindResult<T>();
        }
        public static FindResult<T> Find<T>(this IEnumerable<T> source, IEnumerable<T> what)
        {
            return source.Find(what, 0);
        }
        public static FindResult<T> FindLast<T>(this IEnumerable<T> source, IEnumerable<T> what)
        {
            return source.FindAll(what).LastOrDefault() ?? new FindResult<T>();
        }
        public static async Task<FindResult<T>> FindAsync<T>(this IEnumerable<T> source, IEnumerable<T> what)
        {
            return await Task.Run(() => source.Find(what, 0));
        }
        public static async Task<FindResult<T>> FindAsync<T>(this IEnumerable<T> source, IEnumerable<T> what, int startIndex)
        {
            return await Task.Run(() => source.Find(what, startIndex));
        }
        public static FindResult<T> Find<T>(this IList<T> source, IList<T> what, int startIndex)
        {
            int offset = startIndex;
            while (source.Count - offset >= what.Count)
            {
                int i = source.Skip(offset).FindIndex(b => b.Equals(what[0]));
                if (i == -1 || (source.Count - offset - i) < what.Count)
                {
                    break;
                }
                else // First element match at i
                {
                    int j;
                    for (j = 0; j < what.Count; j++) // Check other matches
                    {
                        if (!what[j].Equals(source[offset + i + j]))
                        {
                            break;
                        }
                        else if (j == what.Count - 1)
                        {
                            return new FindResult<T>(offset + i, source);
                        }
                    }

                    offset += i + 1;
                }
            }

            return new FindResult<T>();
        }
        public static FindResult<T> Find<T>(this IEnumerable<T> source, IEnumerable<T> what, int startIndex)
        {
            source = source
                .MakeCached()
                .Skip(startIndex);
            var sourceCount = source.Count();
            //List<T> sourceCopy = new List<T>(source.Skip(startIndex));
            List<T> whatList = what.ToList();

            int removedCount = startIndex;
            while (sourceCount >= whatList.Count)
            {
                int i = source.FindIndex(b => b.Equals(whatList[0]));
                if (i == -1 || (sourceCount - i) < whatList.Count)
                {
                    break;
                }
                else // First element match at i
                {
                    int j;
                    for (j = 0; j < whatList.Count; j++) // Check other matches
                    {
                        if (!whatList[j].Equals(source.ElementAt(i + j))) 
                        {
                            break;
                        }
                        else if (j == whatList.Count - 1)
                        {
                            return new FindResult<T>(removedCount + i, source);
                        }
                    }

                    source = source.Skip(i + 1);
                    sourceCount -= i + 1;
                    removedCount += i + 1;
                }
            }

            return new FindResult<T>();
        }
        public static IEnumerable<FindResult<T>> FindAll<T>(this IEnumerable<T> source, IEnumerable<T> what)
        {
            List<T> sourceCopy = new List<T>(source);
            List<T> whatList = what.ToList();
            List<int> positions = new List<int>();

            int removedCount = 0;
            while (sourceCopy.Count >= whatList.Count)
            {
                int i = sourceCopy.FindIndex(b => b.Equals(whatList[0]));
                if (i == -1 || (sourceCopy.Count - i) < whatList.Count)
                    break;

                int j;
                for (j = 0; j < whatList.Count; j++)
                {
                    if (!whatList[j].Equals(sourceCopy[i + j]))
                        break;
                    else if (j == whatList.Count - 1)
                    {
                        positions.Add(removedCount + i);
                        yield return new FindResult<T>(removedCount + i, source);
                    }
                }

                sourceCopy.RemoveRange(0, i + 1);
                removedCount += i + 1;
            }
        }

        public static FindResult<T> FindLast<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source
                .Select((v, i) => new { I = i, V = v })
                .Reverse()
                .Where(v => predicate(v.V))
                .Select(v => new FindResult<T>(v.I, v.V))
                .Concat(new FindResult<T>())
                .FirstItem();
        }

        #endregion

        public static IEnumerable<double> NaNToZero(this IEnumerable<double> numbers)
        {
            return numbers
                .Select(n => n.IsNaN() ? 0 : n);
        }

        public static int Count<T>(this IEnumerable<T> values, T element)
        {
            return values
                .Where(v => v.Equals(element))
                .Count();
        }
        public static bool ContainsAll<T>(this IEnumerable<T> sequence, params T[] elements)
        {
            return sequence.ContainsAll(elements.AsEnumerable());
        }
        public static bool ContainsAll<T>(this IEnumerable<T> sequence, IEnumerable<T> elements)
        {
            var remainingElements = elements.ToList();
            foreach (var value in sequence)
            {
                remainingElements.Remove(value);
            }

            return remainingElements.Count == 0;
        }

        public static List<double> Sum(this IEnumerable<double> a1, IEnumerable<double> a2)
        {
            return a1.ToList().SumSelf(a2.ToList());
        }
        public static List<double> Sub(this IEnumerable<double> a1, IEnumerable<double> a2)
        {
            return a1.ToList().SubSelf(a2.ToList());
        }
        public static List<double> Mul(this IEnumerable<double> a1, IEnumerable<double> a2)
        {
            return a1.ToList().MulSelf(a2.ToList());
        }
        public static List<double> Div(this IEnumerable<double> a1, IEnumerable<double> a2)
        {
            return a1.ToList().DivSelf(a2.ToList());
        }
        public static IEnumerable<double> InvertSignEach(this IEnumerable<double> values)
        {
            return values.Select(v => -v);
        }

        public static double Mul(this IEnumerable<double> a1)
        {
            var acc = 1D; 
            foreach (var item in a1)
            {
                acc *= item;
            }

            return acc;
        }

        #region ##### Primitives de/serialization #####

        public static List<byte> Serialize(this IEnumerable<int> values)
        {
            List<byte> serialized = new List<byte>(values.Count() * sizeof(int));
            foreach (var v in values)
            {
                serialized.AddRange(BitConverter.GetBytes(v));
            }

            return serialized;
        }
        public static List<byte> Serialize(this IEnumerable<float> values)
        {
            List<byte> serialized = new List<byte>(values.Count() * sizeof(float));
            foreach (var v in values)
            {
                serialized.AddRange(BitConverter.GetBytes(v));
            }

            return serialized;
        }
        public static List<byte> Serialize(this IEnumerable<double> values)
        {
            List<byte> serialized = new List<byte>(values.Count() * sizeof(double));
            foreach (var v in values)
            {
                serialized.AddRange(BitConverter.GetBytes(v));
            }

            return serialized;
        }

        #endregion

        #region ##### BASIC MATH OP #####

        public static IEnumerable<double> DivEach(this IEnumerable<double> numbers, double value)
        {
            return numbers.Select(n => n / value);
        }
        public static IEnumerable<double> MulEach(this IEnumerable<double> numbers, double value)
        {
            return numbers.Select(n => n * value);
        }

        public static IEnumerable<double> AddEach(this IEnumerable<double> numbers, double value)
        {
            return numbers.Select(n => n + value);
        }
        public static IEnumerable<double> SubEach(this IEnumerable<double> numbers, double value)
        {
            return numbers.Select(n => n - value);
        }

        public static IEnumerable<double> PowEach(this IEnumerable<double> sequence, double power)
        {
            return sequence.Select(e => Math.Pow(e, power));
        }
        public static IEnumerable<double> RootEach(this IEnumerable<double> sequence, double root)
        {
            return sequence.Select(e => Math.Pow(e, 1 / root));
        }

        public static IEnumerable<double> AbsEach(this IEnumerable<double> sequence)
        {
            return sequence.Select(e => Math.Abs(e));
        }
        public static IEnumerable<double> RoundEach(this IEnumerable<double> sequence, int decimals)
        {
            return sequence.Select(e => Math.Round(e, decimals));
        }

        #endregion

        #endregion

        #region ##### IParallel #####

        public static ParallelQuery<T> WithAggressiveParallelizationSetup<T>(this ParallelQuery<T> query)
        {
            return query
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .WithDegreeOfParallelism(Environment.ProcessorCount);
        }

        #endregion

        #region ##### ICollection #####

        public static bool NotContains<T>(this ICollection<T> collection, T item)
        {
            return !collection.Contains(item);
        }
        public static bool NotContains<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            return !collection.Any(predicate);
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        #endregion

        #region ##### IList #####

        static void removeRange<T>(IList<T> collection, int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                collection.RemoveAt(index);
            }
        }
        static List<T> getRange<T>(IList<T> collection, int index, int count)
        {
            List<T> range = new List<T>();
            for (int i = index; i < index + count; i++)
            {
                range.Add(collection[i]);
            }
            return range;
        }
        static void addRange<T>(IList<T> collection, IEnumerable<T> array)
        {
            foreach(T t in array)
            {
                collection.Add(t);
            }
        }

        public static T[][] SplitOnGroups<T>(this IList<T> list, int numOfGroups)
        {
            var groupSize = list.Count / numOfGroups;
            var result = new T[numOfGroups][];
            for (int i = 0; i < numOfGroups; i++)
            {
                var from = groupSize * i;
                var to = from + groupSize;
                if (i == numOfGroups - 1)
                {
                    to = list.Count; // Add the rest
                }
                var length = to - from;
                result[i] = new T[length];
                for (int k = 0; k < length; k++)
                {
                    result[i][k] = list[from + k];
                }
            }

            return result;
        }

        public static T TryGetElementOrDefault<T>(this IList<T> list, int index, T defaultValue = default)
        {
            if (index >= 0 && list.Count > index)
            {
                return list[index];
            }
            else
            {
                return defaultValue;
            }
        }

        public static T[] PullAll<T>(this IList<T> list)
        {
            var data = list.ToArray();
            list.Clear();
            
            return data;
        }

        public static IList<T> Shuffle<T>(this IList<T> list, Random random)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public static async Task AddRangeAsync<T>(this IList<T> list, IEnumerable<T> items, SemaphoreSlim listLocker)
        {
            using (await listLocker.AcquireAsync())
            {
                list.AddRange(items);
            }
        }

        public static void RemoveRange<T>(this IList<T> collection, int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                collection.RemoveAt(index);
            }
        }
        public static void RemoveTill<T>(this IList<T> collection, int index)
        {
            for (int i = 0; i < index; i++)
            {
                collection.RemoveAt(0);
            }
        }
        public static void RemoveFrom<T>(this IList<T> collection, int index)
        {
            var count = collection.Count - index;
            for (int i = 0; i < count; i++)
            {
                collection.RemoveAt(index);
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        //public static IEnumerable<object> AsEnumerable(this IList array)
        //{
        //    foreach (var item in array)
        //    {
        //        yield return item;
        //    }
        //}

        /// <summary>
        /// Returns randomly ordered <paramref name="array"/> without making changes to it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static IEnumerable<T> Shake<T>(this IList<T> array, Random random)
        {
            var indexes = random.NextUnique(0, array.Count, array.Count);
            foreach (var index in indexes)
            {
                yield return array[index];
            }
        }

        /// <summary>
        /// Returns randomly ordered <paramref name="array"/> without making changes to it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static IEnumerable<T> Shake<T>(this IEnumerable<T> array, Random random)
        {
            var cachedArray = array.MakeCached();
            var indexes = random.NextUnique(0, cachedArray.Count(), cachedArray.Count());
            foreach (var index in indexes)
            {
                yield return cachedArray.ElementAt(index);
            }
        }

        public static void RemoveLast<T>(this IList<T> array)
        {
            array.RemoveAt(array.Count - 1);
        }
        public static void RemoveFirst<T>(this IList<T> array)
        {
            array.RemoveAt(0);
        }

        public static IList<TElement> SetAllExept<TElement>(this IList<TElement> array, TElement value, params TElement[] exeptValues)
        {
            for (int i = 0; i < array.Count; i++)
            {
                var item = array[i];
                if (!exeptValues.Contains(item))
                {
                    array[i] = value;
                }
            }

            return array;
        }

        public static IList<T> Remove<T>(this IList<T> array,  Func<T, bool> predicate)
        {
            for (int i = 0; i < array.Count; i++)
            {
                var item = array[i];
                if (predicate(item))
                {
                    array.RemoveAt(i);
                    i--;
                }
            }

            return array;
        }

        public static IList<T> ForEachSelf<T>(this IList<T> collection, Func<T, T> transformer)
        {
            return ForEachSelf(collection, (i, v) => transformer(v));
        }
        public static IList<T> ForEachSelf<T>(this IList<T> collection, Func<int, T, T> transformer)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                collection[i] = transformer(i, collection[i]);
            }

            return collection;
        }

        /// <summary>
        /// Аналог побитового кольцевого сдвига 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IList<T> MoveRightSelf<T>(this IList<T> collection, int count)
        {
            if (count < 0)
            {
                throw new NotSupportedException();
            }

            return collection.MoveSelf(count);
        }

        /// <summary>
        /// Performs cyclic shift, so size keeps constant
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IList<T> MoveLeftSelf<T>(this IList<T> collection, int count)
        {
            if (count < 0)
            {
                throw new NotSupportedException();
            }

            return collection.MoveSelf(-count);
        }

        public static IList<T> MoveSelf<T>(this IList<T> collection, int count)
        {
            if (collection.Count == 0)
            {
                return collection;
            }
            else if (collection.Count < count.Abs())
            {
                throw new ArgumentOutOfRangeException();
            }
            else if (count > 0)
            {
                var tmp = new List<T>(collection);
                for (int i = 0; i < count; i++)
                {
                    collection[i] = tmp[collection.Count - count + i];
                }
                for (int i = 0; i < collection.Count - count; i++)
                {
                    collection[i + count] = tmp[i];
                }
            }
            else if (count < 0)
            {
                count = count.Abs();
                var tmp = new List<T>(collection);
                for (int i = collection.Count - count, j = 0;  i < collection.Count; i++, j++)
                {
                    collection[i] = tmp[j];
                }
                for (int i = 0; i < collection.Count - count; i++)
                {
                    collection[i] = tmp[i + count];
                }
            }

            return collection;
        }

        #region ##### Get #####

        public static ReadWriteIterator<T> GetReadWriteIterator<T>(this IList<T> collection)
            where T : struct
        {
            return new ReadWriteIterator<T>(collection);
        }

        public static List<T> GetMixed<T>(this IList<T> array)
        {
            var mixed = new List<T>(array.Count);
            Global.Random
                .NextUnique(0, array.Count, array.Count)
                .ForEach((index) => mixed.Add(array[index]));

            return mixed;
        }

        #endregion

        #region ##### Get / Set #####

        public static T FindOrDefault<T>(this IList<T> array, Func<T, bool> predicate)
        {
            foreach (T item in array)
            {
                if (predicate(item))
                {
                    return item;
                }
            }

            return default(T);
        }

        public static T GetFromEnd<T>(this IList<T> array, int index)
        {
            return array[array.Count - 1 - index];
        }

        public static T FirstElement<T>(this IList<T> array)
        {
            return array[0];
        }
        public static T LastElement<T>(this IList<T> array)
        {
            return array[array.Count - 1];
        }

        public static T FirstElementOrNew<T>(this IList<T> array)
            where T : new()
        {
            return array.Count == 0 ? new T() : array[0];
        }
        public static T LastElementOrNew<T>(this IList<T> array)
            where T : new()
        {
            return array.Count == 0 ? new T() : array[array.Count - 1];
        }
        public static T FirstElementOrDefault<T>(this IList<T> array, T defaultValue = default(T))
        {
            return array.Count == 0 ? defaultValue : array[0];
        }
        public static T LastElementOrDefault<T>(this IList<T> array, T defaultValue = default(T))
        {
            return array.Count == 0 ? defaultValue : array[array.Count - 1];
        }

        public static IList<T> Set<T>(this IList<T> array, IEnumerable<T> values, IEnumerable<int> indexes)
        {
            values = values is IList<T> ? values : values.ToArray();
            indexes = indexes is IList<int> ? indexes : indexes.ToArray();
            if (values.Count() != indexes.Count())
            {
                throw new ArgumentOutOfRangeException();
            }

            var iValue = 0;
            foreach (var i in indexes)
            {
                array[i] = values.ElementAt(iValue++);
            }

            return array;
        }
        public static IList<T> Set<T>(this IList<T> array, T value, int startIndex, int endIndex)
        {
            var indexes = ArrayUtils.CreateListByRange(startIndex, 1, endIndex - startIndex);
            return array.Set(value, indexes.ToArray());
        }
        public static IList<T> Set<T>(this IList<T> array, T value, params int[] indexes)
        {
            return array.Set(value, (IEnumerable<int>)indexes);
        }
        public static IList<T> Set<T>(this IList<T> array, T value, IEnumerable<int> indexes)
        {
            foreach (var i in indexes)
            {
                array[i] = value;
            }

            return array;
        }
        public static IList<T> SetExchange<T>(this IList<T> array, T newValue, params T[] oldValues)
        {
            for (int i = 0; i < oldValues.Length; i++)
            {
                var valToSet = oldValues[i];
                for (int j = 0; j < array.Count; j++)
                {
                    var val = array[j];
                    if (val.Equals(valToSet))
                    {
                        array[j] = newValue;
                    }
                }
            }

            return array;
        }

        public static IList<T> SetAll<T>(this IList<T> array, T value)
        {
            for (int i = 0; i < array.Count; i++)
            {
                array[i] = value;
            }

            return array;
        }

        public static IList<T> SetAll<T>(this IList<T> array, Func<T> valueFactory)
        {
            for (int i = 0; i < array.Count; i++)
            {
                array[i] = valueFactory();
            }

            return array;
        }

        #endregion

        #region ##### Sum~ Sub~ Mul~ Div~ InvertSignEach~ Self #####

        public static T SumSelf<T>(this T a1, T a2)
            where T : IList<double>
        {
            return a1.sumSelf<T, T>(a2);
        }
        public static T SubSelf<T>(this T a1, T a2)
            where T : IList<double>
        {
            return a1.subSelf<T, T>(a2);
        }
        public static T MulSelf<T>(this T a1, T a2)
            where T : IList<double>
        {
            return a1.mulSelf<T, T>(a2);
        }
        public static T DivSelf<T>(this T a1, T a2)
            where T : IList<double>
        {
            return a1.divSelf<T, T>(a2);
        }
       
        static T1 sumSelf<T1, T2>(this T1 a1, T2 a2)
            where T1 : IList<double>
            where T2 : IList<double>
        {
            for (int i = 0; i < a1.Count; i++)
            {
                a1[i] += a2[i];
            }

            return a1;
        }
        static T1 subSelf<T1, T2>(this T1 a1, T2 a2)
            where T1 : IList<double>
            where T2 : IList<double>
        {
            for (int i = 0; i < a1.Count; i++)
            {
                a1[i] -= a2[i];
            }

            return a1;
        }
        static T1 mulSelf<T1, T2>(this T1 a1, T2 a2)
            where T1 : IList<double>
            where T2 : IList<double>
        {
            for (int i = 0; i < a1.Count; i++)
            {
                a1[i] *= a2[i];
            }

            return a1;
        }
        static T1 divSelf<T1, T2>(this T1 a1, T2 a2)
            where T1 : IList<double>
            where T2 : IList<double>
        {
            for (int i = 0; i < a1.Count; i++)
            {
                a1[i] /= a2[i];
            }

            return a1;
        }

        public static T InvertSignEachSelf<T>(this T values)
            where T : IList<double>
        {
            for (int i = 0; i < values.Count; i++)
            {
                values[i] *= -1;
            }

            return values;
        }

#endregion

        public static IList<T> TruncateByCount<T>(this IList<T> array, int count, bool throwIfShorter = true)
        {
            int collectionCount = array.Count;
            ThrowUtils.ThrowIf_Negative(count);
            ThrowUtils.ThrowIf_True(count > collectionCount && throwIfShorter, "Truncate operation error: count > collectionCount ({0} > {1})".Format(count, collectionCount));

            if (collectionCount > count && collectionCount != 0)
            {
                removeRange(array, count, collectionCount - count);
            }

            return array;
        }

        #region ##### AddXXX like methods #####

        public static IList<T> Add<T>(this IList<T> array, T element, int count)
        {
            ThrowUtils.ThrowIf_Negative(count);

            for (int i = 0; i < count; i++)
            {
                array.Add(element);
            }

            return array;
        }
        public static IList<T> Add<T>(this IList<T> array, T element)
        {
            return array.Add(element, 1);
        }
        public static IList<T> AddRange<T>(this IList<T> array, params T[] elements)
        {
            addRange(array, elements);
            return array;
        }

        #endregion

        #region ##### GetRange like methods #####

        public static IList<T> Min<T>(this IList<T> array, Func<IList<T>, double> aggregator, int subarrayLength)
        {
            return array
                .Max(subArr => -aggregator(subArr), subarrayLength);
        }
        public static IList<T> Max<T>(this IList<T> array, Func<IList<T>, double> aggregator, int subarrayLength)
        {
            ThrowUtils.ThrowIf_Negative_Zero(subarrayLength);
            ThrowUtils.ThrowIf_NullArgument(aggregator);
            if (array.Count == 0 || array.Count <= subarrayLength)
                return array;

            Dictionary<int, double> index_aggregation_pair = new Dictionary<int, double>();
            for (int i = 0; i < array.Count - subarrayLength; i++)
            {
                double aggregation = aggregator(array.SubArray(i, subarrayLength));
                index_aggregation_pair.Add(i, aggregation);
            }

            int maxIndex = index_aggregation_pair.Max(kvp => kvp.Key);
            return array.SubArray(maxIndex, subarrayLength);
        }

        public static List<T> SubArray<T>(this IList<T> array, int startIndex, int subarrayLength)
        {
            ThrowUtils.ThrowIf_Negative(startIndex, subarrayLength);
            ThrowUtils.ThrowIf_Negative_Zero(subarrayLength);
            ThrowUtils.ThrowIf_True(array.Count - startIndex < subarrayLength, "Cannot extract subarray because of collection length smaller ({0}) than last index ({1})".Format(array.Count, startIndex + subarrayLength));
            if (subarrayLength == 0 || array.Count == 0)
            {
                return new List<T>();
            }

            List<T> subArray = new List<T>();
            for (int i = 0; i < subarrayLength; i++)
            {
                T val = array[startIndex + i];
                subArray.Add(array[startIndex + i]);
            }

            return subArray;
        }
        public static List<T> SubArray<T>(this IList<T> array, IList<int> indexes)
        {
            if (indexes.Count == 0 || array.Count == 0)
            {
                return new List<T>();
            }

            List<T> subArray = new List<T>(indexes.Count);
            for (int i = 0; i < indexes.Count; i++)
            {
                T val = array[indexes[i]];
                subArray.Add(val);
            }

            return subArray;
        }
        public static List<T> SubArray<T>(this IList<T> array, IEnumerable<int> indexes)
        {
            List<T> subArray = new List<T>();
            foreach (int i in indexes)
            {
                T val = array[i];
                subArray.Add(val);
            }

            return subArray;
        }

        public static IList<T> FirstHalf<T>(this IList<T> array)
        {
            ThrowUtils.ThrowIf_True(array.Count % 2 != 0, "Cannot get half of array if its length odd");

            if (array.Count == 0)
            {
                return array;
            }

            return getRange(array, 0, array.Count / 2);
        }

        public static List<List<T>> SubArrays<T>(this IList<T> source, int groupLength)
        {
            return source.SubArrays(groupLength, false);
        }

        // Хорошо работает
        public static List<List<T>> SubArrays<T>(this IList<T> source, int groupLength, bool reverseGroups)
        {
            ThrowUtils.ThrowIf_Negative_Zero(groupLength);
            ThrowUtils.ThrowIf_ZeroLength(source);
            ThrowUtils.ThrowIf_True(source.Count % groupLength != 0, "Cannot separate array by groups if it's not contains whole number of groups");

            List<List<T>> groups = new List<List<T>>();

            for (int i = 0; i < source.Count; i += groupLength)
            {
                List<T> group = getRange(source, i, groupLength);

                if (reverseGroups)
                {
                    group.Reverse();
                }
                groups.Add(group);
            }

            return groups;
        }

        public static List<T> Cut<T>(this IList<T> array, int startIndex, int cutoffLength)
        {
            List<T> area = getRange(array, startIndex, cutoffLength);
            removeRange(array, startIndex, cutoffLength);
            return area;
        }

        #endregion

        public static bool Contains<T>(this IList<T> source, IList<T> what)
        {
            return Find(source, what).Found;
        }

        public static T Replace<T, TElement>(this T array, TElement oldValue, TElement newValue)
            where T : IList<object>
        {
            for (int i = 0; i < array.Count; i++)
            {
                var v = array[i];
                array[i] = v.Equals(oldValue) ? newValue : v;
            }

            return array;
        }

        #region ##### BASIC MATH OP #####

        public static T DivEachSelf<T>(this T numbers, double value)
            where T : IList<double>
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                numbers[i] /= value;
            }

            return numbers;
        }
        public static T MulEachSelf<T>(this T numbers, double value)
            where T : IList<double>
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                numbers[i] *= value;
            }

            return numbers;
        }

        public static T AddEachSelf<T>(this T numbers, double value)
            where T : IList<double>
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                numbers[i] += value;
            }

            return numbers;
        }
        public static T SubEachSelf<T>(this T numbers, double value)
            where T : IList<double>
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                numbers[i] -= value;
            }

            return numbers;
        }

        public static T PowEachSelf<T>(this T numbers, double power)
            where T : IList<double>
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                numbers[i] = Math.Pow(numbers[i], power);
            }

            return numbers;
        }
        public static T RootEachSelf<T>(this T numbers, double root)
            where T : IList<double>
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                numbers[i] = Math.Pow(numbers[i], 1 / root);
            }

            return numbers;
        }

        public static T AbsEachSelf<T>(this T numbers)
            where T : IList<double>
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                numbers[i] = Math.Abs(numbers[i]);
            }

            return numbers;
        }
        public static T RoundEachSelf<T>(this T numbers, int decimals)
            where T : IList<double>
        {
            for (int i = 0; i < numbers.Count; i++)
            {
                numbers[i] = Math.Round(numbers[i], decimals);
            }

            return numbers;
        }

        #endregion

        #endregion

        #region ##### IDictionary #####

        public static IDictionary<TKey, TValue> EnsureKeyExists<TKey, TValue>(this IDictionary<TKey, TValue> sequence, TKey key, TValue initializationValue)
        {
            if (!sequence.ContainsKey(key))
            {
                sequence[key] = initializationValue;
            }

            return sequence;
        }

        public static TValue TryGetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> sequence, TKey key, 
            TValue defaultValue = default)
        {
            var isFound = sequence.TryGetValue(key, out TValue value);

            return isFound 
                ? value 
                : defaultValue;
        }

        public static IDictionary<TKey, TValue> ForEach<TKey, TValue>(this IDictionary<TKey, TValue> sequence, Action<TKey, TValue> action)
        {
            foreach (var e in sequence)
            {
                action(e.Key, e.Value);
            }

            return sequence;
        }
        public static IDictionary<TKey, TValue> ForEach<TKey, TValue>(this IDictionary<TKey, TValue> sequence, Action<TKey, TValue, int> action)
        {
            int index = 0;

            foreach (var e in sequence)
            {
                action(e.Key, e.Value, index);
                index++;
            }

            return sequence;
        }

        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>
            (this IDictionary<TKey, TValue> sourceDictonary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(sourceDictonary);
        }

        public static bool NotContainsKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            return !dictionary.ContainsKey(key);
        }

        public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TKey> keys)
        {
            var keysArr = keys.ToArray();
            foreach (var key in keysArr)
            {
                dictionary.Remove(key);
            }
        }

        public static void RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TKey, bool> selector)
        {
            dictionary.RemoveAll((k, v) => selector(k));
        }

        public static List<TKey> RemoveAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TKey, TValue, bool> selector)
        {
            List<TKey> toRemove = new List<TKey>();
            foreach (var kvp in dictionary)
            {
                if (selector(kvp.Key, kvp.Value))
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var key in toRemove)
            {
                dictionary.Remove(key);
            }

            return toRemove;
        }

        #endregion

        #region ##### Array #####

        /// <summary>
        /// Returns all of the elements by enumerating them from left top corner by utilizing 'Z' pattern.
        /// It is supposed that array is indexed by[row, column].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array2d"></param>
        /// <returns></returns>
        public static IEnumerable<T> Enumerate<T>(this T[,] array2d)
        {
            for (int row = 0; row < array2d.GetLength(0); row++)
            {
                for (int column = 0; column < array2d.GetLength(1); column++)
                {
                    yield return array2d[row, column];
                }
            }
        }

        public static T[][] ToJagged<T>(this T[,] array2d)
        {
            var jagged = new T[array2d.GetRowsLength()][];
            for (int rowI = 0; rowI < array2d.GetRowsLength(); rowI++)
            {
                var row = new T[array2d.GetColumnsLength()];
                for (int columnI = 0; columnI < array2d.GetColumnsLength(); columnI++)
                {
                    row[columnI] = array2d[rowI, columnI];
                }

                jagged[rowI] = row;
            }

            return jagged;
        }
        public static T[,] ToMatrix<T>(this IEnumerable<IEnumerable<T>> matrix)
        {
            var matrixAsArray = matrix.Select(r => r.ToArray()).ToArray();
            var isMatrix = matrixAsArray.Select(r => r.Length).Distinct().Count() <= 1;
            if (!isMatrix)
            {
                throw new ArgumentException("All rows must have the same length");
            }

            var matrix2d = new T[matrixAsArray.Length, matrixAsArray?.FirstElementOrDefault()?.Length ?? 0];
            for (int rowI = 0; rowI < matrix2d.GetRowsLength(); rowI++)
            {
                for (int columnI = 0; columnI < matrix2d.GetColumnsLength(); columnI++)
                {
                    matrix2d[rowI, columnI] = matrixAsArray[rowI][columnI];
                }
            }

            return matrix2d;
        }

        public static int GetColumnsLength<T>(this T[,] array2d)
        {
            return array2d.GetLength(1);
            //return array2d.GetLength(0);
        }
        public static int GetRowsLength<T>(this T[,] array2d)
        {
            return array2d.GetLength(0);
            //return array2d.GetLength(1);
        }

        /// <summary>
        /// Just casts to IList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static IList<T> AsIList<T>(this T[] array)
        {
            return array;
        }

        public static ReadOnlyCollection<T> AsReadOnly<T>(this T[] array)
        {
            return Array.AsReadOnly(array);
        }

        #endregion

        #region ##### List #####

        public static IEnumerable<T> ToEnumerable<T>(this List<T> list)
        {
            return list;
        }

        public static IList<T> AsIList<T>(this List<T> array)
        {
            return array;
        }

        #endregion
    }
}