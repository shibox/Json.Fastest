
using FastBlockCopy;

namespace JShibo.Serialization.Common
{
    public class FastWriteName
    {

        public unsafe static void WriteUnsafe(string name, char[] buffer, int position)
        {
            fixed (char* src = name, dst = &buffer[position])
                FastBuffer.UnsafeBlockCopy((byte*)src, (byte*)dst, name.Length * 2);
        }

        public unsafe static void WriteUnsafe(char* tsrc, char* tdst, int size)
        {
            FastBuffer.UnsafeBlockCopy((byte*)tsrc, (byte*)tdst, size);
        }

    }
}
