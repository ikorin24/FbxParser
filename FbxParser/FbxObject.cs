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
            for(int i = 0; i < _nodes.Count; i++) {
                if(_nodes[i].Name.SequenceEqual(nodeName)) {
                    var ptr = (FbxNode_*)Unsafe.AsPointer(ref _nodes[i]);
                    node = new FbxNode(ptr);
                    return true;
                }
            }
            node = FbxNode.Null;
            return false;
        }

        /// <summary>Find an index of node of specified name. Returns an index of first found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <returns>an index of found node</returns>
        public int FindIndex(ReadOnlySpan<byte> nodeName)
        {
            for(int i = 0; i < _nodes.Count; i++) {
                if(_nodes[i].Name.SequenceEqual(nodeName)) {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="buffer">buffer to store result</param>
        /// <returns>found count</returns>
        public int FindIndexAll(ReadOnlySpan<byte> nodeName, Span<int> buffer)
        {
            var count = 0;
            for(int i = 0; i < _nodes.Count; i++) {
                if(_nodes[i].Name.SequenceEqual(nodeName)) {
                    buffer[count++] = i;
                }
            }
            return count;
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
