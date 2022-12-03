using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DES
{
    class BitOperations
    {
        public static uint Parity(byte num)
        {
            // https://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetKernighan
            uint result = 0;

            // каждая итерация обнуляет младший установленный бит
            while (num != 0)
            {
                num &= (byte)(num - 1);
                result ^= 1;
            }

            return result;
        }

        private static uint[] _masks = new uint[]
        {
            0,
            0b1,
            0b11,
            0b111,
            0b1111,
            0b11111,
            0b111111,
            0b1111111,
            0b11111111,
            0b111111111,
            0b1111111111,
            0b11111111111,
            0b111111111111,
            0b1111111111111,
            0b11111111111111,
            0b111111111111111,
            0b1111111111111111,
            0b11111111111111111,
            0b111111111111111111,
            0b1111111111111111111,
            0b11111111111111111111,
            0b111111111111111111111,
            0b1111111111111111111111,
            0b11111111111111111111111,
            0b111111111111111111111111,
            0b1111111111111111111111111,
            0b11111111111111111111111111,
            0b111111111111111111111111111,
            0b1111111111111111111111111111,
            0b11111111111111111111111111111,
            0b111111111111111111111111111111,
            0b1111111111111111111111111111111
        };

        public static uint RotateLeft(uint num, int bitLength, int count)
        {
            uint mask = _masks[bitLength];
            return (num << count) & mask | (num >> (bitLength - count));
        }
    }
}
