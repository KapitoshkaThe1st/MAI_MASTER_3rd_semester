using System;
using System.Numerics;
using System.Collections.Generic;
using System.Diagnostics;

using RSA.PrimalityTest;

namespace RSA
{
    class Program
    {
        private static void FermatFails()
        {
            //var number = new BigInteger(561);
            var number = new BigInteger(41041);
            //var number = new BigInteger(825265);

            //var number = BigInteger.Parse("825266");

            //var number = new BigInteger(65537);
            //var number = new BigInteger(65536);

            int totalCount = 0;
            int failedCount = 0;

            List<BigInteger> passedA = new List<BigInteger>();

            for (BigInteger a = 2; a < number; ++a)
            {
                totalCount++;
                var numMinusOne = number - 1;

                if (BigInteger.GreatestCommonDivisor(a, number) > 1)
                {
                    Console.WriteLine($"GCD a = {a}: GCD({a}, {number}) = {BigInteger.GreatestCommonDivisor(a, number)}");
                    failedCount++;
                    continue;
                }

                if (BigInteger.ModPow(a, numMinusOne, number) != BigInteger.One)
                {
                    Console.WriteLine($"MODPOW a = {a}: {a}^{numMinusOne} % {number} = {BigInteger.ModPow(a, numMinusOne, number)}");
                    failedCount++;
                    continue;
                }
                passedA.Add(a);
            }

            Console.WriteLine("passed a:");
            passedA.ForEach((a) => Console.WriteLine(a));

            Console.WriteLine($"failed fraction: {(float)failedCount / totalCount}");
        }

        static void RSATest()
        {
            //RSAKeyGenerator keyGen = new RSAKeyGenerator(new MillerRabinTest(0.995f));
            RSAKeyGenerator keyGen = new RSAKeyGenerator(new SolovayStrassenTest(0.995f));

            Stopwatch sw = new Stopwatch();

            sw.Start();

            keyGen.Generate(out BigInteger n, out BigInteger e, out BigInteger d);

            sw.Stop();

            long milliseconds = sw.ElapsedMilliseconds;
            TimeSpan elapsed = sw.Elapsed;

            Console.WriteLine($"key generation time: {milliseconds} ms ({elapsed})");

            RSAEncoder encoder = new RSAEncoder(n, e);
            RSADecoder decoder = new RSADecoder(n, d);

            //Random rnd = new Random(0);
            Random rnd = new Random();

            BigInteger message = rnd.NextBigInteger(1, 1000000000000);

            Console.WriteLine($"message: {message}");

            BigInteger chipher = encoder.Encode(message);
            BigInteger restoredMessage = decoder.Decode(chipher);

            Console.WriteLine($"chipher: {chipher}");
            Console.WriteLine($"restoredMessage: {restoredMessage}");
        }

        static void BenchRSAKeyGeneration()
        {
            //RSAKeyGenerator keyGen = new RSAKeyGenerator(new SolovayStrassenTest(0.995f), RSAKeyType.Bit4096);
            RSAKeyGenerator keyGen = new RSAKeyGenerator(new SolovayStrassenTest(0.995f));
            //RSAKeyGenerator keyGen = new RSAKeyGenerator(new MillerRabinTest(0.995f));

            Stopwatch sw = new Stopwatch();

            const int nTimes = 10;

            long totalMilliseconds = 0;

            for(int i = 0; i < nTimes; ++i)
            {
                sw.Restart();

                Console.WriteLine($"key generation {i}:");
                keyGen.Generate(out BigInteger n, out BigInteger e, out BigInteger d);

                sw.Stop();

                Console.WriteLine($"key generation time: {sw.ElapsedMilliseconds} ms");

                totalMilliseconds += sw.ElapsedMilliseconds;
            }

            double averageMilliseconds = (double)totalMilliseconds / nTimes;

            Console.WriteLine($"average key generation time: {averageMilliseconds} ms");
        }

        static void Main(string[] args)
        {
            //RSATest();
            BenchRSAKeyGeneration();

            Console.ReadKey();
        }
    }
}
