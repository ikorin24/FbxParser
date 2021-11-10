#nullable enable

#if !NETSTANDARD2_0
#define SPAN_API
#endif

using System;

#if SPAN_API
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
#endif

namespace FbxTools.Internal
{
    internal unsafe readonly struct FbxNodeChildrenInternal
    {
        public readonly FbxNode_* Pointer;
        public readonly int Count;

        public ref readonly FbxNode_ this[int i]
        {
            get
            {
#if DEBUG
                if((uint)i >= (uint)Count) { throw new ArgumentOutOfRangeException(nameof(i)); }
#endif
                return ref Pointer[i];
            }
        }

        public FbxNodeChildrenInternal(FbxNode_* pointer, int count)
        {
            Pointer = pointer;
            Count = count;
        }

        public FbxNode IndexOf(int i)
        {
#if DEBUG
            if((uint)i >= (uint)Count) { throw new ArgumentOutOfRangeException(nameof(i)); }
#endif
            return new FbxNode(Pointer + i);
        }

        public ReadOnlySpan<FbxNode_> AsSpan() =>
#if SPAN_API
            MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef<FbxNode_>(Pointer), Count);
#else
            new ReadOnlySpan<FbxNode_>(Pointer, Count);
#endif
    }
}
