using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace Utilities.Extensions
{
    public static class CommonEx
    {
        class Parser<T> where T : struct
        {
            readonly Func<string, T?> _parser;

            public Parser(Func<string, T?> parser)
            {
                _parser = parser;
            }

            public bool TryParse(string value, out T modelValue)
            {
                var raw = _parser(value);
                modelValue = raw.HasValue ? raw.Value : default;

                return raw.HasValue;
            }
        }

        public static IEnumerable<T> Repeat<T>(this T value, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return value;
            }
        }

        public delegate bool TryParseDelegate<T>(string serialized, out T value);
        public static TryParseDelegate<T> ToOldTryParse<T>(this Func<string, T?> tryParse)
            where T : struct
        {
            return new Parser<T>(tryParse).TryParse;
        }

        public static bool IsTrue<T>(this T value, Func<T, bool> predicate)
        {
            return predicate(value);
        }

        public static T IfOrDefault<T>(this T value, bool condition) 
            where T : class
        {
            return condition 
                ? value 
                : default;
        }

        /// <summary>
        /// Returns single value as single value sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToSequence<T>(this T value)
        {
            yield return value;
        }

        /// <summary>
        /// Works only for Enums!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns><see cref="null"/> if there is no description attribute</returns>
        public static string GetEnumValueDescription<T>(this T value)
            where T : struct
        {
            var t = typeof(T);
            if (t.IsEnum)
            {
                var memInfo = t.GetMember(value.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                return (attributes.SingleOrDefault() as DescriptionAttribute)?.Description;
            }
            else
            {
                throw new NotSupportedException("This method supposed to be used with Enums!");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns>[0; <paramref name="count"/>)</returns>
        public static IEnumerable<int> Range(this int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return i;
            }
        }
        public static IEnumerable<uint> Range(this uint count)
        {
            for (uint i = 0; i < count; i++)
            {
                yield return i;
            }
        }
        public static IEnumerable<int> RangeFromTo(this int from, int to)
        {
            for (int i = from; i < to; i++)
            {
                yield return i;
            }
        }
        public static IEnumerable<int> RangeToFrom(this int to, int from)
        {
            for (int i = from; i < to; i++)
            {
                yield return i;
            }
        }

        public static IEnumerable<T> Times<T>(this int count, Func<int, T> generator)
        {
            for (int i = 0; i < count; i++)
            {
                yield return generator(i);
            }
        }

        public static bool NullToFalse(this object value)
        {
            return value != null;
        }
        public static bool NullToFalse(this bool? value)
        {
            return value == null
                ? false 
                : value.Value;
        }

        public static object DefaultToString<T>(this T value, string stringValue)
        {
            return ReferenceEquals(value, default(T))
                ? (object)stringValue
                : value;
        }

        public static bool IsOneOf(this string value, StringComparison stringComparison, params string[] values)
        {
            foreach (var v in values.NullToEmpty())
            {
                if (string.Equals(value, v, stringComparison))
                {
                    return true;
                }
            }

            return false;
        }
        public static bool IsOneOf<T>(this T value, params T[] values)
        {
            foreach (T v in values)
            {
                if (Equals(value, v))
                {
                    return true;
                }
            }

            return false;
        }

        public static T To<T>(this object obj)
        {
            return (T)obj;
        }
        
        public static T As<T>(this object obj)
            where T : class
        {
            return obj as T;
        }

        public static TOut NullTernar<TIn, TOut>(this TIn obj, TOut nullResult, Func<TIn, TOut> notNullResult)
        {
            return obj == null
                ? nullResult
                : notNullResult(obj);
        }

        public static T Ternar<T>(this bool condition, T trueResult, T falseResult)
        {
            return condition ? trueResult : falseResult;
        }

        public static T MakeDeepClone<T>(this T obj)
        {
            var serializer = new BinaryFormatter();
            var stream = new MemoryStream();
            serializer.Serialize(stream, obj);
            stream.Position = 0;

            return (T)serializer.Deserialize(stream);
        }

        /// <summary>
        /// Uses <see cref="BinaryFormatter"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<byte> Serialize<T>(this T obj)
        {
            var serializer = new BinaryFormatter();
            var stream = new MemoryStream();
            serializer.Serialize(stream, obj);

            return stream.ToArray();
        }
        /// <summary>
        /// Uses<see cref="BinaryFormatter"/>
        /// </summary>
        /// <typeparam name = "T" ></ typeparam >
        /// < param name="obj"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this IEnumerable<byte> obj)
        {
            var serializer = new BinaryFormatter();
            var stream = new MemoryStream(obj.ToArray());

            return (T)serializer.Deserialize(stream);
        }
        /// <summary>
        /// Uses <see cref="BinaryFormatter"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this Stream obj)
        {
            var serializer = new BinaryFormatter();

            return (T)serializer.Deserialize(obj);
        }

        public static IEnumerable<T> Select<T>(this object obj, params (Func<object, bool> predicate, T result)[] conditions)
        {
            foreach (var c in conditions)
            {
                if(c.predicate(obj))
                {
                    yield return c.result;
                }
            }
        }

        public static T CastTo<T>(this object obj)
        {
            return (T)obj;
        }
    }
}
