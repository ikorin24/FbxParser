#nullable enable
using System;
using System.Runtime.CompilerServices;
using FbxTools.Internal;

namespace FbxTools
{
    public sealed unsafe class FbxObject : IDisposable
    {
        private UnsafeRawList<FbxNode> _nodes;
        private bool IsDisposed => _nodes.Ptr == IntPtr.Zero;

        public int NodesCount => _nodes.Count;

        public ReadOnlySpan<FbxNode> Nodes => _nodes.AsSpan();

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

            for(int i = 0; i < _nodes.Count; i++) {
                _nodes[i].Free();
            }
            _nodes.Dispose();
            _nodes = default;
        }
    }
}
