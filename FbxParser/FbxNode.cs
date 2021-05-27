#nullable enable
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Diagnostics;
using FbxTools.Internal;

namespace FbxTools
{
    /// <summary>Node structure of fbx</summary>
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public readonly unsafe struct FbxNode : IEquatable<FbxNode>
    {
        private readonly IntPtr _ptr;

        private string DebuggerDisplay() => _ptr != IntPtr.Zero ? ((FbxNode_*)_ptr)->DebuggerDisplay() : "null";

        internal static FbxNode Null => default;

        /// <summary>Get name of the node</summary>
        public RawString Name => ((FbxNode_*)_ptr)->Name;

        /// <summary>Get properties of the node</summary>
        public ReadOnlySpan<FbxProperty> Properties => ((FbxNode_*)_ptr)->Properties;

        /// <summary>Get children nodes</summary>
        public FbxNodeList Children => ((FbxNode_*)_ptr)->Children;

        internal FbxNode(FbxNode_* ptr)
        {
            _ptr = (IntPtr)ptr;
        }

        /// <summary>Find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <exception cref="InvalidOperationException">Children contains no matching node.</exception>
        /// <returns>a found node</returns>
        public readonly FbxNode Find(ReadOnlySpan<byte> nodeName) => ((FbxNode_*)_ptr)->Find(nodeName);

        /// <summary>Try to find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="node">a found node</param>
        /// <returns>found or not</returns>
        public readonly bool TryFind(ReadOnlySpan<byte> nodeName, out FbxNode node) => ((FbxNode_*)_ptr)->TryFind(nodeName, out node);

        /// <summary>Find an index of node of specified name. Returns an index of first found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <returns>an index of found node</returns>
        public readonly int FindIndex(ReadOnlySpan<byte> nodeName) => ((FbxNode_*)_ptr)->FindIndex(nodeName);

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="buffer">buffer to store result</param>
        /// <returns>found count</returns>
        public readonly int FindIndexAll(ReadOnlySpan<byte> nodeName, Span<int> buffer) => ((FbxNode_*)_ptr)->FindIndexAll(nodeName, buffer);

        public override bool Equals(object? obj) => obj is FbxNode node && Equals(node);

        public bool Equals(FbxNode other) => _ptr.Equals(other._ptr);

        public override int GetHashCode() => _ptr.GetHashCode();
    }


    /// <summary>Node internal structure of fbx</summary>
    [DebuggerTypeProxy(typeof(FbxNode_DebuggerTypeProxy))]
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    internal unsafe struct FbxNode_ : IEquatable<FbxNode_>
    {
        // I want to use 'UnsafeRawList<FbxNode_>' as children,
        // but TypeLoadException happens in xunit test. (It may bug of xunit.)
        private IntPtr _children;   // FbxNode_*
        private int _childrenCapacity;
        private int _childrenCount;

        private UnsafeRawArray<FbxProperty> _properties;
        private RawStringMem _name;

        internal string DebuggerDisplay() => $"\"{_name.ToString()}\" (Properties={_properties.Length} Children={_childrenCount})";

        internal readonly Span<byte> NameInternal => _name.AsSpan();

        internal readonly Span<FbxProperty> PropertiesInternal => _properties.AsSpan();

        /// <summary>Get name of the node (encoded as ASCII)</summary>
        public readonly RawString Name => _name.AsRawString();

        /// <summary>Get children nodes</summary>
        public readonly ReadOnlySpan<FbxNode_> ChildrenInternal =>
#if NETSTANDARD2_0
            new ReadOnlySpan<FbxNode_>((void*)_children, _childrenCount);
#else
            MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<FbxNode_>((void*)_children), _childrenCount);
#endif

        /// <summary>Get properties of the node</summary>
        public readonly ReadOnlySpan<FbxProperty> Properties => _properties.AsSpan();

        public readonly FbxNodeList Children => new FbxNodeList((FbxNode_*)_children, _childrenCount);

        internal FbxNode_(RawStringMem name, int propCount)
        {
            _name = name;
            _properties = new UnsafeRawArray<FbxProperty>(propCount);

            const int InitialCapacity = 16;
            _children = Marshal.AllocHGlobal(InitialCapacity * sizeof(FbxNode_));
            UnmanagedMemoryHelper.RegisterNewAllocatedBytes(InitialCapacity * sizeof(FbxNode_));
            _childrenCapacity = InitialCapacity;
            _childrenCount = 0;
        }

        /// <summary>Find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <exception cref="InvalidOperationException">Children contains no matching node.</exception>
        /// <returns>a found node</returns>
        public readonly FbxNode Find(ReadOnlySpan<byte> nodeName)
        {
            if(TryFind(nodeName, out var node) == false) {
                Throw();
                static void Throw() => throw new InvalidOperationException("Children contains no matching node.");
            }
            return node;
        }

        /// <summary>Try to find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="node">a found node</param>
        /// <returns>found or not</returns>
        public readonly bool TryFind(ReadOnlySpan<byte> nodeName, out FbxNode node)
        {
            var children = (FbxNode_*)_children;
            for(int i = 0; i < _childrenCount; i++) {
                if(children[i].Name.SequenceEqual(nodeName)) {
                    node = new FbxNode(&children[i]);
                    return true;
                }
            }
            node = FbxNode.Null;
            return false;
        }

        /// <summary>Find an index of node of specified name. Returns an index of first found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <returns>an index of found node</returns>
        public readonly int FindIndex(ReadOnlySpan<byte> nodeName)
        {
            var children = (FbxNode_*)_children;
            for(int i = 0; i < _childrenCount; i++) {
                if(children[i].Name.SequenceEqual(nodeName)) {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="buffer">buffer to store result</param>
        /// <returns>found count</returns>
        public readonly int FindIndexAll(ReadOnlySpan<byte> nodeName, Span<int> buffer)
        {
            var children = (FbxNode_*)_children;
            var count = 0;
            for(int i = 0; i < _childrenCount; i++) {
                if(children[i].Name.SequenceEqual(nodeName)) {
                    buffer[count++] = i;
                }
            }
            return count;
        }

        internal void AddChild(in FbxNode_ node)
        {
            if(_childrenCount >= _childrenCapacity) {
                ExtendChildren();
            }
            ((FbxNode_*)_children)[_childrenCount] = node;
            _childrenCount++;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]  // uncommon path
        private void ExtendChildren()
        {
            FbxNode_* children = null;
            var newLen = _childrenCapacity == 0 ? 4 : _childrenCapacity * 2;
            try {
                UnmanagedMemoryHelper.RegisterNewAllocatedBytes(newLen * sizeof(FbxNode_));
                children = (FbxNode_*)Marshal.AllocHGlobal(newLen * sizeof(FbxNode_));
                Buffer.MemoryCopy((void*)_children, children, newLen * sizeof(FbxNode_), _childrenCapacity * sizeof(FbxNode_));
            }
            catch {
                Marshal.FreeHGlobal((IntPtr)children);
                UnmanagedMemoryHelper.RegisterReleasedBytes(newLen * sizeof(FbxNode_));
                throw;
            }
            finally {
                Marshal.FreeHGlobal(_children);
                UnmanagedMemoryHelper.RegisterReleasedBytes(_childrenCapacity * sizeof(FbxNode_));
                _children = (IntPtr)children;
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
                ((FbxNode_*)_children)[i].Free();
            }
            Marshal.FreeHGlobal((IntPtr)_children);
            UnmanagedMemoryHelper.RegisterReleasedBytes(_childrenCapacity * sizeof(FbxNode_));
            _children = default;

            _name.Dispose();
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is FbxNode_ node && Equals(node);

        /// <inheritdoc/>
        public bool Equals(FbxNode_ other)
        {
            return _children.Equals(other._children) &&
                   _childrenCapacity == other._childrenCapacity &&
                   _childrenCount == other._childrenCount &&
                   _properties.Equals(other._properties) &&
                   _name.Equals(other._name);
        }

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(_children, _childrenCapacity, _childrenCount, _properties, _name);
    }

    internal class FbxNode_DebuggerTypeProxy
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FbxNode_ _entity;

        public string Name => _entity.ToString()!;

        public ReadOnlySpan<FbxProperty> Properties => _entity.Properties;

        public ReadOnlySpan<FbxNode_> Children => _entity.ChildrenInternal;

        public FbxNode_DebuggerTypeProxy(FbxNode_ entity)
        {
            _entity = entity;
        }
    }
}
