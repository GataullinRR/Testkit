using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace Utilities.Types
{
    public class StreamProxyBase : Stream
    {
        protected readonly Stream _base;

        public override bool CanRead => _base.CanRead;
        public override bool CanSeek => _base.CanSeek;
        public override bool CanWrite => _base.CanWrite;
        public override long Length => _base.Length;
        public override long Position
        {
            get => _base.Position;
            set => _base.Position = value;
        }

        public override bool CanTimeout => _base.CanTimeout;
        public override int ReadTimeout
        {
            get => _base.ReadTimeout;
            set => _base.ReadTimeout = value;
        }
        public override int WriteTimeout
        {
            get => _base.WriteTimeout;
            set => _base.WriteTimeout = value;
        }

        public StreamProxyBase(Stream baseStream)
        {
            _base = baseStream;
        }

        public override void Flush()
        {
            _base.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _base.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _base.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _base.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _base.Write(buffer, offset, count);
        }

        public override void Close()
        {
            _base.Close();
        }
    }
}
