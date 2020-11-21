#nullable enable
using System;
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

        ~FbxObject() => Dispose(false);

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
