﻿#nullable enable
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.IO.Compression;
using FbxTools.Internal;

namespace FbxTools
{
    /// <summary>Fbx parser class</summary>
    public static class FbxParser
    {
        // MemoryStreamUM instance can be reused after disposing.
        [ThreadStatic]
        private static MemoryStreamUM? _ms;

        // No allocation by optimization. Bytes data are embedded in dll.
        private static ReadOnlySpan<byte> MagicWord => new byte[23]
        {
            0x4B, 0x61, 0x79, 0x64, 0x61, 0x72, 0x61, 0x20,
            0x46, 0x42, 0x58, 0x20, 0x42, 0x69, 0x6E, 0x61,
            0x72, 0x79, 0x20, 0x20, 0x00, 0x1a, 0x00,
        };

        private const byte BOOL_PROPERTY = (byte)'C';
        private const byte INT16_PROPERTY = (byte)'Y';
        private const byte INT32_PROPERTY = (byte)'I';
        private const byte FLOAT_PROPERTY = (byte)'F';
        private const byte DOUBLE_PROPERTY = (byte)'D';
        private const byte INT64_PROPERTY = (byte)'L';

        private const byte BOOL_ARRAY_PROPERTY = (byte)'b';
        private const byte INT32_ARRAY_PROPERTY = (byte)'i';
        private const byte FLOAT_ARRAY_PROPERTY = (byte)'f';
        private const byte DOUBLE_ARRAY_PROPERTY = (byte)'d';
        private const byte INT64_ARRAY_PROPERTY = (byte)'l';

        private const byte STRING_PROPERTY = (byte)'S';
        private const byte RAW_BINARY_PROPERTY = (byte)'R';

        /// <summary>Prase <see cref="Stream"/> to <see cref="FbxObject"/></summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static unsafe FbxObject Parse(Stream stream)
        {
            if(stream is null) {
                throw new ArgumentNullException(nameof(stream));
            }
            var reader = new Reader(stream);

            ParseHeader(reader, out var version);
            UnsafeRawList<FbxNode_> nodes = default;
            try {
                while(true) {
                    if(!ParseNodeRecord(reader, version, out var node)) { break; }
                    nodes.Add(node);
                }
                UnmanagedAllocator.CleanUpSharedHandle();
                return new FbxObject(nodes);
            }
            catch {
                nodes.Dispose();
                throw;
            }
        }

        [SkipLocalsInit]
        private static void ParseHeader(Reader reader, out int version)
        {
            Span<byte> magic = stackalloc byte[MagicWord.Length];
            reader.Read(magic);
            if(magic.SequenceEqual(magic) == false) {
                throw new FormatException("Invalid header");
            }
            reader.Int32(out version);
        }

        private static unsafe bool ParseNodeRecord(Reader reader, int version, out FbxNode_ node)
        {
            ulong endOfRecord;
            ulong propertyCount;
            ulong propertyListLen;
            byte nameLen;
            if(version >= 7400 && version < 7500) {
                reader.UInt32(out var eor);
                endOfRecord = eor;
                reader.UInt32(out var pc);
                propertyCount = pc;
                reader.UInt32(out var pll);
                propertyListLen = pll;
                reader.Byte(out nameLen);
            }
            else if(version >= 7500 && version < 7600) {
                reader.UInt64(out endOfRecord);
                reader.UInt64(out propertyCount);
                reader.UInt64(out propertyListLen);
                reader.Byte(out nameLen);
            }
            else {
                throw new NotSupportedException($"Format version '{version}' is not supported.");
            }
            var isNullRecord = (endOfRecord == 0) && (propertyCount == 0) && (propertyListLen == 0) && (nameLen == 0);
            if(isNullRecord) {
                node = default;
                return false;
            }

            node = default;
            try {
                node = new FbxNode_(new RawStringMem(nameLen), (int)propertyCount);
                var name = node.NameInternal;
                var properties = node.PropertiesInternal;
                reader.Read(name);
                SanitizeString(name);
                for(int i = 0; i < (int)propertyCount; i++) {
                    ParseProperty(reader, ref properties[i]);
                }
                var hasChildren = (ulong)reader.BaseStream.Position != endOfRecord;
                var hasNullRecord = hasChildren || propertyCount == 0;
                if(hasChildren || hasNullRecord) {
                    while(true) {
                        if(!ParseNodeRecord(reader, version, out var child)) { break; }
                        node.AddChild(child);
                    }
                }
            }
            catch {
                node.Free();
                node = default;
                throw;
            }
            return true;
        }

        private static void ParseFooter(Reader reader)
        {
            // Footer does not have important information.
            // No problem if skip parsing this section.
        }

        private unsafe static void ParseProperty(Reader reader, ref FbxProperty property)
        {
            reader.Byte(out var propertyType);
            switch(propertyType) {
                case INT16_PROPERTY: {
                    reader.Int16(out var value);
                    property.SetInt16(value);
                    break;
                }
                case BOOL_PROPERTY: {
                    reader.Byte(out var value);
                    // two types format exist. (Oh my gosh !! Fuuuuuuuu*k !!)
                    // blender             -> true/false = 0x01/0x00
                    // Autodesk production -> true/false = 'Y'/'T' = 0x59/0x54
                    property.SetBool(value != 0x00 && value != 0x54);
                    break;
                }
                case INT32_PROPERTY: {
                    reader.Int32(out var value);
                    property.SetInt32(value);
                    break;
                }
                case FLOAT_PROPERTY: {
                    reader.Float(out var value);
                    property.SetFloat(value);
                    break;
                }
                case DOUBLE_PROPERTY: {
                    reader.Double(out var value);
                    property.SetDouble(value);
                    break;
                }
                case INT64_PROPERTY: {
                    reader.Int64(out var value);
                    property.SetInt64(value);
                    break;
                }
                case STRING_PROPERTY: {
                    reader.Int32(out var len);
                    RawStringMem str = default;
                    try {
                        str = new RawStringMem(len);
                        var span = str.AsSpan();
                        reader.Read(span);
                        SanitizeString(span);
                        property.SetString(str);
                    }
                    catch {
                        str.Dispose();
                        throw;
                    }
                    break;
                }
                case BOOL_ARRAY_PROPERTY: {
                    reader.Int32(out var len);
                    reader.UInt32(out var encoded);
                    reader.UInt32(out var compressedSize);
                    UnmanagedHandle handle = default;       // don't release
                    int arrayLen;
                    try {
                        if(encoded != 0) {
                            handle = Decode<bool>(reader, len, (int)compressedSize);
                            arrayLen = len;
                        }
                        else {
                            var byteLen = (int)compressedSize;
                            arrayLen = byteLen / sizeof(bool);
                            handle = UnmanagedAllocator.Alloc(byteLen);
                            var span = new Span<byte>(handle.GetPtr(), byteLen);
                            reader.Read(span);
                        }
                        property.SetBoolArray(handle, arrayLen);
                    }
                    catch {
                        UnmanagedAllocator.Free(handle);
                        throw;
                    }
                    break;
                }
                case INT32_ARRAY_PROPERTY: {
                    reader.Int32(out var len);
                    reader.UInt32(out var encoded);
                    reader.UInt32(out var compressedSize);
                    UnmanagedHandle handle = default;       // don't release
                    int arrayLen;
                    try {
                        if(encoded != 0) {
                            handle = Decode<int>(reader, len, (int)compressedSize);
                            arrayLen = len;
                        }
                        else {
                            var byteLen = (int)compressedSize;
                            arrayLen = byteLen / sizeof(int);
                            handle = UnmanagedAllocator.Alloc(byteLen);
                            var span = new Span<byte>(handle.GetPtr(), byteLen);
                            reader.Read(span);
                        }
                        property.SetInt32Array(handle, arrayLen);
                    }
                    catch {
                        UnmanagedAllocator.Free(handle);
                        throw;
                    }
                    break;
                }
                case FLOAT_ARRAY_PROPERTY: {
                    reader.Int32(out var len);
                    reader.UInt32(out var encoded);
                    reader.UInt32(out var compressedSize);
                    UnmanagedHandle handle = default;       // don't release
                    int arrayLen;
                    try {
                        if(encoded != 0) {
                            handle = Decode<float>(reader, len, (int)compressedSize);
                            arrayLen = len;
                        }
                        else {
                            var byteLen = (int)compressedSize;
                            arrayLen = byteLen / sizeof(float);
                            handle = UnmanagedAllocator.Alloc(byteLen);
                            var span = new Span<byte>(handle.GetPtr(), byteLen);
                            reader.Read(span);
                        }
                        property.SetFloatArray(handle, arrayLen);
                    }
                    catch {
                        UnmanagedAllocator.Free(handle);
                        throw;
                    }
                    break;
                }
                case DOUBLE_ARRAY_PROPERTY: {
                    reader.Int32(out var len);
                    reader.UInt32(out var encoded);
                    reader.UInt32(out var compressedSize);
                    UnmanagedHandle handle = default;       // don't release
                    int arrayLen;
                    try {
                        if(encoded != 0) {
                            handle = Decode<double>(reader, len, (int)compressedSize);
                            arrayLen = len;
                        }
                        else {
                            var byteLen = (int)compressedSize;
                            arrayLen = byteLen / sizeof(double);
                            handle = UnmanagedAllocator.Alloc(byteLen);
                            var span = new Span<byte>(handle.GetPtr(), byteLen);
                            reader.Read(span);
                        }
                        property.SetDoubleArray(handle, arrayLen);
                    }
                    catch {
                        UnmanagedAllocator.Free(handle);
                        throw;
                    }
                    break;
                }
                case INT64_ARRAY_PROPERTY: {
                    reader.Int32(out var len);
                    reader.UInt32(out var encoded);
                    reader.UInt32(out var compressedSize);
                    UnmanagedHandle handle = default;       // don't release
                    int arrayLen;
                    try {
                        if(encoded != 0) {
                            handle = Decode<long>(reader, len, (int)compressedSize);
                            arrayLen = len;
                        }
                        else {
                            var byteLen = (int)compressedSize;
                            arrayLen = byteLen / sizeof(long);
                            handle = UnmanagedAllocator.Alloc(byteLen);
                            var span = new Span<byte>(handle.GetPtr(), byteLen);
                            reader.Read(span);
                        }
                        property.SetInt64Array(handle, arrayLen);
                    }
                    catch {
                        UnmanagedAllocator.Free(handle);
                        throw;
                    }
                    break;
                }
                case RAW_BINARY_PROPERTY: {
                    reader.Int32(out var len);
                    UnmanagedHandle handle = default;
                    try {
                        handle = UnmanagedAllocator.Alloc(len);
                        var span = new Span<byte>(handle.GetPtr(), len);
                        reader.Read(span);
                        property.SetByteArray(handle, len);
                    }
                    catch {
                        UnmanagedAllocator.Free(handle);
                        throw;
                    }
                    break;
                }
                default: {
                    Debug.WriteLine($"[Skip Unknow Type Property] Position : {reader.BaseStream.Position}, type : {propertyType}");
                    break;
                }
            }

            #region (local func) Decode compressed array data
            static UnmanagedHandle Decode<T>(Reader reader, int arrayLength, int compressedSize) where T : unmanaged
            {
                const int deflateMetaDataSize = 2;
                reader.Int16(out var _);            // deflateMetaData (not be used)

                // read compressed data
                using var buf = new UnsafeRawArray<byte>(compressedSize - deflateMetaDataSize);
                reader.Read(buf.AsSpan());

                // decompress data
                var ptr = (byte*)buf.Ptr;
                using var ms = (_ms is null) ? (_ms = new MemoryStreamUM(ptr, buf.Length)) : _ms.RefreshInstance(ptr, buf.Length);
                using var ds = new DeflateStream(ms, CompressionMode.Decompress);

                var decodedByteLen = arrayLength * sizeof(T);
                var handle = UnmanagedAllocator.Alloc(decodedByteLen);
                try {
                    var decodedSpan = new Span<byte>(handle.GetPtr(), decodedByteLen);
                    while(true) {
                        var readlen = ds.Read(decodedSpan);
                        if(readlen == 0) { break; }
                        decodedSpan = decodedSpan.Slice(readlen);
                    }
                }
                catch {
                    UnmanagedAllocator.Free(handle);
                    throw;
                }
                return handle;
            }
            #endregion
        }

        private static void SanitizeString(Span<byte> bytes)
        {
            for(int i = 0; i < bytes.Length; i++) {
                if(bytes[i] == 0x00 || bytes[i] == 0x01) {
                    bytes[i] = (byte)':';       // replace (0x00, 0x01) into 0x3a ':'
                }
            }
        }
    }



#if NETSTANDARD2_0
    internal static class DeflateStreamExtension
    {
        public static int Read(this DeflateStream source, Span<byte> buffer)
        {
            var array = System.Buffers.ArrayPool<byte>.Shared.Rent(buffer.Length);
            try {
                var readlen = source.Read(array, 0, buffer.Length);
                array.AsSpan(0, buffer.Length).CopyTo(buffer);
                return readlen;
            }
            finally {
                System.Buffers.ArrayPool<byte>.Shared.Return(array);
            }
        }
    }
#endif
}
