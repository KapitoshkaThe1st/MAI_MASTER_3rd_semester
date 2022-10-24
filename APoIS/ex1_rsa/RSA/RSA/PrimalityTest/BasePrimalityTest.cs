using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RSA.PrimalityTest
{
    public abstract class BasePrimalityTest : IPrimalityTest
    {
        public readonly float probability;
        public BasePrimalityTest(float probability)
        {
            this.probability = probability;
        }

        //protected static Random _rnd = new Random(0);
        protected static Random _rnd = new Random();
        protected static readonly BigInteger _bigIntegerTwo = new BigInteger(2);
        protected static readonly BigInteger _bigIntegerThree = new BigInteger(3);

        protected void ThrowIfNegative(BigInteger num)
        {
            if (num <= BigInteger.Zero)
            {
                throw new ArgumentException("num must be positive integer");
            }
        }

        protected bool TestTrivialCases(BigInteger num, out bool testResult)
        {
            if (num == _bigIntegerTwo || num == _bigIntegerThree)
            {
                testResult = true;
                return true;
            }

            if (num.IsEven || num == BigInteger.One)
            {
                testResult = false;
                return true;
            }

            testResult = false;
            return false;
        }

        public abstract bool Test(BigInteger num);
        
        protected abstract int IterationsNumber();
    }
}
