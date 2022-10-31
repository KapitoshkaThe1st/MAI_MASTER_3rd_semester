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

            while (!_primalityTest.Test(number))
            {
                number += 2;
                if (number > _maxPQ)
                {
                    number = GenInteger(larger);
                }
            }

            return number;
        }

        public BigInteger P { get; private set; }
        public BigInteger Q { get; private set; }

        public void Generate(out BigInteger modulus, out BigInteger publicExponent, out BigInteger privateExponent)
        {
            P = GeneratePrimeNumber(true);

            while (true)
            {
                Q = GeneratePrimeNumber(false);

                modulus = P * Q;

                BigInteger phiN = (P - 1) * (Q - 1);

                publicExponent = defaultPublicExponent;

                if (MathUtils.ModularMultiplicativeInverse(publicExponent, phiN, out privateExponent) && BigInteger.Pow(3 * privateExponent, 4) >= modulus)
                {
                    break;
                }
            }
        }

        private BigInteger GeneratePrimeNumberBetween(BigInteger a, BigInteger b)
        {
            BigInteger p = _rnd.NextBigInteger(a, b);

            if (p.IsEven)
            {
                p += 1;
            }

            while (true)
            {
                if (p >= b)
                {
                    p = _rnd.NextBigInteger(a, b);

                    if (p.IsEven)
                    {
                        p += 1;
                    }
                }

                if (_primalityTest.Test(p))
                {
                    break;
                }
                p += 2;
            }

            return p;
        }

        public void GenerateVulnerable(out BigInteger modulus, out BigInteger publicExponent, out BigInteger privateExponent)
        {
            Q = GeneratePrimeNumber(false);

            while (true)
            {
                P = GeneratePrimeNumberBetween(Q + 1, 2 * Q);

                if (P <= Q || P >= 2 * Q)
                {
                    Console.WriteLine("Q >= P || P <= 2 * Q");
                    continue;
                }

                modulus = P * Q;

                BigInteger phiQ = Q - 1;
                BigInteger phiP = P - 1;

                BigInteger phiN = phiP * phiQ;

                privateExponent = GeneratePrimeNumberBetween(0, BigIntegerExtensions.Sqrt(BigIntegerExtensions.Sqrt(modulus)) / 3);

                if (MathUtils.ModularMultiplicativeInverse(privateExponent, phiN, out publicExponent))
                {
                    break;
                }
            }
        }
    }
}
