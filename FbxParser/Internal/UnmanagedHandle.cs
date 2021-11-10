#nullable enable
using System;

namespace FbxTools.Internal
{
    internal readonly struct UnmanagedHandle : IEquatable<UnmanagedHandle>
    {
        private readonly IntPtr _ptr;
        private readonly int _ownedSize;
        private readonly bool _isOwner;

        public static UnmanagedHandle Null => default;
        public IntPtr Ptr => _ptr;
        public int OwnedSize => _ownedSize;
        public bool IsOwner => _isOwner;

        public bool IsNull => _ptr == IntPtr.Zero;

        private UnmanagedHandle(IntPtr ptr, int ownedSize, bool isOwner)
        {
            _ptr = ptr;
            _ownedSize = ownedSize;
            _isOwner = isOwner;
        }

        public unsafe void* GetPtr() => (void*)_ptr;

        public static UnmanagedHandle Own(IntPtr ptr, int size) => new(ptr, size, true);
        public static UnmanagedHandle Borrow(IntPtr ptr) => new(ptr, 0, false);

        public override bool Equals(object? obj) => obj is UnmanagedHandle handle && Equals(handle);

        public bool Equals(UnmanagedHandle other) => _ptr == other._ptr && _ownedSize == other._ownedSize && _isOwner == other._isOwner;

        public override int GetHashCode() => HashCode.Combine(_ptr, _ownedSize, _isOwner);

        public static bool operator ==(UnmanagedHandle left, UnmanagedHandle right) => left.Equals(right);

        public static bool operator !=(UnmanagedHandle left, UnmanagedHandle right) => !(left == right);
    }
}
