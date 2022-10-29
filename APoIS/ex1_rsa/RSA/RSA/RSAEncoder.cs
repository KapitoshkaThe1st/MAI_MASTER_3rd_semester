using System;
using System.Numerics;

namespace RSA
{
    public abstract class RSAHelper
    {
        private BigInteger _n;
        private BigInteger _exponent;

        public RSAHelper(BigInteger modulus, BigInteger publicExponent)
        {
            _n = modulus;
            _exponent = publicExponent;
        }

        protected BigInteger Transform(BigInteger m)
        {
            return BigInteger.ModPow(m, _exponent, _n);
        }
    }

    public class RSAEncoder : RSAHelper
    {
        public RSAEncoder(BigInteger modulus, BigInteger publicExponent) : base(modulus, publicExponent) { }

        public BigInteger Encode(BigInteger m)
        {
            return Transform(m);
        }
    }

    public class RSADecoder : RSAHelper
    {

        public RSADecoder(BigInteger modulus, BigInteger privateExponent) : base(modulus, privateExponent) { }

        public BigInteger Decode(BigInteger m)
        {
            return Transform(m);
        }
    }
}
