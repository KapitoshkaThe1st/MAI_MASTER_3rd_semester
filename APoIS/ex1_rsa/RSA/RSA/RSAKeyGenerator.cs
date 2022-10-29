using System;
using System.Linq;
using System.Numerics;

using RSA.PrimalityTest;

namespace RSA
{
    public class RSAKeyGenerator
    {
        private IPrimalityTest _primalityTest;
        private readonly int _byteLength;
        private readonly BigInteger _maxPQ;

        //private static Random _rnd = new Random(0);
        private static Random _rnd = new Random();

        private static int PQRepresentationByteLength(RSAKeyType keyType)
        {
            return (int)keyType / 8 + 1;
        }

        private static BigInteger MaxPQOfByteLength(int byteLength)
        {
            byte[] bytes = Enumerable.Repeat((byte)0xFF, byteLength).ToArray();
            bytes[byteLength - 1] = 0;

            return new BigInteger(bytes);
        }

        public RSAKeyGenerator(IPrimalityTest primalityTest, RSAKeyType keyType = RSAKeyType.RSA2048)
        {
            _byteLength = PQRepresentationByteLength(keyType);
            _maxPQ = MaxPQOfByteLength(_byteLength);
            _primalityTest = primalityTest;
        }

        private BigInteger GenInteger(bool larger)
        {
            var bytes = new byte[_byteLength];

            _rnd.NextBytes(bytes);

            bytes[0] |= 1;  // выставляем бит четности в 1
            bytes[_byteLength - 1] = 0; // выставляем весь байт в 0, чтобы знаковый бит был 0, чтобы число считалось положительным
            bytes[_byteLength - 2] |= (1 << 7); // выставляем старший бит в 1
            if (larger)
            {
                bytes[_byteLength - 2] |= 0b0100_0000;
            }
            else
            {
                bytes[_byteLength - 2] &= 0b1011_1111;
            }

            return new BigInteger(bytes);
        }

        private static readonly BigInteger defaultPublicExponent = new BigInteger(65537);

        private BigInteger GeneratePrimeNumber(bool larger)
        {
            BigInteger number = GenInteger(larger);

            //Console.WriteLine($"number: {number}");

            while (!_primalityTest.Test(number))
            {
                number += 2;
                if (number > _maxPQ)
                {
                    number = GenInteger(larger);
                }
                //Console.WriteLine($"number: {number}");
            }

            return number;
        }

        public void Generate(out BigInteger modulus, out BigInteger publicExponent, out BigInteger privateExponent)
        {
            BigInteger p = GeneratePrimeNumber(true);
            Console.WriteLine($"p: {p}");

            while (true)
            {
                BigInteger q = GeneratePrimeNumber(false);
                Console.WriteLine($"q: {q}");

                modulus = p * q;

                //Console.WriteLine($"n: {n}");

                BigInteger phiQ = q - 1;
                BigInteger phiP = p - 1;

                //Console.WriteLine($"phiQ: {phiQ}");
                //Console.WriteLine($"phiP: {phiP}");

                BigInteger phiN = phiP * phiQ;

                //Console.WriteLine($"phiN: {phiN}");

                BigInteger treshold = BigIntegerExtensions.Sqrt(BigIntegerExtensions.Sqrt(modulus)) / 3 + 1;

                //Console.WriteLine($"treshold: {treshhold}");

                publicExponent = defaultPublicExponent;
                //Console.WriteLine($"publicExponent: {publicExponent}");

                if (MathUtils.ExtendedGreatestCommonDivisor(publicExponent, phiN, out BigInteger a, out BigInteger b) == 1)
                {
                    privateExponent = ((publicExponent * a) % phiN == BigInteger.One) ? a : b;

                    //Console.WriteLine($"privateExponent: {privateExponent}");

                    if (privateExponent >= treshold)
                    {
                        break;
                    }
                }
            }
        }
    }
}
