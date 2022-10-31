using System;
using System.Numerics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RSA;

namespace Tests
{
    [TestClass]
    public class MathUtilsTest
    {

        #region JacobiSymbol test
        #region Test data
        private static int[,] _jacobiSymbolValues = new int[,]
        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            { 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0},
            { 1, -1, -1, 1, 0, 1, -1, -1, 1, 0, 1, -1, -1, 1, 0, 1, -1, -1, 1, 0, 1, -1, -1, 1, 0, 1, -1, -1, 1, 0},
            { 1, 1, -1, 1, -1, -1, 0, 1, 1, -1, 1, -1, -1, 0, 1, 1, -1, 1, -1, -1, 0, 1, 1, -1, 1, -1, -1, 0, 1, 1},
            { 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0},
            { 1, -1, 1, 1, 1, -1, -1, -1, 1, -1, 0, 1, -1, 1, 1, 1, -1, -1, -1, 1, -1, 0, 1, -1, 1, 1, 1, -1, -1, -1},
            { 1, -1, 1, 1, -1, -1, -1, -1, 1, 1, -1, 1, 0, 1, -1, 1, 1, -1, -1, -1, -1, 1, 1, -1, 1, 0, 1, -1, 1, 1},
            { 1, 1, 0, 1, 0, 0, -1, 1, 0, 0, -1, 0, -1, -1, 0, 1, 1, 0, 1, 0, 0, -1, 1, 0, 0, -1, 0, -1, -1, 0},
            { 1, 1, -1, 1, -1, -1, -1, 1, 1, -1, -1, -1, 1, -1, 1, 1, 0, 1, 1, -1, 1, -1, -1, -1, 1, 1, -1, -1, -1, 1},
            { 1, -1, -1, 1, 1, 1, 1, -1, 1, -1, 1, -1, -1, -1, -1, 1, 1, -1, 0, 1, -1, -1, 1, 1, 1, 1, -1, 1, -1, 1},
            { 1, -1, 0, 1, 1, 0, 0, -1, 0, -1, -1, 0, -1, 0, 0, 1, 1, 0, -1, 1, 0, 1, -1, 0, 1, 1, 0, 0, -1, 0},
            { 1, 1, 1, 1, -1, 1, -1, 1, 1, -1, -1, 1, 1, -1, -1, 1, -1, 1, -1, -1, -1, -1, 0, 1, 1, 1, 1, -1, 1, -1},
            { 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0},
            { 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0, 1, -1, 0},
            { 1, -1, -1, 1, 1, 1, 1, -1, 1, -1, -1, -1, 1, -1, -1, 1, -1, -1, -1, 1, -1, 1, 1, 1, 1, -1, -1, 1, 0, 1},
            { 1, 1, -1, 1, 1, -1, 1, 1, 1, 1, -1, -1, -1, 1, -1, 1, -1, 1, 1, 1, -1, -1, -1, -1, 1, -1, -1, 1, -1, -1},
            { 1, 1, 0, 1, -1, 0, -1, 1, 0, -1, 0, 0, -1, -1, 0, 1, 1, 0, -1, -1, 0, 0, -1, 0, 1, -1, 0, -1, 1, 0},
            { 1, -1, 1, 1, 0, -1, 0, -1, 1, 0, 1, 1, 1, 0, 0, 1, 1, -1, -1, 0, 0, -1, -1, -1, 0, -1, 1, 0, 1, 0},
            { 1, -1, 1, 1, -1, -1, 1, -1, 1, 1, 1, 1, -1, -1, -1, 1, -1, -1, -1, -1, 1, -1, -1, -1, 1, 1, 1, 1, -1, 1},
            { 1, 1, 0, 1, 1, 0, -1, 1, 0, 1, 1, 0, 0, -1, 0, 1, -1, 0, -1, 1, 0, 1, -1, 0, 1, 0, 0, -1, -1, 0},
            { 1, 1, -1, 1, 1, -1, -1, 1, 1, 1, -1, -1, -1, -1, -1, 1, -1, 1, -1, 1, 1, -1, 1, -1, 1, -1, -1, -1, -1, -1},
            { 1, -1, -1, 1, -1, 1, -1, -1, 1, 1, 1, -1, 1, 1, 1, 1, 1, -1, -1, -1, 1, -1, 1, 1, 1, -1, -1, -1, -1, -1},
            { 1, -1, 0, 1, 0, 0, -1, -1, 0, 0, 1, 0, -1, 1, 0, 1, -1, 0, 1, 0, 0, -1, -1, 0, 0, 1, 0, -1, 1, 0},
            { 1, 1, 1, 1, -1, 1, 1, 1, 1, -1, -1, 1, -1, 1, -1, 1, 1, 1, -1, -1, 1, -1, -1, 1, 1, -1, 1, 1, -1, -1},
            { 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1},
            { 1, -1, 0, 1, 1, 0, -1, -1, 0, -1, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 0, -1, 1, 0, 1, -1, 0, -1, 1, 0},
            { 1, -1, -1, 1, -1, 1, 1, -1, 1, 1, 1, -1, 1, -1, 1, 1, 1, -1, -1, -1, -1, -1, -1, 1, 1, -1, -1, 1, 1, -1},
            { 1, 1, -1, 1, 0, -1, 1, 1, 1, 0, 0, -1, 1, 1, 0, 1, 1, 1, -1, 0, -1, 0, -1, -1, 0, 1, -1, 1, -1, 0},
            { 1, 1, 0, 1, -1, 0, 1, 1, 0, -1, -1, 0, -1, 1, 0, 1, -1, 0, 0, -1, 0, -1, -1, 0, 1, -1, 0, 1, 1, 0},
            { 1, -1, 1, 1, 1, -1, 1, -1, 1, -1, -1, 1, -1, -1, 1, 1, 1, -1, 1, 1, 1, 1, -1, -1, 1, 1, 1, 1, 1, -1},
        };
        #endregion

        [TestMethod]
        public void JacobiSymbol()
        {
            // должно бросить исключение на отрицательном n
            Assert.ThrowsException<ArgumentException>(() =>
            {
                MathUtils.JacobiSymbol(8, -1);
            });

            // должно бросить исключение на нулевом n
            Assert.ThrowsException<ArgumentException>(() =>
            {
                MathUtils.JacobiSymbol(8, 0);
            });

            // должно бросить исключение на четном n
            Assert.ThrowsException<ArgumentException>(() =>
            {
                MathUtils.JacobiSymbol(8, 2);
            });

            for (int i = 0; i < _jacobiSymbolValues.GetUpperBound(0); ++i)
            {
                for (int j = 0; j < _jacobiSymbolValues.GetUpperBound(1); ++j)
                {
                    int k = j + 1;
                    int n = 2 * i + 1;
                    Assert.AreEqual(_jacobiSymbolValues[i, j], MathUtils.JacobiSymbol(k, n));
                }
            }
        }

        #endregion

        #region ExtendedGreatestCommonDivisor test

        #region Test data
        private static (BigInteger n, BigInteger m, BigInteger answer)[] _greatestCommonDivisorTestCases
                    = new (BigInteger n, BigInteger m, BigInteger answer)[]
                {
            (2, 4, 2),
            (1, 5, 1),
            (3, 6, 3),
            (4, 12, 4),
            (6, 14, 2),
                };
        #endregion

        [TestMethod]
        public void ExtendedGreatestCommonDivisor()
        {
            // должно бросить исключение на отрицательном m
            Assert.ThrowsException<ArgumentException>(() =>
            {
                MathUtils.ExtendedGreatestCommonDivisor(10, -1, out _, out _);
            });

            // должно бросить исключение на отрицательном n
            Assert.ThrowsException<ArgumentException>(() =>
            {
                MathUtils.ExtendedGreatestCommonDivisor(-1, 10, out _, out _);
            });

            // должно вернуть значение ненулевого аргумента 
            Assert.AreEqual(MathUtils.ExtendedGreatestCommonDivisor(10, 0, out _, out _), 10);
            Assert.AreEqual(MathUtils.ExtendedGreatestCommonDivisor(0, 10, out _, out _), 10);

            // проверка на простых случаях, что считается именно НОД, а не что-то удовлетворяющее условию связи
            foreach (var tuple in _greatestCommonDivisorTestCases)
            {
                Assert.AreEqual(tuple.answer, MathUtils.ExtendedGreatestCommonDivisor(tuple.n, tuple.m, out _, out _));
            }

            // проверка услови связи d с коэффициентам a и b
            Random rnd = new Random(0);
            BigInteger maxBigInteger = BigInteger.Parse("100000000000000000000");

            const int nNumbersToTest = 10000;
            for (int i = 0; i < nNumbersToTest; ++i)
            {
                BigInteger n = rnd.NextBigInteger(maxBigInteger);
                BigInteger m = rnd.NextBigInteger(maxBigInteger);

                BigInteger d = MathUtils.ExtendedGreatestCommonDivisor(n, m, out BigInteger a, out BigInteger b);

                Assert.AreEqual(d, a * n + b * m, $"{d} != {a} * {n} + {b} * {m}");

                Assert.AreEqual(d, BigInteger.GreatestCommonDivisor(n, m), $"gcd({n}, {m}) != {d}");
            }
        }

        #endregion


        #region ModularMultiplicativeInverse test

        #region Test data
        private static (BigInteger n, BigInteger m, BigInteger answer)[] _modularMultiplicativeInverseTestCases
                    = new (BigInteger n, BigInteger m, BigInteger answer)[]
                {
                    (3, 26, 9),
                    (3, 7, 5)
                };
        #endregion

        private bool NaiveModularInverse(BigInteger n, BigInteger m, out BigInteger inverse)
        {
            for (BigInteger j = 1; j < m; ++j)
            {
                var toCheck = (n * j) % m;
                if (toCheck == 1)
                {
                    inverse = j;
                    return true;
                }
            }

            inverse = BigInteger.Zero;
            return false;
        }

        [TestMethod]
        public void ModularMultiplicativeInverse()
        {
            // проверка на простых случаях, что считается именно НОД, а не что-то удовлетворяющее условию связи
            foreach (var tuple in _modularMultiplicativeInverseTestCases)
            {
                bool success = MathUtils.ModularMultiplicativeInverse(tuple.n, tuple.m, out var inverse);
                Assert.IsTrue(success);
                Assert.AreEqual(tuple.answer, inverse);
            }

            Random rnd = new Random(0);
            BigInteger maxBigInteger = BigInteger.Parse("10000");

            const int nNumbersToTest = 10000;
            for (int i = 0; i < nNumbersToTest; ++i)
            {
                BigInteger m = rnd.NextBigInteger(1, maxBigInteger);
                BigInteger n = rnd.NextBigInteger(1, m);

                bool exgcdSuccess = MathUtils.ModularMultiplicativeInverse(n, m, out var exgcdInverse);
                bool naiveSuccess = NaiveModularInverse(n, m, out var naiveInverse);

                Assert.AreEqual(naiveSuccess, exgcdSuccess, $"n = {n}, m = {m}: naive success: {naiveSuccess}, extended gcd succes: {exgcdSuccess}");
                if (naiveSuccess)
                {
                    Assert.AreEqual(naiveInverse, exgcdInverse, $"n = {n}, m = {m}: naive inverse: {naiveInverse}, extended gcd inverse: {exgcdInverse}");

                }
            }
        }

        #endregion
    }
}
