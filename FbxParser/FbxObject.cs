#nullable enable
using System;
using System.Runtime.CompilerServices;
using FbxTools.Internal;

namespace FbxTools
{
    /// <summary>Fbx object</summary>
    public sealed unsafe class FbxObject : IDisposable
    {
        private UnsafeRawList<FbxNode_> _nodes;
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
        public FbxNode Find(ReadOnlySpan<byte> nodeName)
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
        public bool TryFind(ReadOnlySpan<byte> nodeName, out FbxNode node)
        {
            return FbxNode_.TryFind((FbxNode_*)_nodes.Ptr, _nodes.Count, nodeName, out node);
        }

        /// <summary>Find an index of node of specified name. Returns an index of first found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <returns>an index of found node</returns>
        public int FindIndex(ReadOnlySpan<byte> nodeName)
        {
            return FbxNode_.FindIndex((FbxNode_*)_nodes.Ptr, _nodes.Count, nodeName);
        }

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="buffer">buffer to store result</param>
        /// <returns>found count</returns>
        public int FindIndexAll(ReadOnlySpan<byte> nodeName, Span<int> buffer)
        {
            return FbxNode_.FindIndexAll((FbxNode_*)_nodes.Ptr, _nodes.Count, nodeName, buffer);
        }

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
