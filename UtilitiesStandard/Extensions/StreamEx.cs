using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Extensions
{
    public enum StreamAccess
    {
        READ,
        WRITE,
        SEEK,
        TIMEOUT
    }

    public static class StreamEx
    {
        #region ##### To #####

        public static MemoryStream ToStream(this IEnumerable<byte> data)
        {
            return new MemoryStream(data.ToArray());
        }

        public static StreamReader ToStreamReader(this Stream stream)
        {
            return new StreamReader(stream);
        }

        public static StreamWriter ToStreamWriter(this Stream stream, Encoding encoding)
        {
            return new StreamWriter(stream, encoding);
        }

        public static BinaryWriter ToBinaryWriter(this Stream stream)
        {
            return new BinaryWriter(stream);
        }

        #endregion

        #region ##### Stream #####

        class StreamEnumerable : IEnumerable<byte>
        {
            class StreamEnumerator : Disposable, IEnumerator<byte>
            {
                readonly Stream _stream;
                int _position = 0;
                byte? _current = null;
                public byte Current
                {
                    get
                    {
                        throwIfDisposed();

                        if (_current == null)
                        {
                            throw new InvalidOperationException();
                        }
                        else
                        {
                            return _current.Value;
                        }
                    }
                }
                object IEnumerator.Current => Current;

                public StreamEnumerator(Stream stream)
                {
                    _stream = stream ?? throw new ArgumentNullException(nameof(stream));
                }

                public bool MoveNext()
                {
                    throwIfDisposed();

                    lock (_stream)
                    {
                        if (_stream.Length > _position)
                        {
                            var oldPosition = _stream.Position;
                            try
                            {
                                _stream.Position = _position;
                                _current = _stream.ReadByte().ToByte();

                                return true;
                            }
                            finally
                            {
                                _position++;
                                _stream.Position = oldPosition;
                            }
                        }
                        else
                        {
                            _current = null;

                            return false;
                        }
                    }
                }

                public void Reset()
                {
                    throwIfDisposed();

                    _position = 0;
                    _current = null;
                }

                protected override void DisposeManagedState()
                {

                }
            }

            readonly Stream _stream;

            public StreamEnumerable(Stream stream)
            {
                _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            }

            public IEnumerator<byte> GetEnumerator()
            {
                return new StreamEnumerator(_stream);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static IEnumerable<byte> AsEnumerable(this Stream stream)
        {
            return new StreamEnumerable(stream);
        }

        public static async Task WriteToAsync(this Stream origin, Stream destination, CancellationToken cancellation)
        {
            await destination.WriteAsync(originData(), cancellation);

            IEnumerable<byte> originData()
            {
                var value = origin.ReadByte();
                while (value != -1)
                {
                    cancellation.ThrowIfCancellationRequested();

                    yield return (byte)value;

                    value = origin.ReadByte();
                }
            }
        }

        public static void EnsureAccess(this Stream stream, params StreamAccess[] streamAccesses)
        {
            foreach (var access in streamAccesses)
            {
                switch (access)
                {
                    case StreamAccess.READ:
                        if (!stream.CanRead)
                        {
                            throw new NotSupportedException();
                        }
                        break;
                    case StreamAccess.WRITE:
                        if (!stream.CanWrite)
                        {
                            throw new NotSupportedException();
                        }
                        break;
                    case StreamAccess.SEEK:
                        if (!stream.CanSeek)
                        {
                            throw new NotSupportedException();
                        }
                        break;
                    case StreamAccess.TIMEOUT:
                        if (!stream.CanTimeout)
                        {
                            throw new NotSupportedException();
                        }
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public static async Task<FileStream> OpenNewReadOnlyStreamAsync(this FileStream stream)
        {
            await ThreadingUtils.ContinueAtThreadPull();

            return new FileStream(stream.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        public static void Write(this Stream stream, IEnumerable<byte> buffer, int timeout)
        {
            var oldTimeout = stream.WriteTimeout;
            stream.WriteTimeout = timeout;
            try
            {
                stream.Write(buffer);
            }
            finally
            {
                stream.WriteTimeout = oldTimeout;
            }
        }
        public static void Write(this Stream stream, IEnumerable<byte> buffer)
        {
            var asArray = buffer.ToArray();
            stream.Write(asArray, 0, asArray.Length);
        }
        public static void WriteAndDispose(this Stream stream, IEnumerable<byte> buffer)
        {
            using (stream)
            {
                stream.Write(buffer);
            }
        }

        public static async Task WriteAsync(this Stream stream, IEnumerable<byte> buffer, int timeout)
        {
            var oldTimeout = stream.WriteTimeout;
            stream.WriteTimeout = timeout;
            try
            {
                await stream.WriteAsync(buffer);
            }
            finally
            {
                stream.WriteTimeout = oldTimeout;
            }
        }
        public static async Task WriteAsync(this Stream stream, IEnumerable<byte> buffer)
        {
            await stream.WriteAsync(buffer, CancellationToken.None);
        }
        public static async Task WriteAsync
            (this Stream stream, IEnumerable<byte> buffer, CancellationToken cancellation)
        {
            var asArray = buffer.ToArray();
            await stream.WriteAsync(asArray, 0, asArray.Length, cancellation);
        }

        public static async Task<byte[]> ReadToEndAsync(this Stream stream)
        {
            var count = stream.Length - stream.Position;
            var buffer = new byte[count];
            var read = await stream.ReadAsync(buffer, (int)stream.Position, (int)count);
            if (read != count)
            {
                buffer = buffer.SubArray(0, read).ToArray();
            }

            return buffer;
        }
        //public static byte[] ReadToEnd(this Stream stream)
        //{
        //    var count = stream.Length - stream.Position;
        //    var buffer = new byte[count];
        //    var read = stream.Read(buffer, (int)stream.Position, (int)count);
        //    if (read != count)
        //    {
        //        buffer = buffer.SubArray(0, read).ToArray();
        //    }

        //    return buffer;
        //}
        public static byte[] ReadToEnd(this Stream stream)
        {
            return stream.Read((int)(stream.Length - stream.Position));
        }
        public static byte[] Read(this Stream stream, int count, bool throwIfLengthInvalid = true)
        {
            var buffer = new byte[count];
            var bytesRead = stream.Read(buffer, 0, count);
            if (bytesRead != count && throwIfLengthInvalid)
            {
                throw new InvalidOperationException("bytesRead != count");
            }

            return buffer;
        }
        public static async Task<byte[]> ReadAsync(this Stream stream, int count, bool throwIfLengthInvalid = true)
        {
            var buffer = new byte[count];
            var bytesRead = await stream.ReadAsync(buffer, 0, count);
            if (bytesRead != count && throwIfLengthInvalid)
            {
                throw new InvalidOperationException("bytesRead != count");
            }

            return buffer;
        }

        #region ##### ReadExact* #####

        public static async Task<byte[]> ReadExactAsync(this Stream stream, int count, ReadExactInfo info,
            CancellationToken cancellation)
        {
            return await readExactAsync(count, (a, b, c) => stream.ReadAsync(a, b, c, cancellation), info);
        }

        /// <summary>
        /// В случае, когда прозошел <paramref name="methodTimeout"/>, 
        /// прочитанные на текущий момент данные могут быть найдены в свойстве 
        /// <see cref="ReadExactTimeoutException.Buffer"/>.
        /// </summary>
        /// <exception cref="ReadExactTimeoutException"></exception>
        /// <param name="stream"></param>
        /// <param name="count"></param>
        /// <param name="methodTimeout"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadExactAsync(this Stream stream, int count, int methodTimeout,
            CancellationToken cancellation)
        {
            return await readExactAsync(count, (a, b, c) => stream.ReadAsync(a, b, c, cancellation), 
                new ReadExactInfo(methodTimeout));
        }
        /// <summary>
        /// Не все потоки поддерживают <paramref name="streamTimeout"/> и <paramref name="cancellation"/>.
        /// В случае, когда прозошел <paramref name="methodTimeout"/>, 
        /// прочитанные на текущий момент данные могут быть найдены в свойстве 
        /// <see cref="ReadExactTimeoutException.Buffer"/>.
        /// </summary>
        /// <exception cref="ReadExactTimeoutException"></exception>
        /// <param name="stream"></param>
        /// <param name="count"></param>
        /// <param name="streamTimeout"></param>
        /// <param name="methodTimeout"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadExactAsync
            (this Stream stream, int count, int streamTimeout, int methodTimeout, CancellationToken cancellation)
        {
            var oldTimeout = stream.ReadTimeout;
            stream.ReadTimeout = streamTimeout;
            try
            {
                return await readExactAsync(count, (a, b, c) => stream.ReadAsync(a, b, c, cancellation),
                    new ReadExactInfo(methodTimeout));
            }
            finally
            {
                stream.ReadTimeout = oldTimeout;
            }
        }
        /// <summary>
        /// В отличии от <see cref="ReadExactAsync(Stream, int, int, int, CancellationToken)"/> 
        /// использует синхронную весию методв <see cref="Stream.ReadAsync(byte[], int, int, CancellationToken)"/> 
        /// и предоставляет свою реализацию отмены через токен. 
        /// В случае, когда прозошел <paramref name="methodTimeout"/>, 
        /// прочитанные на текущий момент данные могут быть найдены в свойстве 
        /// <see cref="ReadExactTimeoutException.Buffer"/>.
        /// </summary>
        /// <exception cref="ReadExactTimeoutException"></exception>
        /// <param name="stream"></param>
        /// <param name="count"></param>
        /// <param name="streamTimeout"></param>
        /// <param name="methodTimeout"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadExactSyncAsync
            (this Stream stream, int count, int streamTimeout, int methodTimeout, CancellationToken cancellation)
        {
            var oldTimeout = stream.ReadTimeout;
            stream.ReadTimeout = streamTimeout;
            try
            {
                return await Task.Run(async () =>
                {
                    return await readExactAsync(count, (a, b, c) =>
                    {
                        var result = stream.Read(a, b, c);
                        cancellation.ThrowIfCancellationRequested();
                        return result;
                    }, new ReadExactInfo(methodTimeout));
                }, cancellation);
            }
            finally
            {
                stream.ReadTimeout = oldTimeout;
            }
        }
        /// <summary>
        /// В случае, когда прозошел <paramref name="methodTimeout"/>, 
        /// прочитанные на текущий момент данные могут быть найдены в свойстве 
        /// <see cref="ReadExactTimeoutException.Buffer"/>.
        /// </summary>
        /// <exception cref="ReadExactTimeoutException"></exception>
        /// <param name="stream"></param>
        /// <param name="count"></param>
        /// <param name="streamTimeout"></param>
        /// <param name="methodTimeout"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadExactAsync
            (this Stream stream, int count, int streamTimeout, int methodTimeout)
        {
            var oldTimeout = stream.ReadTimeout;
            stream.ReadTimeout = streamTimeout;
            try
            {
                 return await stream.ReadExactAsync(count, methodTimeout);
            }
            finally
            {
                stream.ReadTimeout = oldTimeout;
            }
        }
        /// <summary>
        /// В случае, когда прозошел <paramref name="methodTimeout"/>, 
        /// прочитанные на текущий момент данные могут быть найдены в свойстве 
        /// <see cref="ReadExactTimeoutException.Buffer"/>.
        /// </summary>
        /// <exception cref="ReadExactTimeoutException"></exception>
        /// <param name="stream"></param>
        /// <param name="count"></param>
        /// <param name="methodTimeout"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadExactAsync(this Stream stream, int count, int methodTimeout)
        {
            return await readExactAsync(count, (a, b, c) => stream.ReadAsync(a, b, c), 
                new ReadExactInfo(methodTimeout));
        }
        public static async Task<byte[]> ReadExactAsync(this Stream stream, int count)
        {
            return await readExactAsync(count, (a, b, c) => stream.ReadAsync(a, b, c));
        }
        public static byte[] ReadExact(this Stream stream, int count, int timeout)
        {
            var oldTimeout = stream.ReadTimeout;
            stream.ReadTimeout = timeout;
            try
            {
                return stream.ReadExact(count);
            }
            finally
            {
                stream.ReadTimeout = oldTimeout;
            }
        }
        public static byte[] ReadExact(this Stream stream, int count)
        {
            return readExactAsync(count, (a, b, c) => stream.Read(a, b, c)).GetAwaiter().GetResult();
        }
        static async Task<byte[]> readExactAsync(int count, Func<byte[], int, int, object> readFunc)
        {
            return await readExactAsync(count, readFunc, new ReadExactInfo());
        }
        static async Task<byte[]> readExactAsync
            (int count, Func<byte[], int, int, object> readFunc, ReadExactInfo info)
        {
            var timeouter = new Timeouter(info.MethodTimeout);

            var buffer = new byte[count];
            var offset = 0;
            var totalBytesRead = 0;
            var remain = count;
            while (totalBytesRead != count)
            {
                var bytesRead = 0;
                var readInfo = readFunc(buffer, offset, remain);
                if (readInfo is Task<int> task)
                {
                    bytesRead = await task;
                }
                else if (readInfo is int read)
                {
                    bytesRead = read;
                }
                else
                {
                    throw new NotSupportedException();
                }
                totalBytesRead += bytesRead;
                offset = totalBytesRead;
                remain = count - totalBytesRead;

                if (bytesRead == 0 && info.ThrowIfZeroReceived)
                {
                    throw new ReadExactZeroReceivedException();
                }
                if (info.RetryDelay != 0)
                {
                    await Task.Delay(info.RetryDelay).ConfigureAwait(false);
                }
                if ((info.TimeoutOnlyIfNothingWasRead && totalBytesRead == 0) 
                    || (!info.TimeoutOnlyIfNothingWasRead))
                {
                    timeouter.ThrowIfTimeout(() => new ReadExactTimeoutException(buffer.Take(totalBytesRead), count));
                }
            }

            return buffer;
        }

        #endregion

        #endregion

        #region ##### StreamReader #####

        public static void SkipLines(this StreamReader reader, int count)
        {
            for (int i = 0; i < count; i++)
            {
                reader.ReadLine();
            }
        }

        /// <summary>
        /// Changes the <see cref="Stream.Position"/> during enumeration!
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public static IEnumerable<string> ReadAllLines(this StreamReader sr)
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                yield return line;
            }
        }

        /// <summary>
        /// Changes the <see cref="Stream.Position"/> during enumeration!
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public static IEnumerable<char> ReadAllText(this StreamReader sr)
        {
            int ch;
            while ((ch = sr.Read()) != -1)
            {
                yield return (char)ch;
            }
        }

        #endregion

        #region ##### StreamWriter #####

        public static void WriteLines(this StreamWriter writer, IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
        }

        public static void WriteAndDispose(this StreamWriter writer, string text)
        {
            using (writer)
            {
                writer.Write(text);
            }
        }

        #endregion

        #region ##### NetworkStream #####

        public static async Task<byte[]> ReadToEndAsync(this NetworkStream stream)
        {
            var result = new List<byte>();
            var buffer = new byte[1000];
            int bytesRead = 0;
            do
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                result.AddRange(buffer.GetRangeSafe(0, bytesRead));
            }
            while (bytesRead == buffer.Length);

            return result.ToArray();
        }

        #endregion

        #region ##### MemoryStream #####

        public static bool IsFixedSize(this MemoryStream stream)
        {
            return !(bool)typeof(MemoryStream)
                    .GetField("_expandable",
                         BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    .GetValue(stream);
        }

        #endregion

        #region ##### FileStream #####

        /// <summary>
        /// Enumerates from <see cref="Stream.Position"/> to <see cref="Stream.Length"/>.
        /// Advances stream position!
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static IEnumerable<byte> EnumerateTillEnd(this FileStream stream)
        {
            for (long i = stream.Position; i < stream.Length; i++)
            {
                yield return (byte)stream.ReadByte();
            }
        }

        #endregion

    }
}
