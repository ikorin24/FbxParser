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
    /// <summary>property of <see cref="FbxNode_"/></summary>
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public unsafe struct FbxProperty : IEquatable<FbxProperty>
    {
        private FbxPropertyType _type;
        private int _valueCountOfArray;
        private UnmanagedHandle _ptrToValue;

        private readonly string DebuggerDisplay()
        {
            if(_ptrToValue.IsNull) {
                if(_type == FbxPropertyType.String) {
                    return "string: \"\"";
                }
                return "";
            }
            return _type switch
            {
                FbxPropertyType.Int32 => $"int: {AsInt32()}",
                FbxPropertyType.Int16 => $"short: {AsInt16()}",
                FbxPropertyType.Int64 => $"long: {AsInt64()}",
                FbxPropertyType.Float => $"float: {AsFloat()}",
                FbxPropertyType.Double => $"double: {AsDouble()}",
                FbxPropertyType.Bool => $"bool: {AsBool()}",
                FbxPropertyType.String => $"string: \"{Encoding.ASCII.GetString((byte*)_ptrToValue.Ptr, _valueCountOfArray)}\"",
                FbxPropertyType.Int32Array => $"int[{_valueCountOfArray}]",
                FbxPropertyType.Int64Array => $"long[{_valueCountOfArray}]",
                FbxPropertyType.FloatArray => $"float[{_valueCountOfArray}]",
                FbxPropertyType.DoubleArray => $"double[{_valueCountOfArray}]",
                FbxPropertyType.BoolArray => $"bool[{_valueCountOfArray}]",
                FbxPropertyType.ByteArray => $"byte[{_valueCountOfArray}]",
                _ => "<invalid>",
            };
        }

        /// <summary>Get property type</summary>
        public readonly FbxPropertyType Type => _type;

        internal void Free()
        {
            UnmanagedAllocator.Free(_ptrToValue);
            _ptrToValue = UnmanagedHandle.Null;
            _valueCountOfArray = 0;
        }

        #region method of Set XXX
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetInt16(short value)
        {
            _type = FbxPropertyType.Int16;
            _valueCountOfArray = 0;
            _ptrToValue = UnmanagedAllocator.Alloc(sizeof(short));
            *(short*)_ptrToValue.Ptr = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetInt32(int value)
        {
            _type = FbxPropertyType.Int32;
            _valueCountOfArray = 0;
            _ptrToValue = UnmanagedAllocator.Alloc(sizeof(int));
            *(int*)_ptrToValue.Ptr = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetInt64(long value)
        {
            _type = FbxPropertyType.Int64;
            _valueCountOfArray = 0;
            _ptrToValue = UnmanagedAllocator.Alloc(sizeof(long));
            *(long*)_ptrToValue.Ptr = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetString(RawStringMem str)
        {
            _type = FbxPropertyType.String;
            _valueCountOfArray = str.ByteLength;
            _ptrToValue = str.Handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetBool(bool value)
        {
            _type = FbxPropertyType.Bool;
            _valueCountOfArray = 0;
            _ptrToValue = UnmanagedAllocator.Alloc(sizeof(bool));
            *(bool*)_ptrToValue.Ptr = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetFloat(float value)
        {
            _type = FbxPropertyType.Float;
            _valueCountOfArray = 0;
            _ptrToValue = UnmanagedAllocator.Alloc(sizeof(float));
            *(float*)_ptrToValue.Ptr = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetDouble(double value)
        {
            _type = FbxPropertyType.Double;
            _valueCountOfArray = 0;
            _ptrToValue = UnmanagedAllocator.Alloc(sizeof(double));
            *(double*)_ptrToValue.Ptr = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetBoolArray(UnmanagedHandle handle, int arrayLength)
        {
            _type = FbxPropertyType.BoolArray;
            _valueCountOfArray = arrayLength;
            _ptrToValue = handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetInt32Array(UnmanagedHandle handle, int arrayLength)
        {
            _type = FbxPropertyType.Int32Array;
            _valueCountOfArray = arrayLength;
            _ptrToValue = handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetInt64Array(UnmanagedHandle handle, int arrayLength)
        {
            _type = FbxPropertyType.Int64Array;
            _valueCountOfArray = arrayLength;
            _ptrToValue = handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetFloatArray(UnmanagedHandle handle, int arrayLength)
        {
            _type = FbxPropertyType.FloatArray;
            _valueCountOfArray = arrayLength;
            _ptrToValue = handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetDoubleArray(UnmanagedHandle handle, int arrayLength)
        {
            _type = FbxPropertyType.DoubleArray;
            _valueCountOfArray = arrayLength;
            _ptrToValue = handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetByteArray(UnmanagedHandle handle, int arrayLength)
        {
            _type = FbxPropertyType.ByteArray;
            _valueCountOfArray = arrayLength;
            _ptrToValue = handle;
        }
        #endregion

        #region method of As XXX
        /// <summary>Get property value if its type is <see cref="short"/></summary>
        /// <param name="value">property value of type <see cref="short"/></param>
        /// <returns>success or not</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAsInt16(out short value)
        {
            if(_type == FbxPropertyType.Int16) {
                value = *(short*)_ptrToValue.Ptr;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>Get property value of type <see cref="short"/></summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Int16"/></exception>
        /// <returns>property value of type <see cref="short"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly short AsInt16()
        {
            if(TryAsInt16(out var result) == false) {
                ThrowInvalidCast(_type);
            }
            return result;
        }

        /// <summary>Get property value if its type is <see cref="int"/></summary>
        /// <param name="value">property value of type <see cref="int"/></param>
        /// <returns>success or not</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAsInt32(out int value)
        {
            if(_type == FbxPropertyType.Int32) {
                value = *(int*)_ptrToValue.Ptr;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>Get property value of type <see cref="int"/></summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Int32"/></exception>
        /// <returns>property value of type <see cref="int"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int AsInt32()
        {
            if(TryAsInt32(out var result) == false) {
                ThrowInvalidCast(_type);
            }
            return result;
        }

        /// <summary>Get property value if its type is <see cref="long"/></summary>
        /// <param name="value">property value of type <see cref="long"/></param>
        /// <returns>success or not</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAsInt64(out long value)
        {
            if(_type == FbxPropertyType.Int64) {
                value = *(long*)_ptrToValue.Ptr;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>Get property value of type <see cref="long"/></summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Int64"/></exception>
        /// <returns>property value of type <see cref="long"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly long AsInt64()
        {
            if(TryAsInt64(out var result) == false) {
                ThrowInvalidCast(_type);
            }
            return result;
        }

        /// <summary>Get property value if its type is <see cref="string"/> like</summary>
        /// <param name="value">property value of type like <see cref="string"/></param>
        /// <returns>success or not</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAsString(out RawString value)
        {
            if(_type == FbxPropertyType.String) {
                value = new RawString(_ptrToValue.Ptr, _valueCountOfArray);
                return true;
            }
            value = RawString.Empty;
            return false;
        }

        /// <summary>Get property value of type like <see cref="string"/></summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.String"/></exception>
        /// <returns>property value of type like <see cref="string"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly RawString AsString()
        {
            if(TryAsString(out var result) == false) {
                ThrowInvalidCast(_type);
            }
            return result;
        }

        /// <summary>Get property value if its type is <see cref="bool"/></summary>
        /// <param name="value">property value of type <see cref="bool"/></param>
        /// <returns>success or not</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAsBool(out bool value)
        {
            if(_type == FbxPropertyType.Bool) {
                value = *(bool*)_ptrToValue.Ptr;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>Get property value of type <see cref="bool"/></summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Bool"/></exception>
        /// <returns>property value of type <see cref="bool"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool AsBool()
        {
            if(TryAsBool(out var result) == false) {
                ThrowInvalidCast(_type);
            }
            return result;
        }

        /// <summary>Get property value if its type is <see cref="float"/></summary>
        /// <param name="value">property value of type <see cref="float"/></param>
        /// <returns>success or not</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAsFloat(out float value)
        {
            if(_type == FbxPropertyType.Float) {
                value = *(float*)_ptrToValue.Ptr;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>Get property value of type <see cref="float"/></summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Float"/></exception>
        /// <returns>property value of type <see cref="float"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float AsFloat()
        {
            if(TryAsFloat(out var result) == false) {
                ThrowInvalidCast(_type);
            }
            return result;
        }

        /// <summary>Get property value of type <see cref="double"/></summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Double"/></exception>
        /// <returns>property value of type <see cref="double"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAsDouble(out double value)
        {
            if(_type == FbxPropertyType.Double) {
                value = *(double*)_ptrToValue.Ptr;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>Get property value of type <see cref="double"/></summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Double"/></exception>
        /// <returns>property value of type <see cref="double"/></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly double AsDouble()
        {
            if(TryAsDouble(out var result) == false) {
                ThrowInvalidCast(_type);
            }
            return result;
        }

        /// <summary>Get property value of type <see cref="bool"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.BoolArray"/></exception>
        /// <returns>property value of type <see cref="bool"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAsBoolArray(out RawArray<bool> value)
        {
            if(_type == FbxPropertyType.BoolArray) {
                value = new RawArray<bool>(_ptrToValue.Ptr, _valueCountOfArray);
                return true;
            }
            value = RawArray<bool>.Empty;
            return false;
        }

        /// <summary>Get property value of type like <see cref="bool"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.BoolArray"/></exception>
        /// <returns>property value of type like <see cref="bool"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly RawArray<bool> AsBoolArray()
        {
            if(TryAsBoolArray(out var value) == false) {
                ThrowInvalidCast(_type);
            }
            return value;
        }

        /// <summary>Get property value of type <see cref="int"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Int32Array"/></exception>
        /// <returns>property value of type <see cref="int"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAsInt32Array(out RawArray<int> value)
        {
            if(_type == FbxPropertyType.Int32Array) {
                value = new RawArray<int>(_ptrToValue.Ptr, _valueCountOfArray);
                return true;
            }
            value = RawArray<int>.Empty;
            return false;
        }

        /// <summary>Get property value of type like <see cref="int"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Int32Array"/></exception>
        /// <returns>property value of type like <see cref="int"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly RawArray<int> AsInt32Array()
        {
            if(TryAsInt32Array(out var result) == false) {
                ThrowInvalidCast(_type);
            }
            return result;
        }

        /// <summary>Get property value of type <see cref="long"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Int64Array"/></exception>
        /// <returns>property value of type <see cref="long"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAsInt64Array(out RawArray<long> value)
        {
            if(_type == FbxPropertyType.Int64Array) {
                value = new RawArray<long>(_ptrToValue.Ptr, _valueCountOfArray);
                return true;
            }
            value = RawArray<long>.Empty;
            return false;
        }

        /// <summary>Get property value of type like <see cref="long"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.Int64Array"/></exception>
        /// <returns>property value of type like <see cref="long"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly RawArray<long> AsInt64Array()
        {
            if(TryAsInt64Array(out var result) == false) {
                ThrowInvalidCast(_type);
            }
            return result;
        }

        /// <summary>Get property value of type <see cref="float"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.FloatArray"/></exception>
        /// <returns>property value of type <see cref="float"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAsFloatArray(out RawArray<float> value)
        {
            if(_type == FbxPropertyType.FloatArray) {
                value = new RawArray<float>(_ptrToValue.Ptr, _valueCountOfArray);
                return true;
            }
            value = RawArray<float>.Empty;
            return false;
        }

        /// <summary>Get property value of type like <see cref="float"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.FloatArray"/></exception>
        /// <returns>property value of type like <see cref="float"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly RawArray<float> AsFloatArray()
        {
            if(TryAsFloatArray(out var result) == false) {
                ThrowInvalidCast(_type);
            }
            return result;
        }

        /// <summary>Get property value of type <see cref="double"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.DoubleArray"/></exception>
        /// <returns>property value of type <see cref="double"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAsDoubleArray(out RawArray<double> value)
        {
            if(_type == FbxPropertyType.DoubleArray) {
                value = new RawArray<double>(_ptrToValue.Ptr, _valueCountOfArray);
                return true;
            }
            value = RawArray<double>.Empty;
            return false;
        }

        /// <summary>Get property value of type like <see cref="double"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.DoubleArray"/></exception>
        /// <returns>property value of type like <see cref="double"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly RawArray<double> AsDoubleArray()
        {
            if(TryAsDoubleArray(out var result) == false) {
                ThrowInvalidCast(_type);
            }
            return result;
        }

        /// <summary>Get property value of type <see cref="byte"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.ByteArray"/></exception>
        /// <returns>property value of type <see cref="byte"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool TryAsByteArray(out RawArray<byte> value)
        {
            if(_type == FbxPropertyType.ByteArray) {
                value = new RawArray<byte>(_ptrToValue.Ptr, _valueCountOfArray);
                return true;
            }
            value = RawArray<byte>.Empty;
            return false;
        }

        /// <summary>Get property value of type like <see cref="byte"/> array</summary>
        /// <exception cref="InvalidCastException"><see cref="Type"/> is not <see cref="FbxPropertyType.ByteArray"/></exception>
        /// <returns>property value of type like <see cref="byte"/> array</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly RawArray<byte> AsByteArray()
        {
            if(TryAsByteArray(out var result) == false) {
                ThrowInvalidCast(_type);
            }
            return result;
        }
        #endregion

        [DoesNotReturn]
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
