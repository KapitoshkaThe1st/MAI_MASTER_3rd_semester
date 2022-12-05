using System;

using GF_Operations;

namespace SBoxComputation
{
    class Program
    {
        private static uint rijndaelModulo = 0x11b;

        static void GenerateGFInverseTable()
        {
            byte[] inverse = new byte[256];
            for (int i = 0; i < 256; ++i)
            {
                inverse[i] = (byte)(i != 0 ? GF256.Inverse((uint)i, rijndaelModulo) : 0);
            }

            Console.WriteLine("uint[] _gfInverse = new uint[] {");
            for (int i = 0; i < 16; ++i)
            {
                Console.Write("\t");
                for (int j = 0; j < 16; ++j)
                {
                    Console.Write($"0x{Convert.ToString(inverse[i * 16 + j], 16).PadLeft(2, '0').ToUpper()}");
                    if (i != 15 || j != 15)
                    {
                        Console.Write(", ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("};");
        }

        static byte GetBit(byte b, int i)
        {
            return (byte)((b >> i) & 1);
        }

        public static byte RotateLeft(byte num, int count)
        {
            return (byte)((num << count) | (num >> (8 - count)));
        }

        public static byte RotateRight(byte num, int count)
        {
            return (byte)((num << (8 - count)) | (num >> count));
        }

        public static byte Parity(byte num)
        {
            // https://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetKernighan
            byte result = 0;

            // каждая итерация обнуляет младший установленный бит
            while (num != 0)
            {
                num &= (byte)(num - 1);
                result ^= 1;
            }

            return result;
        }

        static void GenerateRCon()
        {
            const int nConstants = 26;
            uint[] rCon = new uint[nConstants];

            rCon[1] = 1;

            for(int i = 2; i < nConstants; ++i)
            {
                rCon[i] = GF256.Multiply(rCon[i - 1], 0b10, rijndaelModulo);
            }

            Console.WriteLine("uint[] _rCon = new uint[] {");
            for (int i = 0; i < nConstants; ++i)
            {
                Console.Write($"0x{Convert.ToString(rCon[i], 16).PadLeft(4, '0').ToUpper()}");
                if (i != nConstants - 1)
                {
                    Console.Write(", ");
                }
            }
            Console.WriteLine();
            Console.WriteLine("};");
        }

        static void GenerateSBox()
        {
            byte[] sBox = new byte[256];
            byte[] inverseSBox = new byte[256];

            byte constant = 0x63;

            for (int k = 0; k < 256; ++k)
            {
                byte inv = (byte)(k != 0 ? GF256.Inverse((uint)k, rijndaelModulo) : 0);

                byte b = 0;
                byte mask = 0b10001111;
                for(int i = 0; i < 8; ++i)
                {
                    //byte bit = Parity((byte)((inverse & RotateRight(mask, i)) ^ constant));
                    byte bit = (byte)(GetBit(inv, i) ^ GetBit(inv, (i + 4) % 8) ^ GetBit(inv, (i + 5) % 8) ^ GetBit(inv, (i + 6) % 8) ^ GetBit(inv, (i + 7) % 8) ^ GetBit(constant, i));
                    b |= (byte)(bit << i);
                }

                sBox[k] = b;
                inverseSBox[b] = (byte)k;
            }

            Console.WriteLine("byte[] _sBox = new byte[] {");
            for (int i = 0; i < 16; ++i)
            {
                Console.Write("\t");
                for (int j = 0; j < 16; ++j)
                {
                    Console.Write($"0x{Convert.ToString(sBox[i * 16 + j], 16).PadLeft(2, '0').ToUpper()}");
                    if (i != 15 || j != 15)
                    {
                        Console.Write(", ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("};");

            Console.WriteLine("byte[] _inverseSBox = new byte[] {");
            for (int i = 0; i < 16; ++i)
            {
                Console.Write("\t");
                for (int j = 0; j < 16; ++j)
                {
                    Console.Write($"0x{Convert.ToString(inverseSBox[i * 16 + j], 16).PadLeft(2, '0').ToUpper()}");
                    if (i != 15 || j != 15)
                    {
                        Console.Write(", ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("};");
        }

        static void Main(string[] args)
        {
            GenerateSBox();

            GenerateRCon();
        }
    }
}
