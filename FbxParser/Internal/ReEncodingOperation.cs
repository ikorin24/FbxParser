#nullable enable
using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace FbxTools.Internal
{
    internal static unsafe class ReEncodingOperation
    {
        [SkipLocalsInit]
        public static TResult Func<TArg, TResult>(string str, TArg arg, delegate*<ReadOnlySpan<byte>, TArg, TResult> func, delegate*<TResult> fallback)
        {
            if(string.IsNullOrEmpty(str)) {
                return func(ReadOnlySpan<byte>.Empty, arg);
            }
            var byteCount = Encoding.ASCII.GetByteCount(str);
            if(byteCount != str.Length) {
                // The string contains non-ASCII charactors.
                return fallback();
            }

            if(byteCount <= 128) {
                var buf = stackalloc byte[byteCount];
                fixed(char* c = str) {
                    Encoding.ASCII.GetBytes(c, str.Length, buf, byteCount);
                }
                return func(new ReadOnlySpan<byte>(buf, byteCount), arg);
            }
            else {
                byte* buf = null;
                try {
                    buf = (byte*)Marshal.AllocHGlobal(byteCount);
                    fixed(char* c = str) {
                        Encoding.ASCII.GetBytes(c, str.Length, buf, byteCount);
                    }
                    return func(new ReadOnlySpan<byte>(buf, byteCount), arg);
                }
                finally {
                    Marshal.FreeHGlobal(new IntPtr(buf));
                }
            }
        }
    }
}
