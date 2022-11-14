using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GF_Operations
{
    public struct GF256 : IEquatable<GF256>, IComparable<GF256>
    {
        public static readonly GF256 Zero = new GF256(0);
        public static readonly GF256 One = new GF256(1);

        private const int _bitSize = sizeof(uint) * 8;
        private byte _bits;

        public GF256(byte bits)
        {
            _bits = bits;
        }

        private static byte BinaryAddition(byte a, byte b)
        {
            return (byte)(a ^ b);
        }

        public static GF256 Add(GF256 a, GF256 b)
        {
            return new GF256(BinaryAddition(a._bits, b._bits));
        }

        //Russian peasant multiplication algorithm
        //https://en.wikipedia.org/wiki/Finite_field_arithmetic
        public static GF256 Multiply(GF256 a, GF256 b, uint m)
        {
            byte p = 0; // аккумулятор 
            while (a._bits != 0 && b._bits != 0) // пока есть что и на что умножать
            {
                if ((b._bits & 1) != 0) // если в многочлене b есть член '1' , то "добавляем" (^ -- операция сложения для поля Галуа) весь многочлен a к в аккумулятор единожды
                    p ^= a._bits; /* addition in GF(2^m) is an XOR of the polynomial coefficients */

                if ((a._bits & 0x80) != 0) // если есть x^7 (т.е. член с максимальной допустимой степенью для GF(256)), тогда после сдвига (ниже), он должен быть приведен опять в GF(256) */
                    a._bits = (byte)((a._bits << 1) ^ m); // вычитаем неприводимый многочлен m, чтобы результат помещался в 8 бит -- операция умножения по модулю для многочленов в поле Галуа
                else
                    a._bits <<= 1; // умножение многочлена a на x
                b._bits >>= 1; // деление многочлена b на x
            }

            return new GF256(p);
        }

        static int BitLength(byte b)
        {
            int length = 0;
            while (b != 0)
            {
                b >>= 1;
                length++;
            }

            return length;
        }

        public static GF256 DivRem(GF256 a, GF256 b, out GF256 r)
        {
            if(b == Zero)
            {
                throw new ArgumentException("b must be non-zero");
            }

            byte aByte = (byte)a;
            byte bByte = (byte)b;

            int bBitLength = BitLength(bByte);

            byte result = 0;

            while (aByte >= bByte)
            {
                int aBitLength = BitLength(aByte);

                int c = aBitLength - bBitLength;

                aByte ^= (byte)(bByte << c);
                result |= (byte)(1 << c);
            }

            r = new GF256(aByte);
            return new GF256(result);
        }

        public static GF256 operator +(GF256 f, GF256 g)
        {
            return Add(f, g);
        }

        public static GF256 operator -(GF256 f, GF256 g)
        {
            return f + g;
        }

        public static GF256 Pow(GF256 a, int n, uint m)
        {
            GF256 res = new GF256(1);
            while (n > 0)
            {
                if ((n & 1) != 0)
                    res = Multiply(res, a, m);
                a = Multiply(a, a, m);
                n >>= 1;
            }
            return res;
        }

        //public static GF256 Inverse(GF256 e, uint m)
        //{
        //    if (e == Zero)
        //    {
        //        throw new ArgumentException("f must be non-zero");
        //    }

        //    return Pow(e, 254, m);
        //}

        public static GF256 Inverse(GF256 e, uint m)
        {
            if (e == Zero)
            {
                throw new ArgumentException("f must be non-zero");
            }

            if (ExtendedGreatestCommonDivisor(num, modulus, out BigInteger a, out BigInteger b) == 1)
            {
                //result = ((num * a) % modulus == BigInteger.One) ? a : b;
                GF256 toCheck = (e * a) % modulus;
                if (toCheck == BigInteger.One || toCheck == (BigInteger.One - modulus))
                {
                    result = a;
                }
                else
                {
                    result = b;
                }

                if (result < 0)
                {
                    result = (result % modulus + modulus) % modulus;

                }

                return true;
            }
        }

        public static GF256 ExtendedGreatestCommonDivisor(GF256 a, GF256 b, uint m, out GF256 u, out GF256 w)
        {
            if(a == Zero)
            {
                u = Zero;
                w = One;
                return b;
            }

            if (b == Zero)
            {
                u = One;
                w = Zero;
                return a;
            }

            GF256 r0 = b;
            GF256 r1 = a;

            GF256 u0 = Zero;
            GF256 u1 = One;

            GF256 w0 = One;
            GF256 w1 = Zero;

            int j = 0;
            while (true)
            {
                if(j % 2 == 0)
                {
                    var q = DivRem(r0, r1, out r0);
                    u0 = u0 - Multiply(q, u1, m);
                    w0 = w0 - Multiply(q, w1, m);

                    if(r0 == Zero)
                    {
                        u = u1;
                        w = w1;
                        return r1;
                    }
                }
                else
                {
                    var q = DivRem(r1, r0, out r1);
                    u1 = u1 - Multiply(q, u0, m);
                    w1 = w1 - Multiply(q, w0, m);


                    if (r1 == Zero)
                    {
                        u = u0;
                        w = w0;
                        return r0;
                    }
                }
                j++;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            uint data = _bits;

            if (data == 0)
            {
                return "0";
            }

            for (int i = _bitSize - 1; i >= 0; --i)
            {
                if ((data & (1 << (_bitSize - 1))) != 0)
                {
                    if (sb.Length != 0)
                    {
                        sb.Append(" + ");
                    }
                    if (i == 0)
                    {
                        sb.Append("1");
                    }
                    else if (i == 1)
                    {
                        sb.Append("x");
                    }
                    else
                    {
                        sb.Append($"x^{i}");
                    }
                }
                data <<= 1;
            }

            return sb.ToString();
        }

        private static Lazy<Regex> _formatValidationRegex = new Lazy<Regex>(() => new Regex(@"^(?:\s*\+?\s*(x\^[0-7]|x|1))+$", RegexOptions.Compiled));
        private static Lazy<Regex> _polynomialTermCapturingRegex = new Lazy<Regex>(() => new Regex(@"(x\^[0-7]|x|1)", RegexOptions.Compiled));

        public static bool TryParse(string polynomString, out GF256 result)
        {
            byte resultBits = 0;

            if (!_formatValidationRegex.Value.IsMatch(polynomString))
            {
                result = Zero;
                return false;
            }

            foreach (Match match in _polynomialTermCapturingRegex.Value.Matches(polynomString))
            {
                int index;
                if (match.Value.Length > 1)
                {
                    index = Convert.ToInt32(match.Value.Substring(2));
                    if (index > _bitSize - 1)
                    {
                        result = Zero;
                        return false;
                    }
                }
                else
                {
                    if (match.Value == "x")
                    {
                        index = 1;
                    }
                    else
                    {
                        index = 0;
                    }
                }
                resultBits ^= (byte)(1 << index);
            }

            result = new GF256(resultBits);
            return true;
        }

        public bool Equals(GF256 other)
        {
            return _bits == other._bits;
        }
        
        public override int GetHashCode()
        {
            return _bits.GetHashCode();
        }

        public static explicit operator byte(GF256 e)
        {
            return e._bits;
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            GF256 otherCasted = (GF256)other;
            return _bits == otherCasted._bits;
        }

        public int CompareTo(GF256 other)
        {
            return _bits.CompareTo(other._bits);
        }

        public static bool operator <(GF256 a, GF256 b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator >(GF256 a, GF256 b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool operator ==(GF256 a, GF256 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(GF256 a, GF256 b)
        {
            return !(a == b);
        }
    }
}
