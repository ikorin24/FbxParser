#nullable enable

#if !NETSTANDARD2_0
#define CREATE_SPAN_API
#endif

#if CREATE_SPAN_API
using System.Runtime.InteropServices;
#endif

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FbxTools
{
    /// <summary>Raw array struct representing a slice of memory.</summary>
    /// <typeparam name="T">type of elements</typeparam>
    [DebuggerTypeProxy(typeof(RawArrayTypeProxy<>))]
    [DebuggerDisplay("{DebugDisplay(),nq}")]
    public unsafe readonly struct RawArray<T> : IEquatable<RawArray<T>> where T : unmanaged
    {
        private readonly IntPtr _ptr;   // T*
        private readonly int _length;

        /// <summary>Get length of string</summary>
        public int Length => _length;

        /// <summary>Get whether the array is empty</summary>
        public bool IsEmpty => _length == 0;

        /// <summary>Get an item of specified index.</summary>
        /// <param name="i">index of the item</param>
        /// <returns>item</returns>
        public ref readonly T this[int i]
        {
            get
            {
                if((uint)i >= _length) {
                    ThrowOutOfRange();
                    static void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(i));
                }
                return ref *(((T*)_ptr) + i);
            }
        }

        /// <summary>Get empty array instance</summary>
        public static RawArray<T> Empty => default;

        internal RawArray(IntPtr ptr, int length)
        {
            _ptr = ptr;
            _length = length;
        }

        /// <summary>Get sliced array starting at the specified index</summary>
        /// <param name="start">start index</param>
        /// <returns>sliced array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawArray<T> Slice(int start)
        {
            if((uint)start > (uint)_length) {
                ThrowOutOfRange();
                static void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(start));
            }
            return new RawArray<T>((IntPtr)(((T*)_ptr) + start), _length - start);
        }

        /// <summary>Get sliced array of the specified length starting at the specified index</summary>
        /// <param name="start">start index</param>
        /// <param name="length">length of array</param>
        /// <returns>sliced array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RawArray<T> Slice(int start, int length)
        {
            if((uint)start > (uint)_length) {
                ThrowOutOfRange();
                static void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(start));
            }
            if((uint)length > (uint)(_length - start)) {
                ThrowOutOfRange();
                static void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(length));
            }
            return new RawArray<T>((IntPtr)(((T*)_ptr) + start), length);
        }

        /// <summary>Convert to <see cref="ReadOnlySpan{T}"/></summary>
        /// <returns><see cref="ReadOnlySpan{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public ReadOnlySpan<T> AsSpan()
        {
#if CREATE_SPAN_API
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<T>((void*)_ptr), _length);
#else
            return new ReadOnlySpan<T>((void*)_ptr, _length);
#endif
        }

        /// <summary>Convert to <see cref="ReadOnlySpan{T}"/> starting at the specified index</summary>
        /// <param name="start">start index</param>
        /// <returns><see cref="ReadOnlySpan{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> AsSpan(int start)
        {
            if((uint)start > (uint)_length) {
                ThrowOutOfRange();
                static void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(start));
            }

#if CREATE_SPAN_API
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<T>(((T*)_ptr) + start), _length - start);
#else
            return new ReadOnlySpan<T>(((T*)_ptr) + start, _length - start);
#endif
        }

        /// <summary>Convert to <see cref="ReadOnlySpan{T}"/> of the specified length starting at the specified index</summary>
        /// <param name="start">start index</param>
        /// <param name="length">length of span</param>
        /// <returns><see cref="ReadOnlySpan{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<T> AsSpan(int start, int length)
        {
            if((uint)start > (uint)_length) {
                ThrowOutOfRange();
                static void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(start));
            }
            if((uint)length > (uint)(_length - start)) {
                ThrowOutOfRange();
                static void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(length));
            }

#if CREATE_SPAN_API
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<T>(((T*)_ptr) + start), length);
#else
            return new ReadOnlySpan<T>(((T*)_ptr) + start, length);
#endif
        }

        /// <summary>Copy to an array</summary>
        /// <returns>a copied array instance</returns>
        public T[] ToArray()
        {
            return AsSpan().ToArray();
        }

        /// <summary>Get pinnable reference for 'fixed' statement</summary>
        /// <returns>reference to pin</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ref readonly T GetPinnableReference()    // only for 'fixed'
        {
            return ref Unsafe.AsRef<T>((void*)_ptr);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is RawArray<T> array && Equals(array);

        /// <inheritdoc/>
        public bool Equals(RawArray<T> other) => _ptr.Equals(other._ptr) && _length == other._length;

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(_ptr, _length);

        private string DebugDisplay() => $"RawArray<{typeof(T).Name}>[{_length}]";
    }

    /// <summary>Provides extensions of <see cref="RawArray{T}"/></summary>
    public static class RawArrayExtension
    {
        /// <summary>Determines whether two read-only sequences are equal by comparing the elements</summary>
        /// <typeparam name="T">type of elements</typeparam>
        /// <param name="source">source array</param>
        /// <param name="array">array to compere to source</param>
        /// <returns>equal or not equal</returns>
        public static bool SequenceEqual<T>(this RawArray<T> source, RawArray<T> array) where T : unmanaged, IEquatable<T>
        {
            return source.AsSpan().SequenceEqual(array.AsSpan());
        }

        /// <summary>Determines whether two read-only sequences are equal by comparing the elements</summary>
        /// <typeparam name="T">type of elements</typeparam>
        /// <param name="source">source array</param>
        /// <param name="span">span to compere to source</param>
        /// <returns>equal or not equal</returns>
        public static bool SequenceEqual<T>(this RawArray<T> source, ReadOnlySpan<T> span) where T : unmanaged, IEquatable<T>
        {
            return source.AsSpan().SequenceEqual(span);
        }
    }

    internal sealed class RawArrayTypeProxy<T> where T : unmanaged
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly T[] _items;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _items;

        public RawArrayTypeProxy(RawArray<T> entity)
        {
            _items = entity.ToArray();
        }
    }
}
