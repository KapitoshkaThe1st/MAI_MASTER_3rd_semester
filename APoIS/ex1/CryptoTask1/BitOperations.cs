using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTask1
{
    class BitOperations
    {
        private const int bitSize = sizeof(uint) * 8;
        public static void ThrowIfIndexOutOfRange(int index)
        {
            if (index < 0 || index > bitSize)
            {
                throw new IndexOutOfRangeException($"Index {index} out of bound for type uint");
            }
        }

        public static uint GetBit(uint num, int index)
        {
            ThrowIfIndexOutOfRange(index);
            return (num >> index) & 1;
        }

        public static bool Test(uint num, int index)
        {
            return Convert.ToBoolean(GetBit(num, index));
        }

        public static uint SetBit(uint num, int index)
        {
            ThrowIfIndexOutOfRange(index);
            return num | ((uint)1 << index);
        }

        public static uint ResetBit(uint num, int index)
        {
            ThrowIfIndexOutOfRange(index);
            return num & ~((uint)1 << index);
        }

        public static uint ClearLeastSignificantBits(uint num, int count)
        {
            return (num >> count) << count;
        }

        public static uint ClearMostSignificantBits(uint num, int count)
        {
            return (num << count) >> count;
        }

        public static uint DropMiddle(uint num, int count)
        {
            int bitSizeMinusCount = (bitSize - count);
            return ((num >> bitSizeMinusCount) << count) | ClearMostSignificantBits(num, bitSizeMinusCount);
        }
        
        public static uint RetrieveMiddle(uint num, int count)
        {
            return (num << count) >> (count * 2);
        }

        public static uint Parity(uint num)
        {
            // https://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetKernighan
            uint result = 0;
            while(num != 0)
            {
                num &= num - 1;
                result ^= 1;
            }

            return result;
        }

        public static uint RotateLeft(uint num, int count)
        {
            return (num << count) | (num >> (bitSize - count));
        }

        public static uint RotateRight(uint num, int count)
        {
            return (num << (bitSize - count)) | (num >> count);
        }

        public static uint SwapBits(uint num, int index1, int index2)
        {
            ThrowIfIndexOutOfRange(index1);
            ThrowIfIndexOutOfRange(index2);

            uint bit1 = GetBit(num, index1);
            uint bit2 = GetBit(num, index2);

            uint needFlip = bit1 ^ bit2;

            uint flipMask = (needFlip << index1) | (needFlip << index2);

            return num ^ flipMask;
        }
    }
}
