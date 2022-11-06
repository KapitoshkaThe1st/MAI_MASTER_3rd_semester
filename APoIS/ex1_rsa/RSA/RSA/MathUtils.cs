﻿using System;
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

        public static bool ModularMultiplicativeInverse(BigInteger num, BigInteger modulus, out BigInteger result)
        {
            if (ExtendedGreatestCommonDivisor(num, modulus, out BigInteger a, out BigInteger b) == 1)
            {
                //result = ((num * a) % modulus == BigInteger.One) ? a : b;
                BigInteger toCheck = (num * a) % modulus;
                if (toCheck == BigInteger.One || toCheck == (BigInteger.One - modulus))
                {
                    result = a;
                }
                else
                {
                    result = b;
                }

                if(result < 0)
                {
                    result = (result % modulus + modulus) % modulus;

                }

                return true;
            }

            result = BigInteger.Zero;
            return false;
        }

        public static int SolveQuadraticEquation(BigInteger a, BigInteger b, BigInteger c, out BigInteger x1, out BigInteger x2)
        {
            var discriminant = b * b - 4 * a * c;

            //Console.WriteLine($"discriminant: {discriminant}");

            if (discriminant > 0)
            {
                x1 = (-b + BigIntegerExtensions.Sqrt(discriminant)) / (2 * a);
                x2 = (-b - BigIntegerExtensions.Sqrt(discriminant)) / (2 * a);
                return 2;
            }

            else if (discriminant == 0)
            {
                x1 = -b / (2 * a);
                x2 = 0;
                return 1;
            }
            else
            {
                x1 = 0;
                x2 = 0;
                return 0;
            }
        }
    }
}
