#nullable enable
using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace FbxTools.Internal
{
    [DebuggerDisplay("{ToString()}")]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe readonly struct RawString : IDisposable, IEquatable<RawString>
    {
        private readonly IntPtr _headPointer;
        private readonly int _byteLength;
        public readonly int ByteLength => _byteLength;
        public readonly IntPtr Ptr => _headPointer;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal RawString(int byteLength)
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

        public override bool Equals(object? obj) => obj is RawString str && Equals(str);

        public bool Equals(RawString other)
        {
            return _headPointer == other._headPointer &&
                   _byteLength == other._byteLength;
        }

        public override int GetHashCode() => HashCode.Combine(_headPointer, _byteLength);
    }
}
