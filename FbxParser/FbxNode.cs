#nullable enable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using FbxTools.Internal;

namespace FbxTools
{
    /// <summary>Node structure of fbx</summary>
    [DebuggerTypeProxy(typeof(FbxNodeDebuggerTypeProxy))]
    [DebuggerDisplay("{DebuggerDisplay()}")]
    public unsafe struct FbxNode
    {
        // I want to use 'UnsafeRawList<FbxNode>' as children,
        // but TypeLoadException happens in xunit test. (It may bug of xunit.)
        private FbxNode* _children;
        private int _childrenCapacity;
        private int _childrenCount;

        private UnsafeRawArray<FbxProperty> _properties;
        private RawString _name;

        private string DebuggerDisplay() => $"{Encoding.ASCII.GetString(_name.AsSpan())}   (Properties={_properties.Length} Children={_childrenCount})";

        internal readonly Span<byte> NameInternal => _name.AsSpan();

        internal readonly Span<FbxProperty> PropertiesInternal => _properties.AsSpan();

        /// <summary>Get name of the node</summary>
        public readonly ReadOnlySpan<byte> Name => _name.AsSpan();

        /// <summary>Get children nodes</summary>
        public readonly ReadOnlySpan<FbxNode> Children => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<FbxNode>(_children), _childrenCount);

        public readonly ReadOnlySpan<FbxProperty> Properties => _properties.AsSpan();

        internal FbxNode(RawString name, int propCount)
        {
            _name = name;
            _properties = new UnsafeRawArray<FbxProperty>(propCount);

            const int InitialCapacity = 16;
            _children = (FbxNode*)Marshal.AllocHGlobal(InitialCapacity * sizeof(FbxNode));
            UnmanagedMemoryHelper.RegisterNewAllocatedBytes(InitialCapacity * sizeof(FbxNode));
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
                UnmanagedMemoryHelper.RegisterNewAllocatedBytes(newLen * sizeof(FbxNode));
                children = (FbxNode*)Marshal.AllocHGlobal(newLen * sizeof(FbxNode));
                Buffer.MemoryCopy(_children, children, newLen * sizeof(FbxNode), _childrenCapacity * sizeof(FbxNode));
            }
            catch {
                Marshal.FreeHGlobal((IntPtr)children);
                UnmanagedMemoryHelper.RegisterReleasedBytes(newLen * sizeof(FbxNode));
                throw;
            }
            finally {
                Marshal.FreeHGlobal((IntPtr)_children);
                UnmanagedMemoryHelper.RegisterReleasedBytes(_childrenCapacity * sizeof(FbxNode));
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
            UnmanagedMemoryHelper.RegisterReleasedBytes(_childrenCapacity * sizeof(FbxNode));
            _children = null;

            _name.Dispose();
        }
    }

    internal class FbxNodeDebuggerTypeProxy
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FbxNode _entity;

        public string Name => Encoding.ASCII.GetString(_entity.Name);

        public ReadOnlySpan<FbxProperty> Properties => _entity.Properties;

        public ReadOnlySpan<FbxNode> Children => _entity.Children;

        public FbxNodeDebuggerTypeProxy(FbxNode entity)
        {
            _entity = entity;
        }
    }
}
