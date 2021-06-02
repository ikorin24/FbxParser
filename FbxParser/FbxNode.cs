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

        private FbxNode_* Pointer => (FbxNode_*)_ptr;

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
        public readonly FbxNode Find(ReadOnlySpan<byte> nodeName) => FbxNode_.Find(Pointer->ChildrenInternal, nodeName);

        /// <summary>Find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <exception cref="InvalidOperationException">Children contains no matching node.</exception>
        /// <returns>a found node</returns>
        public readonly FbxNode Find(RawString nodeName) => Find(nodeName.AsSpan());

        /// <summary>Find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <exception cref="InvalidOperationException">Children contains no matching node.</exception>
        /// <returns>a found node</returns>
        public readonly FbxNode Find(string nodeName) => FbxNode_.Find(Pointer->ChildrenInternal, nodeName);

        /// <summary>Try to find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="node">a found node</param>
        /// <returns>found or not</returns>
        public readonly bool TryFind(ReadOnlySpan<byte> nodeName, out FbxNode node) => FbxNode_.TryFind(Pointer->ChildrenInternal, nodeName, out node);

        /// <summary>Try to find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="node">a found node</param>
        /// <returns>found or not</returns>
        public readonly bool TryFind(RawString nodeName, out FbxNode node) => TryFind(nodeName.AsSpan(), out node);

        /// <summary>Try to find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="node">a found node</param>
        /// <returns>found or not</returns>
        public readonly bool TryFind(string nodeName, out FbxNode node) => TryFind(nodeName, out node);

        /// <summary>Find an index of node of specified name. Returns an index of first found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <returns>an index of found node</returns>
        public readonly int FindIndex(ReadOnlySpan<byte> nodeName) => FbxNode_.FindIndex(Pointer->ChildrenInternal, nodeName);

        /// <summary>Find an index of node of specified name. Returns an index of first found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <returns>an index of found node</returns>
        public readonly int FindIndex(RawString nodeName) => FindIndex(nodeName.AsSpan());

        /// <summary>Find an index of node of specified name. Returns an index of first found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <returns>an index of found node</returns>
        public readonly int FindIndex(string nodeName) => FbxNode_.FindIndex(Pointer->ChildrenInternal, nodeName);

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="buffer">buffer to store result</param>
        /// <returns>found count</returns>
        public readonly int FindIndexAll(ReadOnlySpan<byte> nodeName, Span<int> buffer) => FbxNode_.FindIndexAll(Pointer->ChildrenInternal, nodeName, buffer);

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="buffer">buffer to store result</param>
        /// <returns>found count</returns>
        public readonly int FindIndexAll(RawString nodeName, Span<int> buffer) => FindIndexAll(nodeName.AsSpan(), buffer);

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="buffer">buffer to store result</param>
        /// <returns>found count</returns>
        public readonly int FindIndexAll(string nodeName, Span<int> buffer) => FbxNode_.FindIndexAll(Pointer->ChildrenInternal, nodeName, buffer);

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <returns>index list</returns>
        public readonly int[] FindIndexAll(ReadOnlySpan<byte> nodeName) => FbxNode_.FindIndexAll(Pointer->ChildrenInternal, nodeName);

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <returns>index list</returns>
        public readonly int[] FindIndexAll(RawString nodeName) => FindIndexAll(nodeName.AsSpan());

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <returns>index list</returns>
        public readonly int[] FindIndexAll(string nodeName) => FbxNode_.FindIndexAll(Pointer->ChildrenInternal, nodeName);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is FbxNode node && Equals(node);

        /// <inheritdoc/>
        public bool Equals(FbxNode other) => _ptr.Equals(other._ptr);

        /// <inheritdoc/>
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
        public readonly FbxNodeChildrenInternal ChildrenInternal => new((FbxNode_*)_children, _childrenCount);

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

        public static FbxNode Find(FbxNodeChildrenInternal children, ReadOnlySpan<byte> nodeName)
        {
            if(TryFind(children, nodeName, out var node) == false) {
                Throw();
                static void Throw() => throw new InvalidOperationException("Children contains no matching node.");
            }
            return node;
        }

        public static FbxNode Find(FbxNodeChildrenInternal children, string nodeName)
        {
            return ReEncodingOperation.Func(nodeName, children, &FindLocal, &Fallback);

            static FbxNode FindLocal(ReadOnlySpan<byte> ascii, FbxNodeChildrenInternal children) => Find(children, ascii);
            static FbxNode Fallback() => throw new InvalidOperationException("Children contains no matching node.");
        }

        public static bool TryFind(FbxNodeChildrenInternal children, ReadOnlySpan<byte> nodeName, out FbxNode node)
        {
            for(int i = 0; i < children.Count; i++) {
                if(children[i].Name.SequenceEqual(nodeName)) {
                    node = new FbxNode(&children.Pointer[i]);
                    return true;
                }
            }
            node = FbxNode.Null;
            return false;
        }

        public static bool TryFind(FbxNodeChildrenInternal children, string nodeName, out FbxNode node)
        {
            bool isFound;
            (node, isFound) = ReEncodingOperation.Func(nodeName, children, &TryFindLocal, &Fallback);
            return isFound;

            static (FbxNode result, bool isFound) TryFindLocal(ReadOnlySpan<byte> ascii, FbxNodeChildrenInternal children)
            {
                var isFound = TryFind(children, ascii, out var result);
                return (result, isFound);
            }
            static (FbxNode result, bool isFound) Fallback() => (FbxNode.Null, false);
        }

        public static int FindIndex(FbxNodeChildrenInternal children, ReadOnlySpan<byte> nodeName)
        {
            for(int i = 0; i < children.Count; i++) {
                if(children[i].Name.SequenceEqual(nodeName)) {
                    return i;
                }
            }
            return -1;
        }

        public static int FindIndex(FbxNodeChildrenInternal children, string nodeName)
        {
            return ReEncodingOperation.Func(nodeName, children, &FindIndexLocal, &Fallback);

            static int FindIndexLocal(ReadOnlySpan<byte> ascii, FbxNodeChildrenInternal children) => FindIndex(children, ascii);
            static int Fallback() => -1;
        }

        public static int FindIndexAll(FbxNodeChildrenInternal children, ReadOnlySpan<byte> nodeName, Span<int> buffer)
        {
            var count = 0;
            for(int i = 0; i < children.Count; i++) {
                if(children[i].Name.SequenceEqual(nodeName)) {
                    buffer[count++] = i;
                }
            }
            return count;
        }

        public static int FindIndexAll(FbxNodeChildrenInternal children, string nodeName, Span<int> buffer)
        {
            fixed(int* b = buffer) {
                var state = (new IntPtr(b), buffer.Length, children);
                return ReEncodingOperation.Func(nodeName, state, &FindIndexAllLocal, &Fallback);
            }

            static int FindIndexAllLocal(ReadOnlySpan<byte> ascii, (IntPtr b, int len, FbxNodeChildrenInternal children) state)
            {
                var buffer = new Span<int>((int*)state.b, state.len);
                return FindIndexAll(state.children, ascii, buffer);
            }
            static int Fallback() => 0;
        }

#if NET5_0_OR_GREATER
        [SkipLocalsInit]
#endif
        public static int[] FindIndexAll(FbxNodeChildrenInternal children, ReadOnlySpan<byte> nodeName)
        {
            if(children.Count <= 128) {
                Span<int> buf = stackalloc int[children.Count];
                var count = FindIndexAll(children, nodeName, buf);
                return buf.Slice(0, count).ToArray();
            }
            else {
                int* p = null;
                try {
                    p = (int*)Marshal.AllocHGlobal(sizeof(int) * children.Count);
                    var buf = new Span<int>(p, children.Count);
                    var count = FindIndexAll(children, nodeName, buf);
                    return buf.Slice(0, count).ToArray();
                }
                finally {
                    Marshal.FreeHGlobal(new IntPtr(p));
                }
            }
        }

        public static int[] FindIndexAll(FbxNodeChildrenInternal children, string nodeName)
        {
            return ReEncodingOperation.Func(nodeName, children, &FindIndexAllLocal, &Fallback);

            static int[] FindIndexAllLocal(ReadOnlySpan<byte> ascii, FbxNodeChildrenInternal children) => FindIndexAll(children, ascii);
            static int[] Fallback() => Array.Empty<int>();
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

        public ReadOnlySpan<FbxNode_> Children => _entity.ChildrenInternal.AsSpan();

        public FbxNode_DebuggerTypeProxy(FbxNode_ entity)
        {
            _entity = entity;
        }
    }
}
