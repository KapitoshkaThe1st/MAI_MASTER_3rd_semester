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

                BigInteger phiQ = Q - 1;
                BigInteger phiP = P - 1;

                BigInteger phiN = phiP * phiQ;

                BigInteger treshold = BigIntegerExtensions.Sqrt(BigIntegerExtensions.Sqrt(modulus)) / 3 + 1;

                publicExponent = defaultPublicExponent;

                if(MathUtils.ModularMultiplicativeInverse(publicExponent, phiN, out privateExponent) && privateExponent >= treshold)
                {
                    break;
                }
            }
        }
    }
}
