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
    }
}
