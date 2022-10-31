using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using RSA;

namespace RSA_Attack
{
    // https://sagi.io/2016/04/crypto-classics-wieners-rsa-attack/
    public class WienerAttack : BaseRSAAttack
    {
        public WienerAttack() : base() { }

        public WienerAttack(Action<float> updateCallback, Action<bool> endCallBack) : base(updateCallback, endCallBack) { }

        private static IEnumerable<BigInteger> ToContinuedFraction(BigInteger num, BigInteger denom)
        {
            while (num != 1)
            {
                yield return BigInteger.DivRem(num, denom, out BigInteger r);

                num = denom;
                denom = r;
            }
        }

        private static IEnumerable<(BigInteger p, BigInteger q)> Convergents(BigInteger num, BigInteger denom)
        {
            var enumerator = ToContinuedFraction(num, denom).GetEnumerator();

            BigInteger prevP = BigInteger.One;
            BigInteger prevQ = BigInteger.Zero;

            enumerator.MoveNext();

            BigInteger P = enumerator.Current;
            BigInteger Q = BigInteger.One;

            yield return (P, Q);

            while (enumerator.MoveNext())
            {
                var newP = enumerator.Current * P + prevP;
                var newQ = enumerator.Current * Q + prevQ;

                prevP = P;
                prevQ = Q;

                P = newP;
                Q = newQ;

                yield return (P, Q);
            }
        }

        public override RSAAttackResult Attack(BigInteger modulus, BigInteger publicExponent)
        {
            // если бы не потребность в вычислении прогресса, то можно было бы не приводить
            // к листу и не производить таким образом вычисление ненужных подходящих дробей
            var convergents = Convergents(publicExponent, modulus).ToList();

            float step = 100f / convergents.Count;

            foreach (var (k, d) in convergents)
            {
                if (d < 1 || k <= 0)
                {
                    _updateCallback?.Invoke(step);
                    continue;
                }

                var phi = (publicExponent * d - 1) / k;

                int nRoots = MathUtils.SolveQuadraticEquation(BigInteger.One, phi - modulus - 1, modulus, out var p, out var q);

                if (nRoots == 2 && p * q == modulus)
                {
                    _endCallback?.Invoke(true);

                    return new RSAAttackResult
                    {
                        Success = true,
                        Modulus = modulus,
                        Phi = phi,
                        PublicExponent = publicExponent,
                        PrivateExponent = d,
                        P = p,
                        Q = q
                    };
                }

                _updateCallback?.Invoke(step);
            }

            _endCallback?.Invoke(false);

            return new RSAAttackResult { Success = false, Modulus = modulus, PublicExponent = publicExponent };
        }
    }
}
