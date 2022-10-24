using System;
using System.Numerics;

namespace RSA
{
    public class MathUtils
    {
        public static int JacobiSymbol(BigInteger a, BigInteger n)
        {
            if (n <= 0 || n.IsEven)
            {
                throw new ArgumentException("n must be greater than zero and odd");
            }

            a = a % n;
            int t = 1;
            BigInteger r;
            while (a != 0)
            {
                while (a % 2 == 0)
                {
                    a = a >> 1;
                    r = n % 8;
                    if (r == 3 || r == 5)
                    {
                        t = -t;
                    }
                }
                r = n;
                n = a;
                a = r;
                if (a % 4 == 3 && n % 4 == 3)
                {
                    t = -t;
                }
                a = a % n;
            }
            if (n == 1)
            {
                return t;
            }
            else
            {
                return 0;
            }
        }

        public static BigInteger ExtendedGreatestCommonDivisor(BigInteger n, BigInteger m, out BigInteger a, out BigInteger b)
        {
            if(n < BigInteger.Zero)
            {
                throw new ArgumentException("n must be non-negative");
            }

            if (m < BigInteger.Zero)
            {
                throw new ArgumentException("m must be non-negative");
            }

            if (n < m)
            {
                return ExtendedGreatestCommonDivisor(m, n, out b, out a);
            }

            if(m == BigInteger.Zero)
            {
                a = 1;
                b = 0;
                return n;
            }

            BigInteger r0 = n;
            BigInteger r1 = m;

            BigInteger s0 = 1;
            BigInteger s1 = 0;

            BigInteger t0 = 0;
            BigInteger t1 = 1;

            int iteration = 0;
            while (r0 != BigInteger.Zero && r1 != BigInteger.Zero)
            {
                if(iteration % 2 == 0)
                {
                    var q = BigInteger.DivRem(r0, r1, out r0);
                    s0 = s0 - q * s1;
                    t0 = t0 - q * t1;
                }
                else
                {
                    var q = BigInteger.DivRem(r1, r0, out r1);
                    s1 = s1 - q * s0;
                    t1 = t1 - q * t0;
                }
                iteration++;
            }

            if(iteration % 2 == 0)
            {
                a = s0;
                b = t0;

                return r0;
            }

            a = s1;
            b = t1;

            return r1;
        }
    }
}
