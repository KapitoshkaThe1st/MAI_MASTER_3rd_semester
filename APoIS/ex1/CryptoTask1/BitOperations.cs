using System;

namespace CryptoTask1
{
    public class BitOperations
    {
        private const int _bitSize = sizeof(uint) * 8;
        private static void ThrowIfCountOutOfRange(int count)
        {
            if (count < 0)
            {
                throw new IndexOutOfRangeException($"Count {count} must be non-negative");
            }
        }

        private static void ThrowIfIndexOutOfRange(int index)
        {
            if (index < 0 || index >= _bitSize)
            {
                throw new IndexOutOfRangeException($"Index {index} out of bound for type uint");
            }
        }

        private static void ThrowIfIndexOutOfRangeForDropOperations(int index)
        {
            if (index < 0 || index > _bitSize / 2)
            {
                throw new IndexOutOfRangeException($"Index {index} out of bound for type uint for drop/retrieve middle operations");
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

        private static uint TrueLeftShift(uint num, int count)
        {
            if (count >= 32)
                return 0;
            return num << count;
        }

        private static uint TrueRightShift(uint num, int count)
        {
            if (count >= 32)
                return 0;
            return num >> count;
        }

        public static uint ClearLeastSignificantBits(uint num, int count)
        {
            ThrowIfCountOutOfRange(count);
            return TrueLeftShift(TrueRightShift(num, count), count);
        }

        public static uint ClearMostSignificantBits(uint num, int count)
        {
            ThrowIfCountOutOfRange(count);
            return TrueRightShift(TrueLeftShift(num, count), count);
        }

        public static uint OuterBits(uint num, int count)
        {
            ThrowIfIndexOutOfRangeForDropOperations(count);
            int bitSizeMinusCount = (_bitSize - count);

            // можно было тупо проверить если count = 0, то вернуть 0 и не заморачиваться с True*Shift'ами, а обойтись обычными, но так тоже норм
            uint leftPart = TrueLeftShift(TrueRightShift(num, bitSizeMinusCount), count);
            uint rightPart = ClearMostSignificantBits(num, bitSizeMinusCount);

            return leftPart | rightPart;
        }
        
        public static uint InnerBits(uint num, int count)
        {
            ThrowIfIndexOutOfRangeForDropOperations(count);
            return TrueRightShift(TrueLeftShift(num, count), count * 2);
        }

        // Computing parity(1 if an odd number of bits set, 0 otherwise)
        public static uint Parity(uint num)
        {
            // https://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetKernighan
            uint result = 0;

            // каждая итерация обнуляет младший установленный бит
            while(num != 0)
            {
                num &= num - 1;
                result ^= 1;
            }

            return result;
        }

        public static uint RotateLeft(uint num, int count)
        {
            ThrowIfCountOutOfRange(count);
            count %= _bitSize;
            return (num << count) | (num >> (_bitSize - count));
        }

        public static uint RotateRight(uint num, int count)
        {
            ThrowIfCountOutOfRange(count);
            count %= _bitSize;
            return (num << (_bitSize - count)) | (num >> count);
        }
    }
}
