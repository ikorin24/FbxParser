using Xunit;
using System;
using System.Linq;
using FbxTools;

namespace UnitTest
{
    public class OtherTest
    {
        [Fact]
        public unsafe void RawStringSlice()
        {
            const int memLen = 5;
            var mem = stackalloc byte[memLen]
            {
                (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e',
            };
            var abcde = new RawString((IntPtr)mem, memLen);

            var abc = new RawString((IntPtr)mem, 3);
            var cd = new RawString((IntPtr)(mem + 2), 2);
            var de = new RawString((IntPtr)(mem + 3), 2);

            AssertEqual(abcde.Slice(0, abcde.Length), abcde);
            AssertEqual(abcde.Slice(0, 3), abc);
            AssertEqual(abcde.Slice(2, 2), cd);
            AssertEqual(abcde.Slice(3, 2), de);
            AssertEqual(abcde.Slice(3), de);
            AssertEqual(abcde.Slice(0), abcde);
            Assert.True(abcde.Slice(5).IsEmpty);
            Assert.True(abcde.Slice(5, 0).IsEmpty);

            AssertEqual(abcde.AsSpan(0, abcde.Length), abcde);
            AssertEqual(abcde.AsSpan(0, 3), abc);
            AssertEqual(abcde.AsSpan(2, 2), cd);
            AssertEqual(abcde.AsSpan(3, 2), de);
            AssertEqual(abcde.AsSpan(3), de);
            AssertEqual(abcde.AsSpan(0), abcde);
            Assert.True(abcde.AsSpan(5).IsEmpty);
            Assert.True(abcde.AsSpan(5, 0).IsEmpty);
        }

        [Fact]
        public unsafe void RawArraySlice()
        {
            const int memLen = 5;
            var mem = stackalloc int[memLen] { 0, 1, 2, 3, 4 };
            var array = new RawArray<int>((IntPtr)mem, memLen);
            var array03 = new RawArray<int>((IntPtr)mem, 3);
            var array22 = new RawArray<int>((IntPtr)(mem + 2), 2);
            var array32 = new RawArray<int>((IntPtr)(mem + 3), 2);

            AssertEqual(array.Slice(0, array.Length), array);
            AssertEqual(array.Slice(0, 3), array03);
            AssertEqual(array.Slice(2, 2), array22);
            AssertEqual(array.Slice(3, 2), array32);
            AssertEqual(array.Slice(3), array32);
            AssertEqual(array.Slice(0), array);
            Assert.True(array.Slice(5).IsEmpty);
            Assert.True(array.Slice(5, 0).IsEmpty);

            AssertEqual(array.AsSpan(0, array.Length), array);
            AssertEqual(array.AsSpan(0, 3), array03);
            AssertEqual(array.AsSpan(2, 2), array22);
            AssertEqual(array.AsSpan(3, 2), array32);
            AssertEqual(array.AsSpan(3), array32);
            AssertEqual(array.AsSpan(0), array);
            Assert.True(array.AsSpan(5).IsEmpty);
            Assert.True(array.AsSpan(5, 0).IsEmpty);
        }

        private static void AssertEqual(RawString str1, RawString str2)
        {
            Assert.True(str1.SequenceEqual(str2));
            Assert.True(str1.AsSpan().SequenceEqual(str2.AsSpan()));
        }

        private static void AssertEqual(ReadOnlySpan<byte> str1, RawString str2)
        {
            Assert.True(str1.SequenceEqual(str2.AsSpan()));
        }

        private static void AssertEqual<T>(RawArray<T> array1, RawArray<T> array2) where T : unmanaged, IEquatable<T>
        {
            Assert.True(array1.SequenceEqual(array2));
            Assert.True(array1.AsSpan().SequenceEqual(array2.AsSpan()));
        }

        private static void AssertEqual<T>(ReadOnlySpan<T> array1, RawArray<T> array2) where T : unmanaged, IEquatable<T>
        {
            Assert.True(array1.SequenceEqual(array2.AsSpan()));
        }
    }
}
