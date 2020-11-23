#nullable enable
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace FbxTools.Internal
{
    internal readonly ref struct Reader
    {
        public readonly Stream BaseStream;

        public Reader(Stream stream)
        {
            BaseStream = stream;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Read(Span<byte> buffer)
        {
#if NETSTANDARD2_0
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
#else   // NET5_0 || NETCOREAPP3_1 || NETSTANDARD2_1
            if(BaseStream.Read(buffer) != buffer.Length) {
                ThrowEndOfStream();
            }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Byte(out byte value)
        {
#if NET5_0
            Unsafe.SkipInit(out value);
            Read(MemoryMarshal.CreateSpan(ref value, sizeof(byte)));
#elif NETSTANDARD2_1 || NETCOREAPP3_1
                value = default;
                Read(MemoryMarshal.CreateSpan(ref value, sizeof(byte)));
#else // NETSTANDARD2_0
                unsafe {
                    byte buf = default;
                    Read(new Span<byte>(&buf, sizeof(byte)));
                    value = buf;
                }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Int16(out short value)
        {
#if NET5_0
            Unsafe.SkipInit(out value);
            Read(MemoryMarshal.CreateSpan(ref Unsafe.As<short, byte>(ref value), sizeof(short)));
#elif NETSTANDARD2_1 || NETCOREAPP3_1
                value = default;
                Read(MemoryMarshal.CreateSpan(ref Unsafe.As<short, byte>(ref value), sizeof(short)));
#else // NETSTANDARD2_0
                unsafe {
                    short buf = default;
                    Read(new Span<byte>(&buf, sizeof(short)));
                    value = buf;
                }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Int32(out int value)
        {
#if NET5_0
            Unsafe.SkipInit(out value);
            Read(MemoryMarshal.CreateSpan(ref Unsafe.As<int, byte>(ref value), sizeof(int)));
#elif NETSTANDARD2_1 || NETCOREAPP3_1
                value = default;
                Read(MemoryMarshal.CreateSpan(ref Unsafe.As<int, byte>(ref value), sizeof(int)));
#else // NETSTANDARD2_0
                unsafe {
                    int buf = default;
                    Read(new Span<byte>(&buf, sizeof(int)));
                    value = buf;
                }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UInt32(out uint value)
        {
#if NET5_0
            Unsafe.SkipInit(out value);
            Read(MemoryMarshal.CreateSpan(ref Unsafe.As<uint, byte>(ref value), sizeof(uint)));
#elif NETSTANDARD2_1 || NETCOREAPP3_1
                value = default;
                Read(MemoryMarshal.CreateSpan(ref Unsafe.As<uint, byte>(ref value), sizeof(uint)));
#else // NETSTANDARD2_0
                unsafe {
                    uint buf = default;
                    Read(new Span<byte>(&buf, sizeof(uint)));
                    value = buf;
                }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Int64(out long value)
        {
#if NET5_0
            Unsafe.SkipInit(out value);
            Read(MemoryMarshal.CreateSpan(ref Unsafe.As<long, byte>(ref value), sizeof(long)));
#elif NETSTANDARD2_1 || NETCOREAPP3_1
                value = default;
                Read(MemoryMarshal.CreateSpan(ref Unsafe.As<long, byte>(ref value), sizeof(long)));
#else // NETSTANDARD2_0
                unsafe {
                    long buf = default;
                    Read(new Span<byte>(&buf, sizeof(long)));
                    value = buf;
                }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UInt64(out ulong value)
        {
#if NET5_0
            Unsafe.SkipInit(out value);
            Read(MemoryMarshal.CreateSpan(ref Unsafe.As<ulong, byte>(ref value), sizeof(ulong)));
#elif NETSTANDARD2_1 || NETCOREAPP3_1
                value = default;
                Read(MemoryMarshal.CreateSpan(ref Unsafe.As<ulong, byte>(ref value), sizeof(ulong)));
#else // NETSTANDARD2_0
                unsafe {
                    ulong buf = default;
                    Read(new Span<byte>(&buf, sizeof(ulong)));
                    value = buf;
                }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Float(out float value)
        {
#if NET5_0
            Unsafe.SkipInit(out value);
            Read(MemoryMarshal.CreateSpan(ref Unsafe.As<float, byte>(ref value), sizeof(float)));
#elif NETSTANDARD2_1 || NETCOREAPP3_1
                value = default;
                Read(MemoryMarshal.CreateSpan(ref Unsafe.As<float, byte>(ref value), sizeof(float)));
#else // NETSTANDARD2_0
                unsafe {
                    float buf = default;
                    Read(new Span<byte>(&buf, sizeof(float)));
                    value = buf;
                }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Double(out double value)
        {
#if NET5_0
            Unsafe.SkipInit(out value);
            Read(MemoryMarshal.CreateSpan(ref Unsafe.As<double, byte>(ref value), sizeof(double)));
#elif NETSTANDARD2_1 || NETCOREAPP3_1
                value = default;
                Read(MemoryMarshal.CreateSpan(ref Unsafe.As<double, byte>(ref value), sizeof(double)));
#else // NETSTANDARD2_0
                unsafe {
                    double buf = default;
                    Read(new Span<byte>(&buf, sizeof(double)));
                    value = buf;
                }
#endif
        }

#if NET5_0
        [DoesNotReturn]
#endif
        private static void ThrowEndOfStream() => throw new EndOfStreamException();
    }
}
