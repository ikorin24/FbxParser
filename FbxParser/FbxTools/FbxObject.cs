#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FbxTools.Internal;

namespace FbxTools
{
    public sealed unsafe class FbxObject : IDisposable
    {
        private UnsafeRawList<FbxNode> _nodes;
        private bool IsDisposed => _nodes.Ptr == IntPtr.Zero;

        public int NodesCount => _nodes.Count;

        public Span<FbxNode> Nodes => _nodes.AsSpan();

        public ref readonly FbxNode this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if((uint)index >= _nodes.Count) {
                    ThrowOutOfRange();
                    static void ThrowOutOfRange() => throw new ArgumentOutOfRangeException(nameof(index));
                }
                return ref _nodes[index];
            }
        }

        internal FbxObject(in UnsafeRawList<FbxNode> nodes)
        {
            _nodes = nodes;
        }

        ~FbxObject() => Dispose(false);

        public void Dispose()
        {
            if(IsDisposed) { return; }
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if(IsDisposed) { return; }

            _nodes.Dispose();
            _nodes = default;
        }
    }

    public unsafe struct FbxNode
    {
        private FbxNode* _children;
        private int _childrenCapacity;
        private int _childrenCount;

        private UnsafeRawArray<FbxProperty> _properties;
        private RawString _name;

        internal readonly Span<byte> NameInternal => _name.AsSpan();

        internal readonly Span<FbxProperty> PropertiesInternal => _properties.AsSpan();

        public readonly int ChildrenCount => _childrenCount;

        public ReadOnlySpan<FbxNode> Children => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<FbxNode>(_children), _childrenCount);

        public readonly int PropertiesCount => _properties.Length;

        public ReadOnlySpan<FbxProperty> Properties => _properties.AsSpan();

        internal FbxNode(RawString name, int propCount)
        {
            _name = name;
            _properties = new UnsafeRawArray<FbxProperty>(propCount);

            const int InitialCapacity = 16;
            _children = (FbxNode*)Marshal.AllocHGlobal(InitialCapacity * sizeof(FbxNode));
            _childrenCapacity = InitialCapacity;
            _childrenCount = 0;
        }

        internal void AddChild(in FbxNode node)
        {
            if(_childrenCount >= _childrenCapacity) {
                ExtendChildren();
            }
            _children[_childrenCount] = node;
            _childrenCount++;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]  // uncommon path
        private void ExtendChildren()
        {
            FbxNode* children = null;
            var newLen = _childrenCapacity == 0 ? 4 : _childrenCapacity * 2;
            try {
                children = (FbxNode*)Marshal.AllocHGlobal(newLen * sizeof(FbxNode));
                Buffer.MemoryCopy(_children, children, newLen * sizeof(FbxNode), _childrenCapacity * sizeof(FbxNode));
            }
            catch {
                Marshal.FreeHGlobal((IntPtr)children);
                throw;
            }
            finally {
                Marshal.FreeHGlobal((IntPtr)_children);
                _children = children;
                _childrenCapacity = newLen;
            }
        }

        internal void Free()
        {
            for(int i = 0; i < _properties.Length; i++) {
                _properties[i].Free();
            }
            _properties.Dispose();
            _properties = default;

            for(int i = 0; i < _childrenCount; i++) {
                _children[i].Free();
            }
            Marshal.FreeHGlobal((IntPtr)_children);
            _children = null;

            _name.Dispose();
        }
    }

    public unsafe struct FbxProperty
    {
        internal FbxPropertyType _type;
        internal int _valueCountOfArray;
        internal void* _ptrToValue;

        public readonly FbxPropertyType Type => _type;

        internal void Free()
        {
            Marshal.FreeHGlobal((IntPtr)_ptrToValue);
            _ptrToValue = null;
            _valueCountOfArray = 0;
        }

        #region Set XXX
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetInt16(short value)
        {
            _type = FbxPropertyType.Int16;
            _valueCountOfArray = 0;
            _ptrToValue = Marshal.AllocHGlobal(sizeof(short)).ToPointer();
            *(short*)_ptrToValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetInt32(int value)
        {
            _type = FbxPropertyType.Int32;
            _valueCountOfArray = 0;
            _ptrToValue = Marshal.AllocHGlobal(sizeof(int)).ToPointer();
            *(int*)_ptrToValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetInt64(long value)
        {
            _type = FbxPropertyType.Int64;
            _valueCountOfArray = 0;
            _ptrToValue = Marshal.AllocHGlobal(sizeof(long)).ToPointer();
            *(long*)_ptrToValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetString(byte* strInHeap, int length)
        {
            _type = FbxPropertyType.String;
            _valueCountOfArray = length;
            _ptrToValue = strInHeap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetBool(bool value)
        {
            _type = FbxPropertyType.Bool;
            _valueCountOfArray = 0;
            _ptrToValue = Marshal.AllocHGlobal(sizeof(bool)).ToPointer();
            *(bool*)_ptrToValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetFloat(float value)
        {
            _type = FbxPropertyType.Float;
            _valueCountOfArray = 0;
            _ptrToValue = Marshal.AllocHGlobal(sizeof(float)).ToPointer();
            *(float*)_ptrToValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetDouble(double value)
        {
            _type = FbxPropertyType.Double;
            _valueCountOfArray = 0;
            _ptrToValue = Marshal.AllocHGlobal(sizeof(double)).ToPointer();
            *(double*)_ptrToValue = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetBoolArray(bool* arrayInHeap, int length)
        {
            _type = FbxPropertyType.BoolArray;
            _valueCountOfArray = length;
            _ptrToValue = arrayInHeap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetInt32Array(int* arrayInHeap, int length)
        {
            _type = FbxPropertyType.Int32Array;
            _valueCountOfArray = length;
            _ptrToValue = arrayInHeap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetInt64Array(long* arrayInHeap, int length)
        {
            _type = FbxPropertyType.Int64Array;
            _valueCountOfArray = length;
            _ptrToValue = arrayInHeap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetFloatArray(float* arrayInHeap, int length)
        {
            _type = FbxPropertyType.FloatArray;
            _valueCountOfArray = length;
            _ptrToValue = arrayInHeap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetDoubleArray(double* arrayInHeap, int length)
        {
            _type = FbxPropertyType.DoubleArray;
            _valueCountOfArray = length;
            _ptrToValue = arrayInHeap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetByteArray(byte* arrayInHeap, int length)
        {
            _type = FbxPropertyType.ByteArray;
            _valueCountOfArray = length;
            _ptrToValue = arrayInHeap;
        }
        #endregion

        #region As XXX
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short AsInt16()
        {
            if(_type != FbxPropertyType.Int16) { ThrowInvalidCast(_type); }
            return *(short*)_ptrToValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AsInt32()
        {
            if(_type != FbxPropertyType.Int32) { ThrowInvalidCast(_type); }
            return *(int*)_ptrToValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long AsInt64()
        {
            if(_type != FbxPropertyType.Int64) { ThrowInvalidCast(_type); }
            return *(long*)_ptrToValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> AsString()
        {
            if(_type != FbxPropertyType.String) { ThrowInvalidCast(_type); }
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<byte>(_ptrToValue), _valueCountOfArray);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AsBool()
        {
            if(_type != FbxPropertyType.Bool) { ThrowInvalidCast(_type); }
            return *(bool*)_ptrToValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float AsFloat()
        {
            if(_type != FbxPropertyType.Float) { ThrowInvalidCast(_type); }
            return *(float*)_ptrToValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double AsDouble()
        {
            if(_type != FbxPropertyType.Double) { ThrowInvalidCast(_type); }
            return *(double*)_ptrToValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<bool> AsBoolArray()
        {
            if(_type != FbxPropertyType.BoolArray) { ThrowInvalidCast(_type); }
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<bool>(_ptrToValue), _valueCountOfArray);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<int> AsInt32Array()
        {
            if(_type != FbxPropertyType.Int32Array) { ThrowInvalidCast(_type); }
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<int>(_ptrToValue), _valueCountOfArray);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<long> AsInt64Array()
        {
            if(_type != FbxPropertyType.Int64Array) { ThrowInvalidCast(_type); }
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<long>(_ptrToValue), _valueCountOfArray);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<float> AsFloatArray()
        {
            if(_type != FbxPropertyType.FloatArray) { ThrowInvalidCast(_type); }
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<float>(_ptrToValue), _valueCountOfArray);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<double> AsDoubleArray()
        {
            if(_type != FbxPropertyType.DoubleArray) { ThrowInvalidCast(_type); }
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<double>(_ptrToValue), _valueCountOfArray);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<byte> AsByteArray()
        {
            if(_type != FbxPropertyType.ByteArray) { ThrowInvalidCast(_type); }
            return MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<byte>(_ptrToValue), _valueCountOfArray);
        }
        #endregion

        [DoesNotReturn]
        private static void ThrowInvalidCast(FbxPropertyType type)
        {
            throw new InvalidCastException($"Property type is {type}.");
        }
    }

    public enum FbxPropertyType
    {
        Int32,
        Int16,
        Int64,
        Float,
        Double,
        Bool,
        String,

        Int32Array,
        Int64Array,
        FloatArray,
        DoubleArray,
        BoolArray,
        ByteArray,
    }
}
