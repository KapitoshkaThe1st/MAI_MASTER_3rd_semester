using System;
using System.Numerics;

namespace RSA.PrimalityTest
{
    public class FermatTest : BasePrimalityTest
    {
        public FermatTest(float probability) : base(probability) { }

        public override bool Test(BigInteger num)
        {
            ThrowIfNegative(num);

            if(TestTrivialCases(num, out bool trivialCasesTestResult))
            {
                return trivialCasesTestResult;
            }

            int nIterations = IterationsNumber();
            var numMinusOne = num - 1;

            for (int i = 0; i < nIterations; ++i)
            {
                BigInteger a = _rnd.NextBigInteger(2, num);

                if (BigInteger.GreatestCommonDivisor(a, num) > 1)
                {
                    return false;
                }

                if (BigInteger.ModPow(a, numMinusOne, num) != BigInteger.One)
                {
                    return false;
                }
            }
            return true;
        }

        protected override int IterationsNumber()
        {
            return (int)Math.Ceiling(Math.Log(1.0 / (1.0 - probability), 2.0));
        }
    }
}
