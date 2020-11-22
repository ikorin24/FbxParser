#nullable enable
using System;
using System.IO;

namespace FbxTools.Internal
{
    internal unsafe sealed class MemoryStreamUM : Stream
    {
        private byte* _ptr;
        private long _length;
        private long _pos;

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => _length;

        public override long Position
        {
            get => _length;
            set => _length = value;
        }

        public MemoryStreamUM(byte* ptr, int length)
        {
            _ptr = ptr;
            _length = length;
            _pos = 0;
        }

        internal MemoryStreamUM RefreshInstance(byte* ptr, int length)
        {
            _ptr = ptr;
            _length = length;
            _pos = 0;
            return this;
        }

        public override void Flush()
        {
            // nop
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Read(buffer.AsSpan(offset, count));
        }

        public override int Read(Span<byte> buffer)
        {
            var readLen = (int)Math.Min(buffer.Length, Math.Max(0, _length - _pos));
            new Span<byte>(_ptr + _pos, readLen).CopyTo(buffer);
            _pos += readLen;
            return readLen;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch(origin) {
                case SeekOrigin.Begin:
                    _pos = offset;
                    break;
                case SeekOrigin.Current:
                    _pos += offset;
                    break;
                case SeekOrigin.End:
                    if(offset > _length) { throw new ArgumentOutOfRangeException(); }
                    _pos = _length + offset;
                    break;
                default:
                    throw new ArgumentException();
            }
            return _pos;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            _ptr = default;
            _length = default;
            _pos = default;
            base.Dispose(disposing);
        }
    }
}
