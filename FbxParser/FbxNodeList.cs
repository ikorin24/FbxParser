#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using FbxTools.Internal;

namespace FbxTools
{
    /// <summary>Provides list of <see cref="FbxNode"/></summary>
    [DebuggerTypeProxy(typeof(FbxNodeListDebuggerTypeProxy))]
    [DebuggerDisplay("FbxNode[{_count}]")]
    public unsafe readonly struct FbxNodeList : IEnumerable<FbxNode>, IEquatable<FbxNodeList>, ICollection<FbxNode>
    {
        private readonly FbxNode_* _node;
        private readonly int _count;

        /// <summary>Get count of the items</summary>
        public int Count => _count;

        bool ICollection<FbxNode>.IsReadOnly => true;

        /// <summary>Get <see cref="FbxNode"/> of the specified index</summary>
        /// <param name="index">index of the item</param>
        /// <returns>fbx node</returns>
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

        /// <summary>Get enumerator</summary>
        /// <returns>an enumerator instance</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(_node, _count);
        }

        /// <summary>Get enumerator</summary>
        /// <returns>an enumerator instance</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(_node, _count);
        }

        /// <summary>Get enumerator</summary>
        /// <returns>an enumerator instance</returns>
        IEnumerator<FbxNode> IEnumerable<FbxNode>.GetEnumerator()
        {
            return new Enumerator(_node, _count);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is FbxNodeList list && Equals(list);
        }

        /// <summary>Indicates whether this instance and a specified <see cref="FbxNodeList"/> is equal sequentially.</summary>
        /// <param name="other">the object to compere</param>
        /// <returns><see langword="true"/> if equal, otherwise <see langword="false"/></returns>
        public bool Equals(FbxNodeList other)
        {
            return _node == other._node && _count == other._count;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine((IntPtr)_node, _count);
        }

        void ICollection<FbxNode>.Add(FbxNode item) => throw new NotSupportedException();

        void ICollection<FbxNode>.Clear() => throw new NotSupportedException();

        bool ICollection<FbxNode>.Contains(FbxNode item) => throw new NotSupportedException();

        void ICollection<FbxNode>.CopyTo(FbxNode[] array, int arrayIndex)
        {
            if(array is null) { throw new ArgumentNullException(nameof(array)); }
            var slice = array.AsSpan(arrayIndex);
            for(int i = 0; i < _count; i++) {
                slice[i] = new FbxNode(_node + i);
            }
        }

        bool ICollection<FbxNode>.Remove(FbxNode item) => throw new NotSupportedException();

        /// <summary>Enumerator of <see cref="FbxNodeList"/></summary>
        public struct Enumerator : IEnumerator<FbxNode>
        {
            private readonly FbxNode_* _node;
            private FbxNode_* _current;
            private readonly int _count;
            private int _i;

            /// <summary>Get the current item</summary>
            public FbxNode Current => new FbxNode(_current);

            object IEnumerator.Current => Current;

            internal Enumerator(FbxNode_* node, int count)
            {
                _node = node;
                _count = count;
                _current = null;
                _i = 0;
            }

            /// <summary>dispose</summary>
            public void Dispose()
            {
                // nop
            }

            /// <summary>Move to next</summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if(_i >= _count) { return false; }
                _current = _node + _i;
                _i++;
                return true;
            }

            /// <summary>Reset</summary>
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
                ((ICollection<FbxNode>)_list).CopyTo(array, 0);
                return array;
            }
        }

        public FbxNodeListDebuggerTypeProxy(FbxNodeList list)
        {
            _list = list;
        }
    }
}
