using System;
using System.Collections.Generic;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace Utilities
{
    public static class InteropUtils
    {
        public static int SizeOf(Type type)
        {
            if (!type.IsPrimitive)
            {
                throw new InconsistentTypeArgumentException();
            }

            if (type == typeof(char))
            {
                return sizeof(char);
            }
            else
            {
                return Marshal.SizeOf(type);
            }
        }

        public static Func<Stream, object> GetDeserializer(Type type)
        {
            return GetDeserializer(type, false);
        }
        public static Func<Stream, object> GetDeserializer(Type type, bool inverseByteOrder)
        {
            if (!type.IsPrimitive)
            {
                throw new InconsistentTypeArgumentException();
            }

            if (type == typeof(sbyte) || type == typeof(byte))
            {
                return stream => (byte)stream.ReadByte();
            }
            else
            {
                var numOfBytes = SizeOf(type);
                return stream => typeof(BitConverter)
                    .GetMethod("To" + type.Name)
                    .Invoke(null, new object[]
                    {
                        inverseByteOrder ? stream.Read(numOfBytes).Reverse().ToArray() : stream.Read(numOfBytes),
                        0
                    });
            }
        }

        public static T[] DeserializeAsArrayOfType<T>(int arrayLength, Stream source)
            where T : struct
        {
            return DeserializeAsArrayOfType<T>(arrayLength, source, BitConverter.IsLittleEndian);
        }
        public static T[] DeserializeAsArrayOfType<T>(Stream source, bool isLittleEndian)
            where T : struct
        {
            var size = source.Length / (double)SizeOf(typeof(T));
            if (!size.IsInteger())
            {
                throw new InvalidOperationException();
            }

            return DeserializeAsArrayOfType<T>(size.ToInt32(), source, isLittleEndian);
        }
        public static T[] DeserializeAsArrayOfType<T>(int arrayLength, Stream source, bool isLittleEndian)
            where T : struct
        {
            var inverseByteOrder = isLittleEndian != BitConverter.IsLittleEndian;
            var deserializer = GetDeserializer(typeof(T), inverseByteOrder);
            var array = new List<T>();
            for (int i = 0; i < arrayLength; i++)
            {
                array.AddRange((T)deserializer(source));
            }

            return array.ToArray();
        }

        /// <summary>
        /// Работает только для полей примитивных типов. Остальные - игнорирует.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="serializedValues"></param>
        public static void InitializeAllFieldsSequentially<T>(T target, Stream serializedValues)
        {
            InitializeAllFieldsSequentially(target, serializedValues, null, null);
        }
        public static void InitializeAllFieldsSequentially<T>
            (T target, Stream serializedValues, string from, string to)
        {
            foreach (var field in enumerateInstanceFieldsSequentially(typeof(T), from, to))
            {
                setValue(field);
            }

            return;

            void setValue(FieldInfo fi)
            {
                var fieldType = fi.FieldType;
                object value = null;
                if (fieldType.IsPrimitive)
                {
                    value = GetDeserializer(fieldType)(serializedValues);
                    fi.SetValue(target, value);
                }
            }
        }

        class IgnoreMarker { }
        /// <summary>
        /// Учитывается при вызове <see cref="InitializeAllFieldsSequentiallyFromValues"/>
        /// </summary>
        public static readonly object IGNORE_MARKER = new IgnoreMarker();
        public static void InitializeAllFieldsSequentiallyFromValues<T>(T target, params object[] values)
        {
            var type = typeof(T);
            var fields = enumerateInstanceFieldsSequentially(type)
                .ForEach(setValue);

            return;

            void setValue(FieldInfo fi, int i)
            {
                var value = values[i];
                if (value != IGNORE_MARKER)
                {
                    var fieldType = fi.FieldType;
                    var valueType = value?.GetType();
                    if (fieldType != valueType)
                    {
                        if (fieldType.IsPrimitive && (valueType?.IsPrimitive ?? false))
                        {
                            value = Convert.ChangeType(value, fieldType);
                        }
                    }
                    fi.SetValue(target, value);
                }
            }
        }

        public static IEnumerable<byte> SerializeAll(params object[] primitives)
        {
            return serializeAll(true, primitives);
        }
        public static IEnumerable<byte> SerializeAll(bool isLittleEndian, params object[] primitives)
        {
            return serializeAll(true, isLittleEndian, primitives);
        }
        static IEnumerable<byte> serializeAll(bool throwIfNotPrimitive, params object[] primitives)
        {
            return serializeAll(throwIfNotPrimitive, BitConverter.IsLittleEndian, primitives);
        }
        static IEnumerable<byte> serializeAll(bool throwIfNotPrimitive, bool isLittleEndian, params object[] primitives)
        {
            var writer = new BinaryWriter(new MemoryStream());
            long lastPosition = 0;
            var inverseByteOrder = isLittleEndian != BitConverter.IsLittleEndian;

            return serialize(primitives);

            IEnumerable<byte> serialize(object[] values)
            {
                foreach (var value in values)
                {
                    var valueType = value.GetType();
                    if (valueType.IsArray)
                    {
                        var valueValues = new List<object>();
                        foreach (var v in (Array)value)
                        {
                            if (v.GetType().IsArray)
                            {
                                serialize(new object[] { v });
                            }
                            else
                            {
                                valueValues.Add(v);
                            }
                        }

                        foreach (var b in serialize(valueValues.ToArray()))
                        {
                            yield return b;
                        }
                    }
                    else if (!valueType.IsPrimitive)
                    {
                        if (throwIfNotPrimitive)
                        {
                            throw new ArgumentException();
                        }
                    }
                    else
                    {
                        dynamic dValue = value;
                        if (valueType == typeof(char))
                        {
                            dValue = (short)(char)value;
                        }

                        writer.Write(dValue);
                        var serialized = inverseByteOrder
                            ? getBytes().Reverse()
                            : getBytes();
                        foreach (var b in serialized)
                        {
                            yield return b;
                        }

                        IEnumerable<byte> getBytes()
                        {
                            writer.BaseStream.Seek(lastPosition, SeekOrigin.Begin);
                            for (long i = lastPosition; i < writer.BaseStream.Length; i++)
                            {
                                yield return (byte)writer.BaseStream.ReadByte();
                            }
                            lastPosition = writer.BaseStream.Length;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Работает только для публичных полей примитивных типов и массивов примитивных типов. Остальные - игнорирует.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IEnumerable<byte> SerializeAllPrimitiveFieldsSequentially<T>(T target)
        {
            return SerializeAllPrimitiveFieldsSequentially(target, null, null);
        }
        public static IEnumerable<byte> SerializeAllPrimitiveFieldsSequentially<T>(T target, string from, string to)
        {
            var type = typeof(T);
            foreach (var fi in enumerateInstanceFieldsSequentially(type, from, to))
            {
                if (fi.FieldType.IsPrimitive || fi.FieldType.IsArray)
                {
                    dynamic value = fi.GetValue(target);
                    foreach (var b in serializeAll(false, value))
                    {
                        yield return b;
                    }
                }
            }
        }

        static IEnumerable<FieldInfo> enumerateInstanceFieldsSequentially(Type type)
        {
            return enumerateInstanceFieldsSequentially(type, null, null);
        }
        static IEnumerable<FieldInfo> enumerateInstanceFieldsSequentially(Type type, string from, string to)
        {
            // https://stackoverflow.com/questions/8067493/if-getfields-doesnt-guarantee-order-how-does-layoutkind-sequential-work

            var fields = type.GetFields()
                .OrderBy(f => f.MetadataToken)
                .Where(fi => !fi.IsStatic)
                .SkipWhile(fi => fi.Name != from && from != null).ToArray();
            var endI = to == null
                ? fields.Length - 1
                : fields.Find(fi => fi.Name == to).Index;

            for (int i = 0; i <= endI; i++)
            {
                yield return fields[i];
            }
        }
    }
}
