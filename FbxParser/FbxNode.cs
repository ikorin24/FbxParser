﻿#nullable enable
using System;
using System.Diagnostics;
using FbxTools.Internal;

namespace FbxTools
{
    /// <summary>Node structure of fbx</summary>
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public readonly unsafe struct FbxNode : IEquatable<FbxNode>
    {
        private readonly IntPtr _ptr;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private FbxNode_* Pointer => (FbxNode_*)_ptr;

        private string DebuggerDisplay() => _ptr != IntPtr.Zero ? ((FbxNode_*)_ptr)->DebuggerDisplay() : "null";

        internal static FbxNode Null => default;

        /// <summary>Get name of the node</summary>
        public RawString Name => ((FbxNode_*)_ptr)->Name;

        /// <summary>Get properties of the node</summary>
        public ReadOnlySpan<FbxProperty> Properties => ((FbxNode_*)_ptr)->Properties;

        /// <summary>Get children nodes</summary>
        public FbxNodeList Children => ((FbxNode_*)_ptr)->Children;

        internal FbxNode(FbxNode_* ptr)
        {
            _ptr = (IntPtr)ptr;
        }

        /// <summary>Find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <exception cref="InvalidOperationException">Children contains no matching node.</exception>
        /// <returns>a found node</returns>
        public readonly FbxNode FindChild(ReadOnlySpan<byte> nodeName) => FbxNode_.Find(Pointer->ChildrenInternal, nodeName);

        /// <summary>Find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <exception cref="InvalidOperationException">Children contains no matching node.</exception>
        /// <returns>a found node</returns>
        public readonly FbxNode FindChild(RawString nodeName) => FbxNode_.Find(Pointer->ChildrenInternal, nodeName.AsSpan());

        /// <summary>Find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <exception cref="InvalidOperationException">Children contains no matching node.</exception>
        /// <returns>a found node</returns>
        public readonly FbxNode FindChild(string nodeName) => FbxNode_.Find(Pointer->ChildrenInternal, nodeName);

        /// <summary>Find all children node of specified name. Returns empty array if not found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <returns>found children nodes</returns>
        public readonly FbxNode[] FindChildren(ReadOnlySpan<byte> nodeName) => FbxNode_.FindAll(Pointer->ChildrenInternal, nodeName);

        /// <summary>Find all children node of specified name. Returns empty array if not found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <returns>found children nodes</returns>
        public readonly FbxNode[] FindChildren(RawString nodeName) => FbxNode_.FindAll(Pointer->ChildrenInternal, nodeName.AsSpan());

        /// <summary>Find all children node of specified name. Returns empty array if not found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <returns>found children nodes</returns>
        public readonly FbxNode[] FindChildren(string nodeName) => FbxNode_.FindAll(Pointer->ChildrenInternal, nodeName);

        /// <summary>Try to find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <param name="node">a found node</param>
        /// <returns>found or not</returns>
        public readonly bool TryFindChild(ReadOnlySpan<byte> nodeName, out FbxNode node) => FbxNode_.TryFind(Pointer->ChildrenInternal, nodeName, out node);

        /// <summary>Try to find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <param name="node">a found node</param>
        /// <returns>found or not</returns>
        public readonly bool TryFindChild(RawString nodeName, out FbxNode node) => FbxNode_.TryFind(Pointer->ChildrenInternal, nodeName.AsSpan(), out node);

        /// <summary>Try to find a child node of specified name. Returns a first found node. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <param name="node">a found node</param>
        /// <returns>found or not</returns>
        public readonly bool TryFindChild(string nodeName, out FbxNode node) => FbxNode_.TryFind(Pointer->ChildrenInternal, nodeName, out node);

        /// <summary>Find an index of node of specified name. Returns an index of first found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <returns>an index of found node</returns>
        public readonly int FindChildIndex(ReadOnlySpan<byte> nodeName) => FbxNode_.FindIndex(Pointer->ChildrenInternal, nodeName);

        /// <summary>Find an index of node of specified name. Returns an index of first found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <returns>an index of found node</returns>
        public readonly int FindChildIndex(RawString nodeName) => FbxNode_.FindIndex(Pointer->ChildrenInternal, nodeName.AsSpan());

        /// <summary>Find an index of node of specified name. Returns an index of first found. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <returns>an index of found node</returns>
        public readonly int FindChildIndex(string nodeName) => FbxNode_.FindIndex(Pointer->ChildrenInternal, nodeName);

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <param name="buffer">buffer to store result</param>
        /// <returns>found count</returns>
        public readonly int FindChildIndexAll(ReadOnlySpan<byte> nodeName, Span<int> buffer) => FbxNode_.FindIndexAll(Pointer->ChildrenInternal, nodeName, buffer);

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <param name="buffer">buffer to store result</param>
        /// <returns>found count</returns>
        public readonly int FindChildIndexAll(RawString nodeName, Span<int> buffer) => FbxNode_.FindIndexAll(Pointer->ChildrenInternal, nodeName.AsSpan(), buffer);

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <param name="buffer">buffer to store result</param>
        /// <returns>found count</returns>
        public readonly int FindChildIndexAll(string nodeName, Span<int> buffer) => FbxNode_.FindIndexAll(Pointer->ChildrenInternal, nodeName, buffer);

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <returns>index list</returns>
        public readonly int[] FindChildIndexAll(ReadOnlySpan<byte> nodeName) => FbxNode_.FindIndexAll(Pointer->ChildrenInternal, nodeName);

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <returns>index list</returns>
        public readonly int[] FindChildIndexAll(RawString nodeName) => FbxNode_.FindIndexAll(Pointer->ChildrenInternal, nodeName.AsSpan());

        /// <summary>Find index list of specified name. (This method is not recursive, just find from children)</summary>
        /// <param name="nodeName">node name</param>
        /// <returns>index list</returns>
        public readonly int[] FindChildIndexAll(string nodeName) => FbxNode_.FindIndexAll(Pointer->ChildrenInternal, nodeName);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is FbxNode node && Equals(node);

        /// <inheritdoc/>
        public bool Equals(FbxNode other) => _ptr.Equals(other._ptr);

        /// <inheritdoc/>
        public override int GetHashCode() => _ptr.GetHashCode();

        /// <summary>Indicates whether the left and the right are equal</summary>
        /// <param name="left">left object to compere</param>
        /// <param name="right">right object to compere</param>
        /// <returns>true if equal</returns>
        public static bool operator ==(FbxNode left, FbxNode right) => left.Equals(right);

        /// <summary>Indicates whether the left and the right are not equal</summary>
        /// <param name="left">left object to compere</param>
        /// <param name="right">right object to compere</param>
        /// <returns>true if not equal</returns>
        public static bool operator !=(FbxNode left, FbxNode right) => !(left == right);
    }
}
