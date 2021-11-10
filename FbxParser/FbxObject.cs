#nullable enable
using System;
using System.Diagnostics;
using FbxTools.Internal;

namespace FbxTools
{
    /// <summary>Fbx object</summary>
    public sealed unsafe class FbxObject : IDisposable
    {
        private UnsafeRawList<FbxNode_> _nodes;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FbxNodeChildrenInternal RootNodes => new((FbxNode_*)_nodes.Ptr, _nodes.Count);

        private bool IsDisposed => _nodes.Ptr == IntPtr.Zero;

        /// <summary>Get root <see cref="FbxNode"/>s of this <see cref="FbxObject"/></summary>
        public FbxNodeList Nodes => new FbxNodeList((FbxNode_*)_nodes.Ptr, _nodes.Count);

        internal FbxObject(in UnsafeRawList<FbxNode_> nodes)
        {
            _nodes = nodes;
        }

        /// <summary>Finalize <see cref="FbxObject"/></summary>
        ~FbxObject() => Dispose(false);

        /// <summary>Find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <exception cref="InvalidOperationException">Children contains no matching node.</exception>
        /// <returns>a found node</returns>
        public FbxNode Find(ReadOnlySpan<byte> nodeName) => FbxNode_.Find(RootNodes, nodeName);

        /// <summary>Find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <exception cref="InvalidOperationException">Children contains no matching node.</exception>
        /// <returns>a found node</returns>
        public FbxNode Find(RawString nodeName) => FbxNode_.Find(RootNodes, nodeName.AsSpan());

        /// <summary>Find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <exception cref="InvalidOperationException">Children contains no matching node.</exception>
        /// <returns>a found node</returns>
        public FbxNode Find(string nodeName) => FbxNode_.Find(RootNodes, nodeName);

        public FbxNode[] FindAll(ReadOnlySpan<byte> nodeName) => FbxNode_.FindAll(RootNodes, nodeName);

        public FbxNode[] FindAll(RawString nodeName) => FbxNode_.FindAll(RootNodes, nodeName.AsSpan());

        public FbxNode[] FindAll(string nodeName) => FbxNode_.FindAll(RootNodes, nodeName);

        /// <summary>Try to find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="node">a found node</param>
        /// <returns>found or not</returns>
        public bool TryFind(ReadOnlySpan<byte> nodeName, out FbxNode node) => FbxNode_.TryFind(RootNodes, nodeName, out node);

        /// <summary>Try to find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="node">a found node</param>
        /// <returns>found or not</returns>
        public bool TryFind(RawString nodeName, out FbxNode node) => FbxNode_.TryFind(RootNodes, nodeName.AsSpan(), out node);

        /// <summary>Try to find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="node">a found node</param>
        /// <returns>found or not</returns>
        public bool TryFind(string nodeName, out FbxNode node) => FbxNode_.TryFind(RootNodes, nodeName, out node);

        /// <summary>Find an index of node of specified name. Returns an index of first found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <returns>an index of found node</returns>
        public int FindIndex(ReadOnlySpan<byte> nodeName) => FbxNode_.FindIndex(RootNodes, nodeName);

        /// <summary>Find an index of node of specified name. Returns an index of first found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <returns>an index of found node</returns>
        public int FindIndex(RawString nodeName) => FbxNode_.FindIndex(RootNodes, nodeName.AsSpan());

        /// <summary>Find an index of node of specified name. Returns an index of first found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <returns>an index of found node</returns>
        public int FindIndex(string nodeName) => FbxNode_.FindIndex(RootNodes, nodeName);

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="buffer">buffer to store result</param>
        /// <returns>found count</returns>
        public int FindIndexAll(ReadOnlySpan<byte> nodeName, Span<int> buffer) => FbxNode_.FindIndexAll(RootNodes, nodeName, buffer);

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="buffer">buffer to store result</param>
        /// <returns>found count</returns>
        public int FindIndexAll(RawString nodeName, Span<int> buffer) => FbxNode_.FindIndexAll(RootNodes, nodeName.AsSpan(), buffer);

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="buffer">buffer to store result</param>
        /// <returns>found count</returns>
        public int FindIndexAll(string nodeName, Span<int> buffer) => FbxNode_.FindIndexAll(RootNodes, nodeName, buffer);

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <returns>index list</returns>
        public int[] FindIndexAll(ReadOnlySpan<byte> nodeName) => FbxNode_.FindIndexAll(RootNodes, nodeName);

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <returns>index list</returns>
        public int[] FindIndexAll(RawString nodeName) => FbxNode_.FindIndexAll(RootNodes, nodeName.AsSpan());

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <returns>index list</returns>
        public int[] FindIndexAll(string nodeName) => FbxNode_.FindIndexAll(RootNodes, nodeName);

        /// <summary>Release all memories <see cref="FbxObject"/> has</summary>
        public void Dispose()
        {
            if(IsDisposed) { return; }
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if(IsDisposed) { return; }

            for(int i = 0; i < _nodes.Count; i++) {
                _nodes[i].Free();
            }
            _nodes.Dispose();
            _nodes = default;
        }
    }
}
