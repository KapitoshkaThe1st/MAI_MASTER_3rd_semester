using System;
using System.Numerics;

using RSA;

namespace RSA_Attack
{
    public class ModulusFactorizationAttack : BaseRSAAttack
    {
        public ModulusFactorizationAttack() : base() { }

        public ModulusFactorizationAttack(Action<float> updateCallback, Action<bool> endCallBack) : base(updateCallback, endCallBack) { }

        public override RSAAttackResult Attack(BigInteger modulus, BigInteger publicExponent)
        {
            BigInteger p = 3;
            BigInteger q = BigInteger.Zero;

            // step for progress update
            float step = 200.0f / (float)BigIntegerExtensions.Sqrt(modulus);

            for (; p * p <= modulus; p += 2)
            {
                q = BigInteger.DivRem(modulus, p, out var rem);
                if (rem == BigInteger.Zero)
                {
                    break;
                }

                _updateCallback?.Invoke(step);
            }
            
            var phi = (p - 1) * (q - 1);

            MathUtils.ModularMultiplicativeInverse(publicExponent, phi, out BigInteger privateExponent);

            _endCallback?.Invoke(true);

            return new RSAAttackResult
            {
                Success = true,
                Modulus = modulus,
                Phi = phi,
                PublicExponent = publicExponent,
                PrivateExponent = privateExponent,
                P = p,
                Q = q,
            };
        }
    }
}
