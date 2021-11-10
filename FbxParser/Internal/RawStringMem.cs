#nullable enable
using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace FbxTools.Internal
{
    /// <summary>ASCII string which has its own memory by itself.</summary>
    [DebuggerDisplay("{ToString()}")]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe readonly struct RawStringMem : IDisposable, IEquatable<RawStringMem>
    {
        private readonly UnmanagedHandle _handle;
        private readonly int _byteLength;

        public UnmanagedHandle Handle => _handle;

        public readonly int ByteLength => _byteLength;
        public readonly IntPtr Ptr => _handle.Ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal RawStringMem(int byteLength)
        {
            Debug.Assert(byteLength >= 0);

            if(byteLength == 0) {
                this = default;
            }
            else {
                _handle = UnmanagedAllocator.Alloc(byteLength);
                _byteLength = byteLength;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly override string ToString()
        {
            if(_byteLength == 0) { return string.Empty; }
            return Encoding.ASCII.GetString((byte*)Ptr, _byteLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Span<byte> AsSpan()
        {
#if NETSTANDARD2_0
            return new Span<byte>((void*)Ptr, _byteLength);
#else
            return MemoryMarshal.CreateSpan(ref Unsafe.AsRef<byte>((void*)Ptr), _byteLength);
#endif
        }

        public RawString AsRawString() => new RawString(Ptr, _byteLength);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            UnmanagedAllocator.Free(_handle);
            Unsafe.AsRef(_handle) = UnmanagedHandle.Null;
            Unsafe.AsRef(_byteLength) = 0;
        }

        public override bool Equals(object? obj) => obj is RawStringMem str && Equals(str);

        public bool Equals(RawStringMem other) => _handle == other._handle && _byteLength == other._byteLength;

        public override int GetHashCode() => HashCode.Combine(_handle, _byteLength);

        public static implicit operator RawString(RawStringMem str) => str.AsRawString();
    }
}
