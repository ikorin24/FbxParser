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
    public unsafe struct FbxNode : IEquatable<FbxNode>
    {
        // I want to use 'UnsafeRawList<FbxNode>' as children,
        // but TypeLoadException happens in xunit test. (It may bug of xunit.)
        private IntPtr _children;   // FbxNode*
        private int _childrenCapacity;
        private int _childrenCount;

        private UnsafeRawArray<FbxProperty> _properties;
        private RawString _name;

        private string DebuggerDisplay() => $"{_name.ToString()}   (Properties={_properties.Length} Children={_childrenCount})";

        internal readonly Span<byte> NameInternal => _name.AsSpan();

        internal readonly Span<FbxProperty> PropertiesInternal => _properties.AsSpan();

        /// <summary>Get name of the node (encoded as ASCII)</summary>
        public readonly ReadOnlySpan<byte> Name => _name.AsSpan();

        /// <summary>Get children nodes</summary>
        public readonly ReadOnlySpan<FbxNode> Children =>
#if NETSTANDARD2_0
            new ReadOnlySpan<FbxNode>((void*)_children, _childrenCount);
#else
            MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<FbxNode>((void*)_children), _childrenCount);
#endif

        /// <summary>Get properties of the node</summary>
        public readonly ReadOnlySpan<FbxProperty> Properties => _properties.AsSpan();

        internal FbxNode(RawString name, int propCount)
        {
            _name = name;
            _properties = new UnsafeRawArray<FbxProperty>(propCount);

            const int InitialCapacity = 16;
            _children = Marshal.AllocHGlobal(InitialCapacity * sizeof(FbxNode));
            UnmanagedMemoryHelper.RegisterNewAllocatedBytes(InitialCapacity * sizeof(FbxNode));
            _childrenCapacity = InitialCapacity;
            _childrenCount = 0;
        }

        /// <summary>Find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="thorwIfNotFound">whether throws exception or not if not found</param>
        /// <exception cref="InvalidOperationException">Children contains no matching node.</exception>
        /// <returns>a found node</returns>
        public readonly ref readonly FbxNode Find(ReadOnlySpan<byte> nodeName, bool thorwIfNotFound = true)
        {
            var children = Children;
            for(int i = 0; i < children.Length; i++) {
                if(children[i].Name.SequenceEqual(nodeName)) {
                    return ref children[i];
                }
            }
            if(thorwIfNotFound) {
                throw new InvalidOperationException("Children contains no matching node.");
            }
            return ref Unsafe.AsRef<FbxNode>(null);
        }

        /// <summary>Try to find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="isFound">a node is found or not.</param>
        /// <returns>a found node. (If not found, returns reference to null)</returns>
        public readonly ref readonly FbxNode TryFind(ReadOnlySpan<byte> nodeName, out bool isFound)
        {
            var children = Children;
            for(int i = 0; i < children.Length; i++) {
                if(children[i].Name.SequenceEqual(nodeName)) {
                    isFound = true;
                    return ref children[i];
                }
            }
            isFound = false;
            return ref Unsafe.AsRef<FbxNode>(null);
        }

        /// <summary>Find an index of node of specified name. Returns an index of first found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="thorwIfNotFound">whether throws exception or not if not found</param>
        /// <returns>an index of found node</returns>
        public readonly int FindIndex(ReadOnlySpan<byte> nodeName, bool thorwIfNotFound = true)
        {
            var children = Children;
            for(int i = 0; i < children.Length; i++) {
                if(children[i].Name.SequenceEqual(nodeName)) {
                    return i;
                }
            }
            if(thorwIfNotFound) {
                throw new InvalidOperationException("Children contains no matching node.");
            }
            return -1;
        }

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="buffer">buffer to store result</param>
        /// <returns>found count</returns>
        public readonly int FindIndexAll(ReadOnlySpan<byte> nodeName, Span<int> buffer)
        {
            var children = Children;
            var count = 0;
            for(int i = 0; i < children.Length; i++) {
                if(children[i].Name.SequenceEqual(nodeName)) {
                    buffer[count++] = i;
                }
            }
            return count;
        }

        internal void AddChild(in FbxNode node)
        {
            if(_childrenCount >= _childrenCapacity) {
                ExtendChildren();
            }
            ((FbxNode*)_children)[_childrenCount] = node;
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
                Buffer.MemoryCopy((void*)_children, children, newLen * sizeof(FbxNode), _childrenCapacity * sizeof(FbxNode));
            }
            catch {
                Marshal.FreeHGlobal((IntPtr)children);
                UnmanagedMemoryHelper.RegisterReleasedBytes(newLen * sizeof(FbxNode));
                throw;
            }
            finally {
                Marshal.FreeHGlobal(_children);
                UnmanagedMemoryHelper.RegisterReleasedBytes(_childrenCapacity * sizeof(FbxNode));
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
                ((FbxNode*)_children)[i].Free();
            }
            Marshal.FreeHGlobal((IntPtr)_children);
            UnmanagedMemoryHelper.RegisterReleasedBytes(_childrenCapacity * sizeof(FbxNode));
            _children = default;

            _name.Dispose();
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is FbxNode node && Equals(node);

        /// <inheritdoc/>
        public bool Equals(FbxNode other)
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

    internal class FbxNodeDebuggerTypeProxy
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FbxNode _entity;

        public string Name =>
#if NETSTANDARD2_0
            Encoding.ASCII.GetString(_entity.Name.ToArray());
#else
            Encoding.ASCII.GetString(_entity.Name);
#endif

        public ReadOnlySpan<FbxProperty> Properties => _entity.Properties;

        public ReadOnlySpan<FbxNode> Children => _entity.Children;

        public FbxNodeDebuggerTypeProxy(FbxNode entity)
        {
            _entity = entity;
        }
    }
}
