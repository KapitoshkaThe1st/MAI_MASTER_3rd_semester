using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rijndael
{
    public static class WordOperations
    {
        public static uint MakeWord(byte b0, byte b1, byte b2, byte b3)
        {
            return ((((((uint)b3 << 8) | (uint)b2) << 8) | (uint)b1) << 8) | b0;
        }

        public static byte GetByte(uint word, int i)
        {
            return (byte)((word >> (i * 8)) & 0xFFU);
        }

        public static uint SetByte(uint word, byte b, int i)
        {
            int offset = i * 8;
            return (word & ~(0xFFU << offset)) | ((uint)b << offset);
        }
    }
}
