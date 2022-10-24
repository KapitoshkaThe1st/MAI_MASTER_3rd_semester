using System;
using System.Numerics;

namespace RSA.PrimalityTest
{
    public class MillerRabinTest : BasePrimalityTest
    {
        public MillerRabinTest(float probability) : base(probability) { }

        public override bool Test(BigInteger num)
        {
            ThrowIfNegative(num);

            if (TestTrivialCases(num, out bool trivialCasesTestResult))
            {
                return trivialCasesTestResult;
            }

            var numMinusOne = num - 1;

            int s = 0;
            BigInteger d = numMinusOne;
            while (true)
            {
                if (!d.IsEven)
                {
                    break;
                }
                d /= 2;
                s++;
            }

            int nIterations = IterationsNumber();
            for(int i = 0; i < nIterations; ++i)
            {
                var a = _rnd.NextBigInteger(_bigIntegerTwo, numMinusOne);

                if (BigInteger.GreatestCommonDivisor(a, num) > 1)
                {
                    return false;
                }

                BigInteger modpow = BigInteger.ModPow(a, d, num);

                // здесь проверяются первое условие и первая итерация второго условия: a^(2 ^ 0) d == -1 (mod num)
                if (modpow == BigInteger.One || modpow == numMinusOne)
                    continue;

                // здесь проверяются все оставшиеся итерации второго условия
                bool secondConditionSatisfied = false;
                for(int j = 0; j < s; ++j)
                {
                    modpow = BigInteger.ModPow(modpow, _bigIntegerTwo, num);
                    if(modpow == numMinusOne)
                    {
                        secondConditionSatisfied = true;
                        break;
                    }
                }

                if (!secondConditionSatisfied)
                {
                    return false;
                }

                nIterations--;
            }

            return true;
        }

        protected override int IterationsNumber()
        {
            return (int)Math.Ceiling(Math.Log(1.0 / (1.0 - probability), 4.0));
        }
    }
}
