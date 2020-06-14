using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utilities.Extensions;

namespace Utilities.Types
{
    public class SectionedStreamProxy : Stream
    {
        readonly Stream _base;
        readonly long _baseStartPosition;

        public override long Length { get; }

        public override long Position
        {
            get => _base.Position - _baseStartPosition;
            set => _base.Position = value + _baseStartPosition;
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;

        public SectionedStreamProxy(Stream baseStream, long length)
            : this(baseStream, baseStream.Position, length)
        {

        }

        SectionedStreamProxy(Stream baseStream, long position, long length)
        {
            if (baseStream.Length < position + length)
            {
                throw new ArgumentOutOfRangeException();
            }

            _base = baseStream;
            Length = length;
            _baseStartPosition = position;
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var maxCount = Length - Position;
            maxCount = maxCount < 0
                ? 0
                : maxCount;
            return _base.Read(buffer, offset, (int)Math.Min(count, maxCount));
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
