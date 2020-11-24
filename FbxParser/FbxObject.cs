#nullable enable
using System;
using System.Runtime.CompilerServices;
using FbxTools.Internal;

namespace FbxTools
{
    /// <summary>Fbx object</summary>
    public sealed unsafe class FbxObject : IDisposable
    {
        private UnsafeRawList<FbxNode> _nodes;
        private bool IsDisposed => _nodes.Ptr == IntPtr.Zero;

        /// <summary>Get <see cref="FbxNode"/>s of this <see cref="FbxObject"/></summary>
        public ReadOnlySpan<FbxNode> Nodes => _nodes.AsSpan();

        internal FbxObject(in UnsafeRawList<FbxNode> nodes)
        {
            _nodes = nodes;
        }

        /// <summary>Finalize <see cref="FbxObject"/></summary>
        ~FbxObject() => Dispose(false);

        /// <summary>Find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="thorwIfNotFound">whether throws exception or not if not found</param>
        /// <exception cref="InvalidOperationException">Children contains no matching node.</exception>
        /// <returns>a found node</returns>
        public ref readonly FbxNode Find(ReadOnlySpan<byte> nodeName, bool thorwIfNotFound = true)
        {
            for(int i = 0; i < _nodes.Count; i++) {
                if(_nodes[i].Name.SequenceEqual(nodeName)) {
                    return ref _nodes[i];
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
        public ref readonly FbxNode TryFind(ReadOnlySpan<byte> nodeName, out bool isFound)
        {
            for(int i = 0; i < _nodes.Count; i++) {
                if(_nodes[i].Name.SequenceEqual(nodeName)) {
                    isFound = true;
                    return ref _nodes[i];
                }
            }
            isFound = false;
            return ref Unsafe.AsRef<FbxNode>(null);
        }

        /// <summary>Find an index of node of specified name. Returns an index of first found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name as ASCII</param>
        /// <param name="thorwIfNotFound">whether throws exception or not if not found</param>
        /// <returns>an index of found node</returns>
        public int FindIndex(ReadOnlySpan<byte> nodeName, bool thorwIfNotFound = true)
        {
            for(int i = 0; i < _nodes.Count; i++) {
                if(_nodes[i].Name.SequenceEqual(nodeName)) {
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
