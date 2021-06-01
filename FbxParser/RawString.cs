#nullable enable

#if !NETSTANDARD2_0
#define CREATE_SPAN_API
#endif

#if CREATE_SPAN_API
using System.Runtime.InteropServices;
#endif

using System;
using System.Text;
using System.Diagnostics;
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

        /// <summary>Get whether the string is empty</summary>
        public bool IsEmpty => _byteLength == 0;

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

        /// <summary>Get slice starting at the specifide index</summary>
        /// <param name="start">start index</param>
        /// <returns>sliced string</returns>
        public RawString Slice(int start)
        {
            if((uint)start > (uint)_byteLength) {
                ThrowOutOfRange();
                static void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(start));
            }
            return new RawString(IntPtr.Add(_headPointer, start), _byteLength - start);
        }

        /// <summary>Get slice of the specified length starting at the specifide index</summary>
        /// <param name="start">start index</param>
        /// <param name="length">length of slice</param>
        /// <returns>sliced string</returns>
        public RawString Slice(int start, int length)
        {
            if((uint)start > (uint)_byteLength) {
                ThrowOutOfRange();
                static void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(start));
            }
            if((uint)length > (uint)(_byteLength - start)) {
                ThrowOutOfRange();
                static void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(length));
            }

            return new RawString(IntPtr.Add(_headPointer, start), length);
        }

        /// <summary>Get ASCII bytes span</summary>
        /// <returns>ASCII bytes span</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> AsSpan()
        {
#if CREATE_SPAN_API
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<byte>((void*)_headPointer), _byteLength);
#else
            return new ReadOnlySpan<byte>((void*)_headPointer, _byteLength);
#endif
        }

        /// <summary>Get ASCII bytes span starting at the specified index</summary>
        /// <param name="start">start index</param>
        /// <returns>ASCII bytes span</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> AsSpan(int start)
        {
            if((uint)start > (uint)_byteLength) {
                ThrowOutOfRange();
                static void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(start));
            }

#if CREATE_SPAN_API
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<byte>(((byte*)_headPointer) + start), _byteLength - start);
#else
            return new ReadOnlySpan<byte>(((byte*)_headPointer) + start, _byteLength - start);
#endif
        }

        /// <summary>Get ASCII bytes span of the specified length starting at the specified index</summary>
        /// <param name="start">start index</param>
        /// <param name="length">length of span</param>
        /// <returns>ASCII bytes span</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> AsSpan(int start, int length)
        {
            if((uint)start > (uint)_byteLength) {
                ThrowOutOfRange();
                static void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(start));
            }
            if((uint)length > (uint)(_byteLength - start)) {
                ThrowOutOfRange();
                static void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(length));
            }

#if CREATE_SPAN_API
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<byte>(((byte*)_headPointer) + start), length);
#else
            return new ReadOnlySpan<byte>(((byte*)_headPointer) + start, length);
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

        /// <summary>Indicates whether <paramref name="left"/> and <paramref name="right"/> and equal.</summary>
        /// <param name="left">the left object to compere</param>
        /// <param name="right">the right object to compere</param>
        /// <returns><see langword="true"/> if equal, otherwise <see langword="false"/></returns>
#if NET5_0_OR_GREATER
        [SkipLocalsInit]
#endif
        public static bool operator ==(RawString left, string right)
        {
            return ReEncodingOperation.Func(right, left, &Compere, &Fallback);

            static bool Compere(ReadOnlySpan<byte> ascii, RawString s) => ascii.SequenceEqual(s.AsSpan());
            static bool Fallback() => false;
        }

        /// <summary>Indicates whether <paramref name="left"/> and <paramref name="right"/> are not equal.</summary>
        /// <param name="left">the left object to compere</param>
        /// <param name="right">the right object to compere</param>
        /// <returns><see langword="true"/> if not equal, otherwise <see langword="false"/></returns>
        public static bool operator !=(RawString left, string right) => !(left == right);

        /// <summary>Indicates whether <paramref name="left"/> and <paramref name="right"/> and equal.</summary>
        /// <param name="left">the left object to compere</param>
        /// <param name="right">the right object to compere</param>
        /// <returns><see langword="true"/> if equal, otherwise <see langword="false"/></returns>
        public static bool operator ==(string left, RawString right) => right == left;

        /// <summary>Indicates whether <paramref name="left"/> and <paramref name="right"/> are not equal.</summary>
        /// <param name="left">the left object to compere</param>
        /// <param name="right">the right object to compere</param>
        /// <returns><see langword="true"/> if not equal, otherwise <see langword="false"/></returns>
        public static bool operator !=(string left, RawString right) => !(right == left);

        /// <summary>Convert the <see cref="RawString"/> to <see cref="string"/></summary>
        /// <returns></returns>
        public override string ToString()
        {
            if(_byteLength == 0) { return string.Empty; }
            return Encoding.ASCII.GetString((byte*)_headPointer, _byteLength);
        }
    }
}
