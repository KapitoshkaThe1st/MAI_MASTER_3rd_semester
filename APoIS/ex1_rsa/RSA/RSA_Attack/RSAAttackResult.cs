using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RSA_Attack
{
    public class RSAAttackResult
    {
        public bool Success { get; init; }
        public BigInteger Modulus { get; init; }
        public BigInteger Phi { get; init; }
        public BigInteger PublicExponent { get; init; }
        public BigInteger PrivateExponent { get; init; }
        public BigInteger P { get; init; }
        public BigInteger Q { get; init; }
    }
}
