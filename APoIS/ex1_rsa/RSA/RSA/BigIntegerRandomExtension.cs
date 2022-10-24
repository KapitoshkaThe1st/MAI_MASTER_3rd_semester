using System;
using System.Numerics;

namespace RSA
{
    // https://stackoverflow.com/a/48855115
    public static class BigIntegerRandomExtension
    {
        private static byte ResetUpperBits(byte num, int count)
        {
            if (count >= 8)
                return 0;

            return unchecked((byte)(((num << count) & 0xFF) >> count));
        }

        // returns index of most significant bit that is set
        private static int GetMostSignificantBitSet(byte num)
        {
            int r = 0;
            while ((num >>= 1) != 0)
            {
                r++;
            }
            return r;
        }

        public static BigInteger NextBigInteger(this Random rnd, BigInteger max)
        {
            if (max <= BigInteger.Zero)
            {
                throw new ArgumentException("'max' parameter must be greater than zero");
            }

            int nBytes = max.GetByteCount();
            var boundByteArr = max.ToByteArray();

            byte lastByteOfBound = boundByteArr[^1];
            int mostSignBitPos = GetMostSignificantBitSet(lastByteOfBound);
            byte zeroingMask = ResetUpperBits(byte.MaxValue, 8 - mostSignBitPos - 1);

            while (true)
            {
                var bytes = new byte[nBytes];
                rnd.NextBytes(bytes);

                bytes[^1] &= zeroingMask;
                var candidate = new BigInteger(bytes);

                if (candidate < max)
                {
                    return candidate;
                }
            }
        }

        public static BigInteger NextBigInteger(this Random rnd, BigInteger min, BigInteger max)
        {
            if(min >= max)
            {
                throw new ArgumentException("'min' parameter must be lower than 'max' parameter");
            }

            var bound = max - min;
            return min + rnd.NextBigInteger(bound);
        }

        public static BigInteger NextBigInteger(this Random rnd, BigInteger max, out int iterationsTaken)
        {
            if (max <= BigInteger.Zero)
            {
                throw new ArgumentException("'max' parameter must be greater than zero");
            }

            int nBytes = max.GetByteCount();
            var boundByteArr = max.ToByteArray();

            byte lastByteOfBound = boundByteArr[^1];
            int mostSignBitPos = GetMostSignificantBitSet(lastByteOfBound);
            byte zeroingMask = ResetUpperBits(byte.MaxValue, 8 - mostSignBitPos - 1);

            iterationsTaken = 0;

            while (true)
            {
                var bytes = new byte[nBytes];
                rnd.NextBytes(bytes);

                bytes[^1] &= zeroingMask;
                var candidate = new BigInteger(bytes);

                iterationsTaken++;
                if (candidate < max)
                {
                    return candidate;
                }
            }
        }

        public static BigInteger NextBigInteger(this Random rnd, BigInteger min, BigInteger max, out int iterationsTaken)
        {
            if (min >= max)
            {
                throw new ArgumentException("'min' parameter must be lower than 'max' parameter");
            }

            var bound = max - min;
            return min + rnd.NextBigInteger(bound, out iterationsTaken);
        }
    }
}
