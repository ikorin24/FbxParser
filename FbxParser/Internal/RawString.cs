#nullable enable
using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace FbxTools.Internal
{
    [DebuggerDisplay("{ToString()}")]
    public readonly unsafe struct RawString : IEquatable<RawString>
    {
        private readonly IntPtr _headPointer;
        private readonly int _byteLength;

        internal RawString(IntPtr ptr, int byteLen)
        {
            _headPointer = ptr;
            _byteLength = byteLen;
        }

        public override bool Equals(object? obj) => obj is RawString str && Equals(str);

        public bool Equals(RawString other) => (_headPointer == other._headPointer && _byteLength == other._byteLength) || SequenceEqual(other);

        public bool SequenceEqual(RawString other) => AsSpan().SequenceEqual(other.AsSpan());

        public bool SequenceEqual(ReadOnlySpan<byte> other) => AsSpan().SequenceEqual(other);

        public ReadOnlySpan<byte> AsSpan()
        {
#if NETSTANDARD2_0
            return new Span<byte>((void*)_headPointer, _byteLength);
#else
            return MemoryMarshal.CreateSpan(ref Unsafe.AsRef<byte>((void*)_headPointer), _byteLength);
#endif
        }

        public override int GetHashCode()
        {
            return XXHash32.ComputeHash((byte*)_headPointer, _byteLength);
        }

        public static bool operator ==(RawString left, RawString right) => left.Equals(right);

        public static bool operator !=(RawString left, RawString right) => !(left == right);

        public override string ToString()
        {
            if(_byteLength == 0) { return string.Empty; }
            return Encoding.ASCII.GetString((byte*)_headPointer, _byteLength);
        }
    }


    [DebuggerDisplay("{ToString()}")]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe readonly struct RawStringMem : IDisposable, IEquatable<RawStringMem>
    {
        private readonly IntPtr _headPointer;
        private readonly int _byteLength;
        public readonly int ByteLength => _byteLength;
        public readonly IntPtr Ptr => _headPointer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal RawStringMem(int byteLength)
        {
            Debug.Assert(byteLength >= 0);

            if(byteLength == 0) {
                this = default;
            }
            else {
                UnmanagedMemoryHelper.RegisterNewAllocatedBytes(byteLength);
                _headPointer = Marshal.AllocHGlobal(byteLength);
                _byteLength = byteLength;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override string ToString()
        {
            if(_byteLength == 0) { return string.Empty; }
            return Encoding.ASCII.GetString((byte*)_headPointer, _byteLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Span<byte> AsSpan()
        {
#if NETSTANDARD2_0
            return new Span<byte>((void*)_headPointer, _byteLength);
#else
            return MemoryMarshal.CreateSpan(ref Unsafe.AsRef<byte>((void*)_headPointer), _byteLength);
#endif
        }

        public RawString AsRawString() => new RawString(_headPointer, _byteLength);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if(_headPointer != IntPtr.Zero) {
                UnmanagedMemoryHelper.RegisterReleasedBytes(_byteLength);
                Marshal.FreeHGlobal(_headPointer);
                Unsafe.AsRef(_headPointer) = IntPtr.Zero;     // Clear pointer into null for safety.
            }
            Unsafe.AsRef(_byteLength) = 0;
        }

        public override bool Equals(object? obj) => obj is RawStringMem str && Equals(str);

        public bool Equals(RawStringMem other)
        {
            return _headPointer == other._headPointer &&
                   _byteLength == other._byteLength;
        }

        public override int GetHashCode() => HashCode.Combine(_headPointer, _byteLength);

        public static implicit operator RawString(RawStringMem str) => str.AsRawString();
    }
}
