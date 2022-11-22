using System;

namespace DES
{
    class Program
    {
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
                DESEncoder encoder = new DESEncoder(key);

                ulong cipher = encoder.Encode(block);
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

        //https://page.math.tu-berlin.de/~kant/teaching/hess/krypto-ws2006/des.htm

        static void Main(string[] args)
        {
            //Console.WriteLine(BinaryFormatting.Format(0xFFFFFFFF));

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
