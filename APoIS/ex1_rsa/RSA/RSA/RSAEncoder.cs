using System;
using System.Numerics;

namespace RSA
{
    internal class RSAHelper
    {
        private BigInteger _n;
        private BigInteger _exponent;

        public RSAHelper(BigInteger n, BigInteger publicExponent)
        {
            _n = n;
            _exponent = publicExponent;
        }

        protected BigInteger Transform(BigInteger m)
        {
            return BigInteger.ModPow(m, _exponent, _n);
        }
    }

    class RSAEncoder : RSAHelper
    {
        public RSAEncoder(BigInteger n, BigInteger publicExponent) : base(n, publicExponent) { }

        public BigInteger Encode(BigInteger m)
        {
            return Transform(m);
        }
    }

    class RSADecoder : RSAHelper
    {

        public RSADecoder(BigInteger n, BigInteger privateExponent) : base(n, privateExponent) { }

        public BigInteger Decode(BigInteger m)
        {
            return Transform(m);
        }
    }
}
