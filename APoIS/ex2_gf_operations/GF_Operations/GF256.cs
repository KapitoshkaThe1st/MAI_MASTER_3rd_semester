using System;
using System.Text;
using System.Text.RegularExpressions;

namespace GF_Operations
{
    public static class GF256
    {
        public static uint Add(uint a, uint b)
        {
            return a ^ b;
        }

        public static uint Multiply(uint a, uint b, uint m)
        {
            uint p = 0; // аккумулятор 
            while (a != 0 && b != 0) // пока есть что и на что умножать
            {
                if ((b & 1) != 0) // если в многочлене b есть член '1' , то "добавляем" (^ -- операция сложения для поля Галуа) весь многочлен a к в аккумулятор единожды
                    p ^= a; /* addition in GF(2^m) is an XOR of the polynomial coefficients */

                if ((a & 0x80) != 0) // если есть x^7 (т.е. член с максимальной допустимой степенью для GF(256)), тогда после сдвига (ниже), он должен быть приведен опять в GF(256) */
                    a = (a << 1) ^ m; // вычитаем неприводимый многочлен m, чтобы результат помещался в 8 бит -- операция умножения по модулю для многочленов в поле Галуа
                else
                    a <<= 1; // умножение многочлена a на x
                b >>= 1; // деление многочлена b на x
            }

            return p;
        }

        private static int BitLength(uint b)
        {
            int length = 0;
            while (b != 0)
            {
                b >>= 1;
                length++;
            }

            return length;
        }

        public static uint DivRem(uint a, uint b, out uint r)
        {
            if (b == 0)
            {
                throw new ArgumentException("b must be non-zero");
            }

            int bBitLength = BitLength(b);

            uint result = 0;

            while (a >= b)
            {
                int aBitLength = BitLength(a);

                int c = aBitLength - bBitLength;

                a ^= b << c;
                result |= (uint)(1 << c);
            }

            r = a;
            return result;
        }

        public static uint ExtendedGreatestCommonDivisor(uint a, uint b, uint m, out uint u, out uint w)
        {
            if (a < b)
            {
                return ExtendedGreatestCommonDivisor(b, a, m, out w, out u);
            }

            if (a == 0)
            {
                u = 0;
                w = 1;
                return b;
            }

            if (b == 0)
            {
                u = 1;
                w = 0;
                return a;
            }

            uint r0 = b;
            uint r1 = a;

            uint u0 = 0;
            uint u1 = 1;

            uint w0 = 1;
            uint w1 = 0;

            int j = 0;
            while (true)
            {
                if (j % 2 == 0)
                {
                    var q = DivRem(r0, r1, out r0);
                    u0 = Add(u0, Multiply(q, u1, m));
                    w0 = Add(w0, Multiply(q, w1, m));

                    if (r0 == 0)
                    {
                        u = u1;
                        w = w1;
                        return r1;
                    }
                }
                else
                {
                    var q = DivRem(r1, r0, out r1);
                    u1 = Add(u1, Multiply(q, u0, m));
                    w1 = Add(w1, Multiply(q, w0, m));


                    if (r1 == 0)
                    {
                        u = u0;
                        w = w0;
                        return r0;
                    }
                }
                j++;
            }
        }

        public static uint Inverse(uint e, uint modulo)
        {
            if (ExtendedGreatestCommonDivisor(e, modulo, modulo, out uint a, out uint b) == 1)
            {
                return a;
            }

            throw new ArithmeticException($"Could not find multiplicative inverse for {e}");
        }

        private const int _bitSize = 8;

        public static string StringRepresentation(uint e)
        {
            StringBuilder sb = new StringBuilder();

            if (e == 0)
            {
                return "0";
            }

            for (int i = _bitSize - 1; i >= 0; --i)
            {
                if ((e & (1 << (_bitSize - 1))) != 0)
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
                e <<= 1;
            }

            return sb.ToString();
        }

        #region Parsing

        // TODO: пока что корректно распознает многочлены вида x^2 x^3, хотя не должно, но лаба не про регулярки все-таки
        private static Lazy<Regex> _formatValidationRegex = new Lazy<Regex>(() => new Regex(@"^(?:\s*\+?\s*(x\^[0-7]+|x|1))+$", RegexOptions.Compiled));
        private static Lazy<Regex> _polynomialTermCapturingRegex = new Lazy<Regex>(() => new Regex(@"(x\^[0-7]+|x|1)", RegexOptions.Compiled));

        public static bool TryParse(string polynomString, out uint result)
        {
            uint resultBits = 0;

            if (!_formatValidationRegex.Value.IsMatch(polynomString))
            {
                result = 0;
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
                        result = 0;
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
                resultBits ^= (uint)(1 << index);
            }

            result = resultBits;
            return true;
        }

        #endregion
    }
}
