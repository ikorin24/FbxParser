﻿#nullable enable
using System;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;
using FbxTools.Internal;

namespace FbxTools
{
    /// <summary>property of <see cref="FbxNode"/></summary>
    [DebuggerDisplay("{DebuggerDisplay()}")]
    public unsafe struct FbxProperty : IEquatable<FbxProperty>
    {
        private FbxPropertyType _type;
        private int _valueCountOfArray;
        private IntPtr _ptrToValue;

        private readonly string DebuggerDisplay()
        {
            if(_ptrToValue == IntPtr.Zero) {
                return "";
            }
            return _type switch
            {
                FbxPropertyType.Int32 => $"int: '{AsInt32()}'",
                FbxPropertyType.Int16 => $"short: '{AsInt16()}'",
                FbxPropertyType.Int64 => $"long: '{AsInt64()}'",
                FbxPropertyType.Float => $"float: '{AsFloat()}'",
                FbxPropertyType.Double => $"double: '{AsDouble()}'",
                FbxPropertyType.Bool => $"bool: '{AsBool()}'",
                FbxPropertyType.String => $"string: '{Encoding.ASCII.GetString((byte*)_ptrToValue, _valueCountOfArray)}'",
                FbxPropertyType.Int32Array => $"int[{_valueCountOfArray}]",
                FbxPropertyType.Int64Array => $"long[{_valueCountOfArray}]",
                FbxPropertyType.FloatArray => $"float[{_valueCountOfArray}]",
                FbxPropertyType.DoubleArray => $"double[{_valueCountOfArray}]",
                FbxPropertyType.BoolArray => $"bool[{_valueCountOfArray}]",
                FbxPropertyType.ByteArray => $"byte[{_valueCountOfArray}]",
                _ => "",
            };
        }

        /// <summary>Get property type</summary>
        public readonly FbxPropertyType Type => _type;

        private static IntPtr Alloc(int byteSize)
        {
            UnmanagedMemoryHelper.RegisterNewAllocatedBytes(byteSize);
            return Marshal.AllocHGlobal(byteSize);
        }

        internal void Free()
        {
#if DEBUG
            var size = _type switch
            {
                FbxPropertyType.Int32 => sizeof(int),
                FbxPropertyType.Int16 => sizeof(short),
                FbxPropertyType.Int64 => sizeof(long),
                FbxPropertyType.Float => sizeof(float),
                FbxPropertyType.Double => sizeof(double),
                FbxPropertyType.Bool => sizeof(bool),
                FbxPropertyType.String => _valueCountOfArray * sizeof(byte),
                FbxPropertyType.Int32Array => _valueCountOfArray * sizeof(int),
                FbxPropertyType.Int64Array => _valueCountOfArray * sizeof(long),
                FbxPropertyType.FloatArray => _valueCountOfArray * sizeof(float),
                FbxPropertyType.DoubleArray => _valueCountOfArray * sizeof(double),
                FbxPropertyType.BoolArray => _valueCountOfArray * sizeof(bool),
                FbxPropertyType.ByteArray => _valueCountOfArray * sizeof(byte),
                _ => 0,
            };
            UnmanagedMemoryHelper.RegisterReleasedBytes(size);
#endif
            Marshal.FreeHGlobal(_ptrToValue);
            _ptrToValue = IntPtr.Zero;
            _valueCountOfArray = 0;
        }

        #region method of Set XXX
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetInt16(short value)
        {
            _type = FbxPropertyType.Int16;
            _valueCountOfArray = 0;
            _ptrToValue = Alloc(sizeof(short));
            *(short*)_ptrToValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetInt32(int value)
        {
            _type = FbxPropertyType.Int32;
            _valueCountOfArray = 0;
            _ptrToValue = Alloc(sizeof(int));
            *(int*)_ptrToValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetInt64(long value)
        {
            _type = FbxPropertyType.Int64;
            _valueCountOfArray = 0;
            _ptrToValue = Alloc(sizeof(long));
            *(long*)_ptrToValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetString(byte* strInHeap, int length)
        {
            _type = FbxPropertyType.String;
            _valueCountOfArray = length;
            _ptrToValue = (IntPtr)strInHeap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetBool(bool value)
        {
            _type = FbxPropertyType.Bool;
            _valueCountOfArray = 0;
            _ptrToValue = Alloc(sizeof(bool));
            *(bool*)_ptrToValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetFloat(float value)
        {
            _type = FbxPropertyType.Float;
            _valueCountOfArray = 0;
            _ptrToValue = Alloc(sizeof(float));
            *(float*)_ptrToValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetDouble(double value)
        {
            _type = FbxPropertyType.Double;
            _valueCountOfArray = 0;
            _ptrToValue = Alloc(sizeof(double));
            *(double*)_ptrToValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetBoolArray(bool* arrayInHeap, int length)
        {
            _type = FbxPropertyType.BoolArray;
            _valueCountOfArray = length;
            _ptrToValue = (IntPtr)arrayInHeap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetInt32Array(int* arrayInHeap, int length)
        {
            _type = FbxPropertyType.Int32Array;
            _valueCountOfArray = length;
            _ptrToValue = (IntPtr)arrayInHeap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetInt64Array(long* arrayInHeap, int length)
        {
            _type = FbxPropertyType.Int64Array;
            _valueCountOfArray = length;
            _ptrToValue = (IntPtr)arrayInHeap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetFloatArray(float* arrayInHeap, int length)
        {
            _type = FbxPropertyType.FloatArray;
            _valueCountOfArray = length;
            _ptrToValue = (IntPtr)arrayInHeap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetDoubleArray(double* arrayInHeap, int length)
        {
            _type = FbxPropertyType.DoubleArray;
            _valueCountOfArray = length;
            _ptrToValue = (IntPtr)arrayInHeap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetByteArray(byte* arrayInHeap, int length)
        {
            _type = FbxPropertyType.ByteArray;
            _valueCountOfArray = length;
            _ptrToValue = (IntPtr)arrayInHeap;
        }
        #endregion

        #region method of As XXX
        /// <summary>Get property value of type <see cref="short"/></summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Int16"/></exception>
        /// <returns>property value of type <see cref="short"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly short AsInt16()
        {
            if(_type != FbxPropertyType.Int16) { ThrowInvalidCast(_type); }
            return *(short*)_ptrToValue;
        }

        /// <summary>Get property value of type <see cref="int"/></summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Int32"/></exception>
        /// <returns>property value of type <see cref="int"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int AsInt32()
        {
            if(_type != FbxPropertyType.Int32) { ThrowInvalidCast(_type); }
            return *(int*)_ptrToValue;
        }

        /// <summary>Get property value of type <see cref="long"/></summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Int64"/></exception>
        /// <returns>property value of type <see cref="long"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long AsInt64()
        {
            if(_type != FbxPropertyType.Int64) { ThrowInvalidCast(_type); }
            return *(long*)_ptrToValue;
        }

        /// <summary>Get property value of type like <see cref="string"/></summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.String"/></exception>
        /// <returns>property value of type like <see cref="string"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<byte> AsString()
        {
            if(_type != FbxPropertyType.String) { ThrowInvalidCast(_type); }
#if NETSTANDARD2_0
            return new ReadOnlySpan<byte>((void*)_ptrToValue, _valueCountOfArray);
#else
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<byte>((void*)_ptrToValue), _valueCountOfArray);
#endif
        }

        /// <summary>Get property value of type <see cref="bool"/></summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Bool"/></exception>
        /// <returns>property value of type <see cref="bool"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool AsBool()
        {
            if(_type != FbxPropertyType.Bool) { ThrowInvalidCast(_type); }
            return *(bool*)_ptrToValue;
        }

        /// <summary>Get property value of type <see cref="float"/></summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Float"/></exception>
        /// <returns>property value of type <see cref="float"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float AsFloat()
        {
            if(_type != FbxPropertyType.Float) { ThrowInvalidCast(_type); }
            return *(float*)_ptrToValue;
        }

        /// <summary>Get property value of type <see cref="double"/></summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Double"/></exception>
        /// <returns>property value of type <see cref="double"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly double AsDouble()
        {
            if(_type != FbxPropertyType.Double) { ThrowInvalidCast(_type); }
            return *(double*)_ptrToValue;
        }

        /// <summary>Get property value of type like <see cref="bool"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.BoolArray"/></exception>
        /// <returns>property value of type like <see cref="bool"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<bool> AsBoolArray()
        {
            if(_type != FbxPropertyType.BoolArray) { ThrowInvalidCast(_type); }
#if NETSTANDARD2_0
            return new ReadOnlySpan<bool>((void*)_ptrToValue, _valueCountOfArray);
#else
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<bool>((void*)_ptrToValue), _valueCountOfArray);
#endif
        }

        /// <summary>Get property value of type like <see cref="int"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Int32Array"/></exception>
        /// <returns>property value of type like <see cref="int"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<int> AsInt32Array()
        {
            if(_type != FbxPropertyType.Int32Array) { ThrowInvalidCast(_type); }
#if NETSTANDARD2_0
            return new ReadOnlySpan<int>((void*)_ptrToValue, _valueCountOfArray);
#else
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<int>((void*)_ptrToValue), _valueCountOfArray);
#endif
        }

        /// <summary>Get property value of type like <see cref="long"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Int64Array"/></exception>
        /// <returns>property value of type like <see cref="long"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<long> AsInt64Array()
        {
            if(_type != FbxPropertyType.Int64Array) { ThrowInvalidCast(_type); }
#if NETSTANDARD2_0
            return new ReadOnlySpan<long>((void*)_ptrToValue, _valueCountOfArray);
#else
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<long>((void*)_ptrToValue), _valueCountOfArray);
#endif
        }

        /// <summary>Get property value of type like <see cref="float"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.FloatArray"/></exception>
        /// <returns>property value of type like <see cref="float"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<float> AsFloatArray()
        {
            if(_type != FbxPropertyType.FloatArray) { ThrowInvalidCast(_type); }
#if NETSTANDARD2_0
            return new ReadOnlySpan<float>((void*)_ptrToValue, _valueCountOfArray);
#else
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<float>((void*)_ptrToValue), _valueCountOfArray);
#endif
        }

        /// <summary>Get property value of type like <see cref="double"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.DoubleArray"/></exception>
        /// <returns>property value of type like <see cref="double"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<double> AsDoubleArray()
        {
            if(_type != FbxPropertyType.DoubleArray) { ThrowInvalidCast(_type); }
#if NETSTANDARD2_0
            return new ReadOnlySpan<double>((void*)_ptrToValue, _valueCountOfArray);
#else
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<double>((void*)_ptrToValue), _valueCountOfArray);
#endif
        }

        /// <summary>Get property value of type like <see cref="byte"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.ByteArray"/></exception>
        /// <returns>property value of type like <see cref="byte"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ReadOnlySpan<byte> AsByteArray()
        {
            if(_type != FbxPropertyType.ByteArray) { ThrowInvalidCast(_type); }
#if NETSTANDARD2_0
            return new ReadOnlySpan<byte>((void*)_ptrToValue, _valueCountOfArray);
#else
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<byte>((void*)_ptrToValue), _valueCountOfArray);
#endif
        }
        #endregion

#if NET5_0
        [DoesNotReturn]
#endif
        private static void ThrowInvalidCast(FbxPropertyType type)
        {
            throw new InvalidCastException($"Property type is {type}.");
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is FbxProperty property && Equals(property);

        /// <inheritdoc/>
        public bool Equals(FbxProperty other)
        {
            return _type == other._type &&
                   _valueCountOfArray == other._valueCountOfArray &&
                   _ptrToValue.Equals(other._ptrToValue);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(_type, _valueCountOfArray, _ptrToValue);
    }
}
