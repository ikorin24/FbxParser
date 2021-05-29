#nullable enable
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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

        /// <summary>Convert to <see cref="ReadOnlySpan{T}"/></summary>
        /// <returns><see cref="ReadOnlySpan{T}"/></returns>
        public ReadOnlySpan<T> AsSpan()
        {
#if NETSTANDARD2_0
            return new ReadOnlySpan<T>((void*)_ptr, _length);
#else
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<T>((void*)_ptr), _length);
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
