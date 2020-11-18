#nullable enable
using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace FbxTools
{
    [DebuggerDisplay("{ToString()}")]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe readonly struct RawString : IDisposable, IEquatable<RawString>
    {
        private readonly IntPtr _headPointer;
        private readonly int _byteLength;
        public readonly int ByteLength => _byteLength;
        internal readonly IntPtr Ptr => _headPointer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal RawString(int byteLength)
        {
            Debug.Assert(byteLength >= 0);

            if(byteLength == 0) {
                this = default;
            }
            else {
                //UnmanagedMemoryChecker.RegisterNewAllocatedBytes(_byteLength);
                _headPointer = Marshal.AllocHGlobal(byteLength);
                _byteLength = byteLength;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override string ToString()
        {
            if(_byteLength == 0) { return string.Empty; }
            return Encoding.UTF8.GetString((byte*)_headPointer, _byteLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Span<byte> AsSpan() => MemoryMarshal.CreateSpan(ref Unsafe.AsRef<byte>((void*)_headPointer), _byteLength);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if(_headPointer != IntPtr.Zero) {
                //UnmanagedMemoryChecker.RegisterReleasedBytes(_byteLength);
                Marshal.FreeHGlobal(_headPointer);
                Unsafe.AsRef(_headPointer) = IntPtr.Zero;     // Clear pointer into null for safety.
            }
            Unsafe.AsRef(_byteLength) = 0;
        }

        public override bool Equals(object? obj) => obj is RawString str && Equals(str);

        public bool Equals(RawString other)
        {
            return _headPointer == other._headPointer &&
                   _byteLength == other._byteLength;
        }

        public override int GetHashCode() => HashCode.Combine(_headPointer, _byteLength);
    }

    [DebuggerDisplay("{ToString()}")]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe readonly struct ReadOnlyRawString : IEquatable<ReadOnlyRawString>
    {
        private readonly RawString _rawString;
        public readonly int ByteLength => _rawString.ByteLength;

        internal ReadOnlyRawString(RawString rawString) => _rawString = rawString;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> AsSpan() => _rawString.AsSpan();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => _rawString.ToString();

        public override bool Equals(object? obj) => obj is ReadOnlyRawString str && Equals(str);

        public bool Equals(ReadOnlyRawString other) => _rawString.Equals(other._rawString);

        public override int GetHashCode() => HashCode.Combine(_rawString);
    }
}
