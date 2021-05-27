#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace FbxTools
{
    [DebuggerTypeProxy(typeof(FbxNodeListDebuggerTypeProxy))]
    [DebuggerDisplay("FbxNode[{_count}]")]
    public unsafe readonly struct FbxNodeList : IEnumerable<FbxNode>, IEquatable<FbxNodeList>
    {
        private readonly FbxNode_* _node;
        private readonly int _count;

        public int Count => _count;

        public FbxNode this[int index]
        {
            get
            {
                if((uint)index >= (uint)_count) {
                    Throw();
                    static void Throw() => throw new ArgumentOutOfRangeException(nameof(index));
                }
                return new FbxNode(_node + index);
            }
        }

        internal FbxNodeList(FbxNode_* node, int count)
        {
            Debug.Assert(count >= 0);
            _node = node;
            _count = count;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_node, _count);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(_node, _count);
        }

        IEnumerator<FbxNode> IEnumerable<FbxNode>.GetEnumerator()
        {
            return new Enumerator(_node, _count);
        }

        public override bool Equals(object? obj)
        {
            return obj is FbxNodeList list && Equals(list);
        }

        public bool Equals(FbxNodeList other)
        {
            return _node == other._node && _count == other._count;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((IntPtr)_node, _count);
        }

        public struct Enumerator : IEnumerator<FbxNode>
        {
            private readonly FbxNode_* _node;
            private FbxNode_* _current;
            private readonly int _count;
            private int _i;

            public FbxNode Current => new FbxNode(_current);

            object IEnumerator.Current => Current;

            internal Enumerator(FbxNode_* node, int count)
            {
                _node = node;
                _count = count;
                _current = null;
                _i = 0;
            }

            public void Dispose()
            {
                // nop
            }

            public bool MoveNext()
            {
                if(_i >= _count) { return false; }
                _current = _node + _i;
                _i++;
                return true;
            }

            public void Reset()
            {
                _current = null;
                _i = 0;
            }
        }
    }


    internal class FbxNodeListDebuggerTypeProxy
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly FbxNodeList _list;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public FbxNode[] Items
        {
            get
            {
                var array = new FbxNode[_list.Count];
                for(int i = 0; i < array.Length; i++) {
                    array[i] = _list[i];
                }
                return array;
            }
        }

        public FbxNodeListDebuggerTypeProxy(FbxNodeList list)
        {
            _list = list;
        }
    }
}
