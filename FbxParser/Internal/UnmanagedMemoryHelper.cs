#nullable enable
using System;
using System.Diagnostics;

namespace FbxTools.Internal
{
    internal static unsafe class UnmanagedMemoryHelper
    {
        private const string DEBUG = "DEBUG";

        [ThreadStatic]
        private static int _allocatedSize;

        [Conditional(DEBUG)]
        internal static void Initialize()
        {
            _allocatedSize = 0;
        }

        [Conditional(DEBUG)]
        internal static void RegisterNewAllocatedBytes(int newAllocatedByteLength)
        {
            if(newAllocatedByteLength == 0) {
                return;
            }
            _allocatedSize += newAllocatedByteLength;
            Debug.WriteLine($"new alloc {newAllocatedByteLength,8} bytes, all : {_allocatedSize,8} bytes");
        }

        [Conditional(DEBUG)]
        internal static void RegisterReleasedBytes(int releasedByteLength)
        {
            if(releasedByteLength == 0) {
                return;
            }
            _allocatedSize -= releasedByteLength;
            Debug.WriteLine($"release {releasedByteLength,8} bytes, all : {_allocatedSize,8} bytes.");
        }

        [Conditional(DEBUG)]
        internal static void AssertResourceReleased()
        {
            if(_allocatedSize != 0) { throw new Exception("Memory leak !! Some unmanaged memory is not released."); }
        }
    }
}
