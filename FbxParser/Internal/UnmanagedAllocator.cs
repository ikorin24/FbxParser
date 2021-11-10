#nullable enable
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace FbxTools.Internal
{
    internal static class UnmanagedAllocator
    {
        [ThreadStatic]
        private static SharedHandle _sharedLarge;

        private const int SelfOwnThreshold = 128;   // It must be smaller than 'SharedSize'
        private const int SharedSize = 4096;

        public static UnmanagedHandle Alloc(int byteSize)
        {
            if(byteSize == 0) {
                return UnmanagedHandle.Null;
            }
            if(byteSize > SelfOwnThreshold) {
                UnmanagedMemoryHelper.RegisterNewAllocatedBytes(byteSize);
                return UnmanagedHandle.Own(Marshal.AllocHGlobal(byteSize), byteSize);
            }
            return _sharedLarge.AllocOrBorrow(byteSize);
        }

        public static void CleanUpSharedHandle()
        {
            // This method must be called when the parser is complete.

            _sharedLarge.CleanUp();
        }

        public static void Free(UnmanagedHandle handle)
        {
            if(handle.IsOwner) {
                UnmanagedMemoryHelper.RegisterReleasedBytes(handle.OwnedSize);
                Marshal.FreeHGlobal(handle.Ptr);
            }
        }

        private struct SharedHandle
        {
            private UnmanagedHandle _shared;
            private int _sharedOffset;
            private int _availableShared;

            public UnmanagedHandle AllocOrBorrow(int byteSize)
            {
                if(byteSize > _availableShared) {
#if DEBUG
                    if(_availableShared != 0) {
                        Debug.WriteLine($"Unused memory: {_availableShared} bytes");
                    }
#endif

                    UnmanagedMemoryHelper.RegisterNewAllocatedBytes(SharedSize);
                    var shared = UnmanagedHandle.Own(Marshal.AllocHGlobal(SharedSize), SharedSize);
                    _shared = shared;
                    _sharedOffset = byteSize;
                    _availableShared = SharedSize - byteSize;
                    return shared;
                }
                else {
                    var ptr = _shared.Ptr + _sharedOffset;
                    _sharedOffset += byteSize;
                    _availableShared -= byteSize;
                    return UnmanagedHandle.Borrow(ptr);
                }
            }

            public void CleanUp()
            {
                _shared = UnmanagedHandle.Null;
                _sharedOffset = 0;
                _availableShared = 0;
            }
        }
    }
}
