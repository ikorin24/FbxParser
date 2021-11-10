#nullable enable
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

#if NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_1
using System.Runtime.InteropServices;
#endif

namespace FbxTools.Internal
{
    internal readonly ref struct Reader
    {
        public readonly Stream BaseStream;
#if NETSTANDARD2_0

        private const int SharedArraySize = sizeof(ulong);
        [ThreadStatic]
        private static byte[]? _sharedArray;

        private static byte[] SharedArray => _sharedArray ??= new byte[SharedArraySize];
#endif

        public Reader(Stream stream)
        {
            BaseStream = stream;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Read(Span<byte> buffer)
        {
            try {
                ReadPrivate(buffer);
            }
            catch {
                throw;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadPrivate(Span<byte> buffer)
        {
#if NETSTANDARD2_0
            if(buffer.Length <= SharedArraySize) {
                var sharedArray = SharedArray;
                if(BaseStream.Read(sharedArray, 0, buffer.Length) != buffer.Length) {
                    ThrowEndOfStream();
                }
                sharedArray.AsSpan(0, buffer.Length).CopyTo(buffer);
            }
            else {
                var array = System.Buffers.ArrayPool<byte>.Shared.Rent(buffer.Length);
                try {
                    if(BaseStream.Read(array, 0, buffer.Length) != buffer.Length) {
                        ThrowEndOfStream();
                    }
                    array.AsSpan(0, buffer.Length).CopyTo(buffer);
                }
                finally {
                    System.Buffers.ArrayPool<byte>.Shared.Return(array);
                }
            }
#else   // NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_1
            if(BaseStream.Read(buffer) != buffer.Length) {
                ThrowEndOfStream();
            }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Byte(out byte value)
        {
            ReadPrimitive(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Int16(out short value)
        {
            ReadPrimitive(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Int32(out int value)
        {
            ReadPrimitive(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UInt32(out uint value)
        {
            ReadPrimitive(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Int64(out long value)
        {
            ReadPrimitive(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UInt64(out ulong value)
        {
            ReadPrimitive(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Float(out float value)
        {
            ReadPrimitive(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Double(out double value)
        {
            ReadPrimitive(out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReadPrimitive<T>(out T value) where T : unmanaged
        {
#if NET5_0_OR_GREATER
            Unsafe.SkipInit(out value);
            Read(MemoryMarshal.CreateSpan(ref Unsafe.As<T, byte>(ref value), Unsafe.SizeOf<T>()));
#elif NETSTANDARD2_1 || NETCOREAPP3_1
            value = default;
            Read(MemoryMarshal.CreateSpan(ref Unsafe.As<T, byte>(ref value), Unsafe.SizeOf<T>()));
#else // NETSTANDARD2_0
            unsafe {
                T buf = default;
                Read(new Span<byte>(&buf, sizeof(T)));
                value = buf;
            }
#endif
        }

        [DoesNotReturn]
        private static void ThrowEndOfStream() => throw new EndOfStreamException();
    }
}
