#nullable enable
using System;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using FbxTools.Internal;
using System.ComponentModel;

namespace FbxTools
{
    /// <summary>ASCII string representing a slice of memory.</summary>
    [DebuggerDisplay("{ToString()}")]
    public readonly unsafe struct RawString : IEquatable<RawString>
    {
        private readonly IntPtr _headPointer;
        private readonly int _byteLength;

        /// <summary>Get length of string</summary>
        public int Length => _byteLength;

        /// <summary>Get empty of <see cref="RawString"/></summary>
        public static RawString Empty => default;

        /// <summary>Get an ASCII char of specified index.</summary>
        /// <param name="i">index of the char</param>
        /// <returns>item</returns>
        public ref readonly byte this[int i]
        {
            get
            {
                if((uint)i >= _byteLength) {
                    ThrowOutOfRange();
                    static void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(i));
                }
                return ref *(((byte*)_headPointer) + i);
            }
        }

        internal RawString(IntPtr ptr, int byteLen)
        {
            _headPointer = ptr;
            _byteLength = byteLen;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is RawString str && Equals(str);

        /// <summary>Indicates whether this instance and a specified <see cref="RawString"/> is equal sequentially.</summary>
        /// <param name="other">the object to compere</param>
        /// <returns><see langword="true"/> if equal, otherwise <see langword="false"/></returns>
        public bool Equals(RawString other) => (_headPointer == other._headPointer && _byteLength == other._byteLength) || SequenceEqual(other);

        /// <summary>Indicates whether this instance and a specified <see cref="RawString"/> is equal sequentially.</summary>
        /// <param name="other">the object to compere</param>
        /// <returns><see langword="true"/> if equal, otherwise <see langword="false"/></returns>
        public bool SequenceEqual(RawString other) => AsSpan().SequenceEqual(other.AsSpan());

        /// <summary>Indicates whether this instance and a specified <see cref="RawString"/> is equal sequentially.</summary>
        /// <param name="other">the object to compere</param>
        /// <returns><see langword="true"/> if equal, otherwise <see langword="false"/></returns>
        public bool SequenceEqual(ReadOnlySpan<byte> other) => AsSpan().SequenceEqual(other);

        /// <summary>Get ASCII bytes span</summary>
        /// <returns>ASCII byte span</returns>
        public ReadOnlySpan<byte> AsSpan()
        {
#if NETSTANDARD2_0
            return new ReadOnlySpan<byte>((void*)_headPointer, _byteLength);
#else
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<byte>((void*)_headPointer), _byteLength);
#endif
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return XXHash32.ComputeHash((byte*)_headPointer, _byteLength);
        }

        /// <summary>Get pinnable reference for 'fixed' statement</summary>
        /// <returns>reference to pin</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ref readonly byte GetPinnableReference()    // only for 'fixed'
        {
            return ref Unsafe.AsRef<byte>((void*)_headPointer);
        }

        /// <summary>Indicates whether <paramref name="left"/> and <paramref name="right"/> are equal.</summary>
        /// <param name="left">the left object to compere</param>
        /// <param name="right">the right object to compere</param>
        /// <returns><see langword="true"/> if equal, otherwise <see langword="false"/></returns>
        public static bool operator ==(RawString left, RawString right) => left.Equals(right);

        /// <summary>Indicates whether <paramref name="left"/> and <paramref name="right"/> are not equal.</summary>
        /// <param name="left">the left object to compere</param>
        /// <param name="right">the right object to compere</param>
        /// <returns><see langword="true"/> if not equal, otherwise <see langword="false"/></returns>
        public static bool operator !=(RawString left, RawString right) => !(left == right);

        /// <summary>Convert the <see cref="RawString"/> to <see cref="string"/></summary>
        /// <returns></returns>
        public override string ToString()
        {
            if(_byteLength == 0) { return string.Empty; }
            return Encoding.ASCII.GetString((byte*)_headPointer, _byteLength);
        }
    }
}
