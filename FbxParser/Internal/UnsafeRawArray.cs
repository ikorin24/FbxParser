﻿#nullable enable
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FbxTools.Internal
{
    /// <summary>Low level wrapper of malloc, free. There are no safety checking, no zero initialized</summary>
    /// <remarks>
    /// [NOTE] if you use debugger viewer, enable zero-initialized at constructor. (otherwise shows random values or throws an exception in debugger.)
    /// </remarks>
    /// <typeparam name="T">type of element</typeparam>
    [DebuggerTypeProxy(typeof(UnsafeRawArrayDebuggerTypeProxy<>))]
    [DebuggerDisplay("UnsafeRawArray<{typeof(T).Name}>[{Length}]")]
    internal readonly unsafe struct UnsafeRawArray<T> : IDisposable, IEquatable<UnsafeRawArray<T>> where T : unmanaged
    {
        /// <summary>Get length of array</summary>
        public readonly int Length;
        /// <summary>Get pointer to array</summary>
        public readonly IntPtr Ptr;

        /// <summary>Get empty array</summary>
        public static UnsafeRawArray<T> Empty => default;

        /// <summary>Get or set an element of specified index. (Boundary is not checked, be careful.)</summary>
        /// <param name="index">index of the element</param>
        /// <returns>an element of specified index</returns>
        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref Unsafe.Add(ref GetReference(), index);
        }

        /// <summary>Allocate non-zero-initialized array of specified length.</summary>
        /// <param name="length">length of new array</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnsafeRawArray(int length)
        {
            if(length < 0) {
                throw new ArgumentOutOfRangeException(nameof(length));
            }
            if(length == 0) {
                this = default;
                return;
            }
            Length = length;
            UnmanagedMemoryHelper.RegisterNewAllocatedBytes(length * sizeof(T));
            Ptr = Marshal.AllocHGlobal(length * sizeof(T));
        }

        /// <summary>Allocate array of specified length.</summary>
        /// <param name="length">length of new array</param>
        /// <param name="zeroFill">Whether to initialized the array by zero.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UnsafeRawArray(int length, bool zeroFill)
        {
            if(length < 0) {
                throw new ArgumentOutOfRangeException(nameof(length));
            }
            if(length == 0) {
                this = default;
                return;
            }
            Length = length;
            UnmanagedMemoryHelper.RegisterNewAllocatedBytes(length * sizeof(T));
            Ptr = Marshal.AllocHGlobal(length * sizeof(T));
            if(zeroFill) {
                AsSpan().Clear();
            }
        }

        /// <summary>Free the array</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            UnmanagedMemoryHelper.RegisterReleasedBytes(Length * sizeof(T));
            Marshal.FreeHGlobal(Ptr);
            Unsafe.AsRef(Length) = 0;
            Unsafe.AsRef(Ptr) = default;
        }

        /// <summary>Copy to managed memory</summary>
        /// <param name="array">managed memory array</param>
        /// <param name="arrayIndex">start index of destination array</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if(array == null) { throw new ArgumentNullException(nameof(array)); }
            if((uint)arrayIndex >= (uint)array.Length) { throw new ArgumentOutOfRangeException(nameof(arrayIndex)); }
            if(arrayIndex + Length > array.Length) { throw new ArgumentException("There is not enouph length of destination array"); }

            if(Length == 0) {
                return;
            }

            fixed(T* arrayPtr = array) {
                var byteLen = (long)(Length * sizeof(T));
                Buffer.MemoryCopy((void*)Ptr, arrayPtr + arrayIndex, byteLen, byteLen);
            }
        }

        /// <summary>Get reference to the 0th element of the array. (If length of the array is 0, returns reference to null)</summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetReference() => ref Unsafe.AsRef<T>((void*)Ptr);

        /// <summary>Get <see cref="Span{T}"/></summary>
        /// <returns><see cref="Span{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan()
        {
#if NETSTANDARD2_0
            return new Span<T>((T*)Ptr, Length);
#else
            return MemoryMarshal.CreateSpan(ref GetReference(), Length);
#endif
        }

        /// <summary>Get <see cref="Span{T}"/></summary>
        /// <remarks>Boundary is not checked. Be careful !!</remarks>
        /// <param name="start">start index</param>
        /// <returns><see cref="Span{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan(int start)
        {
#if NETSTANDARD2_0
            return new Span<T>((T*)Ptr + start, Length - start);
#else
            return MemoryMarshal.CreateSpan(ref this[start], Length - start);
#endif
        }

        /// <summary>Get <see cref="Span{T}"/></summary>
        /// <remarks>Boundary is not checked. Be careful !!</remarks>
        /// <param name="start">start index</param>
        /// <param name="length">length of <see cref="Span{T}"/></param>
        /// <returns><see cref="Span{T}"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan(int start, int length)
        {
#if NETSTANDARD2_0
            return new Span<T>((T*)Ptr + start, length);
#else
            return MemoryMarshal.CreateSpan(ref this[start], length);
#endif

        }

        public override bool Equals(object? obj) => obj is UnsafeRawArray<T> array && Equals(array);

        public bool Equals(UnsafeRawArray<T> other) => Length == other.Length && Ptr.Equals(other.Ptr);

        public override int GetHashCode() => HashCode.Combine(Length, Ptr);
    }

    internal class UnsafeRawArrayDebuggerTypeProxy<T> where T : unmanaged
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly T[] _items;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _items;

        public UnsafeRawArrayDebuggerTypeProxy(UnsafeRawArray<T> entity)
        {
            _items = entity.AsSpan().ToArray();
        }
    }
}
