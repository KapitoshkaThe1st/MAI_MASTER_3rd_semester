using System;
using System.Text.RegularExpressions;

namespace DES
{
    class Program
    {
        private static byte[] _ipTable = new byte[64] {
            58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8,
            57, 49, 41, 33, 25, 17, 9 , 1, 59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7,
        };


        private static byte[] _eTable = new byte[]
        {
            32,  1,  2,  3,  4,  5,
             4,  5,  6,  7,  8,  9,
             8,  9, 10, 11, 12, 13,
            12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29,
            28, 29, 30, 31, 32,  1
        };

        private static byte[] _pTable = new byte[]
        {
            16,  7, 20, 21,
            29, 12, 28, 17,
             1, 15, 23, 26,
             5, 18, 31, 10,
             2,  8, 24, 14,
            32, 27,  3,  9,
            19, 13, 30,  6,
            22, 11,  4, 25,
        };

        private static byte[] _iipTable = new byte[] {
            40, 8, 48, 16, 56, 24, 64, 32, 39, 7, 47, 15, 55, 23, 63, 31,
            38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13, 53, 21, 61, 29,
            36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27,
            34, 2, 42, 10, 50, 18, 58, 26, 33, 1, 41, 9 , 49, 17, 57, 25,
        };

        static byte[,,] __sTables = new byte[8, 4, 16] {
            { // 0
                {14, 4 , 13, 1 , 2 , 15, 11, 8 , 3 , 10, 6 , 12, 5 , 9 , 0 , 7 },
                {0 , 15, 7 , 4 , 14, 2 , 13, 1 , 10, 6 , 12, 11, 9 , 5 , 3 , 8 },
                {4 , 1 , 14, 8 , 13, 6 , 2 , 11, 15, 12, 9 , 7 , 3 , 10, 5 , 0 },
                {15, 12, 8 , 2 , 4 , 9 , 1 , 7 , 5 , 11, 3 , 14, 10, 0 , 6 , 13},
            },
            { // 1
                {15, 1 , 8 , 14, 6 , 11, 3 , 4 , 9 , 7 , 2 , 13, 12, 0 , 5 , 10},
                {3 , 13, 4 , 7 , 15, 2 , 8 , 14, 12, 0 , 1 , 10, 6 , 9 , 11, 5 },
                {0 , 14, 7 , 11, 10, 4 , 13, 1 , 5 , 8 , 12, 6 , 9 , 3 , 2 , 15},
                {13, 8 , 10, 1 , 3 , 15, 4 , 2 , 11, 6 , 7 , 12, 0 , 5 , 14, 9 },
            },
            { // 2
                {10, 0 , 9 , 14, 6 , 3 , 15, 5 , 1 , 13, 12, 7 , 11, 4 , 2 , 8 },
                {13, 7 , 0 , 9 , 3 , 4 , 6 , 10, 2 , 8 , 5 , 14, 12, 11, 15, 1 },
                {13, 6 , 4 , 9 , 8 , 15, 3 , 0 , 11, 1 , 2 , 12, 5 , 10, 14, 7 },
                {1 , 10, 13, 0 , 6 , 9 , 8 , 7 , 4 , 15, 14, 3 , 11, 5 , 2 , 12},
            },
            { // 3
                {7 , 13, 14, 3 , 0 , 6 , 9 , 10, 1 , 2 , 8 , 5 , 11, 12, 4 , 15},
                {13, 8 , 11, 5 , 6 , 15, 0 , 3 , 4 , 7 , 2 , 12, 1 , 10, 14, 9 },
                {10, 6 , 9 , 0 , 12, 11, 7 , 13, 15, 1 , 3 , 14, 5 , 2 , 8 , 4 },
                {3 , 15, 0 , 6 , 10, 1 , 13, 8 , 9 , 4 , 5 , 11, 12, 7 , 2 , 14},
            },
            { // 4
                {2 , 12, 4 , 1 , 7 , 10, 11, 6 , 8 , 5 , 3 , 15, 13, 0 , 14, 9 },
                {14, 11, 2 , 12, 4 , 7 , 13, 1 , 5 , 0 , 15, 10, 3 , 9 , 8 , 6 },
                {4 , 2 , 1 , 11, 10, 13, 7 , 8 , 15, 9 , 12, 5 , 6 , 3 , 0 , 14},
                {11, 8 , 12, 7 , 1 , 14, 2 , 13, 6 , 15, 0 , 9 , 10, 4 , 5 , 3 },
            },
            { // 5
                {12, 1 , 10, 15, 9 , 2 , 6 , 8 , 0 , 13, 3 , 4 , 14, 7 , 5 , 11},
                {10, 15, 4 , 2 , 7 , 12, 9 , 5 , 6 , 1 , 13, 14, 0 , 11, 3 , 8 },
                {9 , 14, 15, 5 , 2 , 8 , 12, 3 , 7 , 0 , 4 , 10, 1 , 13, 11, 6 },
                {4 , 3 , 2 , 12, 9 , 5 , 15, 10, 11, 14, 1 , 7 , 6 , 0 , 8 , 13},
            },
            { // 6
                {4 , 11, 2 , 14, 15, 0 , 8 , 13, 3 , 12, 9 , 7 , 5 , 10, 6 , 1 },
                {13, 0 , 11, 7 , 4 , 9 , 1 , 10, 14, 3 , 5 , 12, 2 , 15, 8 , 6 },
                {1 , 4 , 11, 13, 12, 3 , 7 , 14, 10, 15, 6 , 8 , 0 , 5 , 9 , 2 },
                {6 , 11, 13, 8 , 1 , 4 , 10, 7 , 9 , 5 , 0 , 15, 14, 2 , 3 , 12},
            },
            { // 7
                {13, 2 , 8 , 4 , 6 , 15, 11, 1 , 10, 9 , 3 , 14, 5 , 0 , 12, 7 },
                {1 , 15, 13, 8 , 10, 3 , 7 , 4 , 12, 5 , 6 , 11, 0 , 14, 9 , 2 },
                {7 , 11, 4 , 1 , 9 , 12, 14, 2 , 0 , 6 , 10, 13, 15, 3 , 5 , 8 },
                {2 , 1 , 14, 7 , 4 , 10, 8 , 13, 15, 12, 9 , 0 , 3 , 5 , 6 , 11},
            },
        };

        //static byte[] _k1pTable = new byte[28] {
        //    57, 49, 41, 33, 25, 17, 9 , 1 , 58, 50, 42, 34, 26, 18,
        //    10, 2 , 59, 51, 43, 35, 27, 19, 11, 3 , 60, 52, 44, 36,
        //};

        //static byte[] _k2pTable = new byte[28] {
        //    63, 55, 47, 39, 31, 23, 15, 7 , 62, 54, 46, 38, 30, 22,
        //    14, 6 , 61, 53, 45, 37, 29, 21, 13, 5 , 28, 20, 12, 4 ,
        //};

        static byte[] _kpTable = new byte[56] {
            57, 49, 41, 33, 25, 17,  9,
            1,  58, 50, 42, 34, 26, 18,
            10,  2, 59, 51, 43, 35, 27,
            19, 11,  3, 60, 52, 44, 36,
            63, 55, 47, 39, 31, 23, 15,
             7, 62, 54, 46, 38, 30, 22,
            14,  6, 61, 53, 45, 37, 29,
            21, 13,  5, 28, 20, 12,  4
        };

        static byte[] _cpTable = new byte[48] {
            14, 17, 11, 24,  1,  5,
             3, 28, 15,  6, 21, 10,
            23, 19, 12,  4, 26,  8,
            16,  7, 27, 20, 13,  2,
            41, 52, 31, 37, 47, 55,
            30, 40, 51, 45, 33, 48,
            44, 49, 39, 56, 34, 53,
            46, 42, 50, 36, 29, 32,
        };

        static byte[] _shiftTable = new byte[16]
        {
            1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1
        };

        static ulong InitialPermutation(ulong block64)
        {
            ulong result = 0;
            for (int i = 0; i < 64; ++i)
            {
                int bitIndex = 64 - _ipTable[i];
                ulong bit = (block64 >> bitIndex) & 1;
                result |= bit << (64 - i - 1);
            }

            return result;
        }

        static ulong FinalPermutation(ulong block64)
        {
            ulong result = 0;
            for (int i = 0; i < 64; ++i)
            {
                int bitIndex = 64 - _iipTable[i];
                ulong bit = (block64 >> bitIndex) & 1;
                result |= bit << (64 - i - 1);
            }

            return result;
        }

        static ulong BlockExpansion(uint block32)
        {
            ulong result = 0;
            for (int i = 0; i < 48; ++i)
            {
                int bitIndex = 32 - _eTable[i];
                ulong bit = (block32 >> bitIndex) & 1;
                result |= bit << (48 - i - 1);
            }

            return result;
        }

        static uint SBlock(ulong block48)
        {
            Console.WriteLine($"block: {BinaryRepresentation(block48, 6, 48)}");

            uint result = 0;
            for(int k = 0; k < 8; ++k)
            {
                byte sblock = (byte)(block48 & 0b111111UL);

                Console.WriteLine($"b_{8 - k}: {BinaryRepresentation(sblock, 6, 6)}");

                int i = ((sblock >> 5) << 1) | (sblock & 1); // первый и последний бит шестизначного числа
                int j = (sblock >> 1) & ~(1 << 4); // все биты кроме первого и последнего

                Console.WriteLine($"i: {i}, j: {j}");

                byte newBits = __sTables[7 - k, i, j];

                Console.WriteLine($"new bits: {BinaryRepresentation(newBits, 4, 4)}");

                result |= ((uint)newBits << (4 * k));

                block48 >>= 6;
            }

            Console.WriteLine($"result: {BinaryRepresentation(result, 4, 32)}");

            return result;
        }

        static uint PPermutation(uint block32)
        {
            uint result = 0;
            for (int i = 0; i < 32; ++i)
            {
                int bitIndex = 32 - _pTable[i];
                uint bit = (block32 >> bitIndex) & 1;
                result |= bit << (32 - i - 1);
            }

            return result;
        }

        static uint F(uint block32, ulong key48)
        {
            ulong block48 = BlockExpansion(block32);
            Console.WriteLine($"expanded block: {BinaryRepresentation(block48, 6, 48)}");

            ulong keyApplied = block48 ^ key48;

            Console.WriteLine($"key: {BinaryRepresentation(key48, 6, 48)}");

            Console.WriteLine($"key applied: {BinaryRepresentation(keyApplied, 6, 48)}");

            uint sBlocksApplied = SBlock(keyApplied);

            Console.WriteLine($"s-block applied: {BinaryRepresentation(sBlocksApplied, 4, 32)}");

            uint pPermutationApplied = PPermutation(sBlocksApplied);

            Console.WriteLine($"p-permutation applied: {BinaryRepresentation(pPermutationApplied, 4, 32)}");

            return pPermutationApplied;
        }

        static ulong Round(ulong block64, ulong key48)
        {
            uint l = (uint)(block64 >> 32);
            uint r = (uint)(block64 & 0xFFFFFFFF);

            Console.WriteLine($"l: {BinaryRepresentation(l, 4, 32)}");
            Console.WriteLine($"r: {BinaryRepresentation(r, 4, 32)}");

            return ((ulong)r << 32) | (l ^ F(r, key48));
        }

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

        static ulong KeyExpansion(ulong key56)
        {
            ulong result = 0;
            for (int i = 0; i < 8; ++i)
            {
                byte part = (byte)(key56 & 0b1111111);
                uint bitToAdd = Parity(part) == 0 ? 1U : 0U;
                part = (byte)(part | (bitToAdd << 7));

                result |= ((ulong)part << 8 * i);

                key56 >>= 7;
            }

            return result;
        }

        static uint KeyPart(ulong key64, byte[] permutationTable)
        {
            uint result = 0;
            for (int i = 0; i < 28; ++i)
            {
                result |= (uint)(((key64 >> (permutationTable[i] - 1)) & 1) << 28 - i - 1);
            }

            return result;
        }

        static ulong KeyPermutation(ulong key56)
        {
            ulong result = 0;
            for (int i = 0; i < 48; ++i)
            {
                int bitIndex = 56 - _cpTable[i];
                ulong bit = (key56 >> bitIndex) & 1;
                result |= bit << (48 - i - 1);
            }

            return result;
        }

        static ulong KeyFirstPermutation(ulong key64)
        {
            ulong result = 0;
            for (int i = 0; i < 56; ++i)
            {
                int bitIndex = 64 - _kpTable[i];
                ulong bit = (key64 >> bitIndex) & 1;
                result |= bit << (56 - i - 1);
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
            0b1111111111111111111111111111111,
        };

        public static uint RotateLeft(uint num, int bitLength, int count)
        {
            uint mask = _masks[bitLength];
            return (num << count) & mask | (num >> (bitLength - count));
        }

        static ulong[] GenerateKeys(ulong key64)
        {
            ulong[] result = new ulong[16];

            //ulong key64 = KeyExpansion(key56);

            Console.WriteLine($"key64: {BinaryRepresentation(key64)}");

            ulong key56 = KeyFirstPermutation(key64);

            Console.WriteLine($"key56: {BinaryRepresentation(key56, 7, 56)}");

            uint c = (uint)(key56 >> 28);
            uint d = (uint)(key56 & 0xFFFFFFF);

            Console.WriteLine($"c_0: {BinaryRepresentation(c, 7, 28)}");
            Console.WriteLine($"d_0: {BinaryRepresentation(d, 7, 28)}");

            for (int i = 0; i < 16; ++i)
            {
                c = RotateLeft(c, 28, _shiftTable[i]);
                d = RotateLeft(d, 28, _shiftTable[i]);

                Console.WriteLine($"c_{i + 1}: {BinaryRepresentation(c, 7, 28)}");
                Console.WriteLine($"d_{i + 1}: {BinaryRepresentation(d, 7, 28)}");

                result[i] = KeyPermutation(((ulong)c << 28) | d);
            }

            int k = 1;
            foreach(var key in result)
            {
                Console.WriteLine($"key_{k++}: {BinaryRepresentation(key, 6, 48)}");
            }

            return result;
        }

        static ulong SwapParts(ulong block64)
        {
            return (block64 << 32) | (block64 >> 32);
        }

        static ulong Encode(ulong block64, ulong[] roundKeys)
        {
            Console.WriteLine($"before ip block64: {BinaryRepresentation(block64, 4, 64)}");

            block64 = InitialPermutation(block64);

            Console.WriteLine($"after ip block64: {BinaryRepresentation(block64, 4, 64)}");

            for (int i = 0; i < 16; ++i)
            {
                Console.WriteLine($"iteration: {i}");
                block64 = Round(block64, roundKeys[i]);
            }

            block64 = SwapParts(block64);

            return FinalPermutation(block64);
        }

        private static (ulong key, ulong message, ulong cipher)[] _testCases = new (ulong key, ulong message, ulong cipher)[]
        {
            (0x0E329232EA6D0D73, 0x8787878787878787, 0x0000000000000000),
            (0x133457799BBCDFF1, 0x0123456789ABCDEF, 0x85E813540F0AB405),
            (0x38627974656B6579, 0x6D6573736167652E, 0x7CF45E129445D451)
        };

        private static void TestDES()
        {
            bool success = true;
            foreach(var (key, block, excpected) in _testCases)
            {
                ulong[] roundKeys = GenerateKeys(key);
                ulong cipher = Encode(block, roundKeys);
                if(cipher != excpected)
                {
                    success = false;
                    Console.WriteLine($"FAIL: key: {key}, block: {block}, cipher: {cipher}, excpected: {excpected}");
                    break;
                }
            }

            if (success)
            {
                Console.WriteLine("SUCCESS!");
            }
            else
            {
                Console.WriteLine("FAIL!");
            }
        }

        static string BinaryRepresentation(ulong num)
        {
            return BinaryRepresentation(num, 8, 64);
        }

        static string BinaryRepresentation(ulong num, int n)
        {
            return BinaryRepresentation(num, 8, n);
        }

        static string BinaryRepresentation(ulong num, int s, int n)
        {
            var binRepr = Convert.ToString((long)num, 2).PadLeft(n, '0');
            return Regex.Replace(binRepr, $".{{{s}}}", "$0 ");
        }

        static string BinaryRepresentation(uint num)
        {
            return BinaryRepresentation(num, 8, 32);
        }

        static string BinaryRepresentation(uint num, int n)
        {
            return BinaryRepresentation(num, 8, n);
        }

        static string BinaryRepresentation(uint num, int s, int n)
        {
            var binRepr = Convert.ToString(num, 2).PadLeft(n, '0');
            return Regex.Replace(binRepr, $".{{{s}}}", "$0 ");
        }

        static void Main(string[] args)
        {
            //Console.WriteLine(BinaryRepresentation(0xFFFFFFFF));

            //for (int i = 0; i < 32; ++i)
            //{
            //    Console.WriteLine("0b" + new string('1', i) + ",");
            //}

            TestDES();

            //ulong key = 0x133457799BBCDFF1;
            //ulong block = 0x0123456789ABCDEF;

            //ulong[] roundKeys = null;
            //GenerateKeys(key, ref roundKeys);

            //ulong cipher = Encode(block, roundKeys);

            //Array.Reverse(roundKeys);
            //ulong decodedBlock = Encode(cipher, roundKeys);

            //Console.WriteLine($"key: {key} (hex: {Convert.ToString((long)key, 16)})");
            //Console.WriteLine($"block: {block} (hex: {Convert.ToString((long)block, 16)})");
            //Console.WriteLine($"cipher: {cipher} (hex: {Convert.ToString((long)cipher, 16)})");
            //Console.WriteLine($"decodedBlock: {decodedBlock} (hex: {Convert.ToString((long)decodedBlock, 16)})");
        }
    }
}
