using System;
using System.Text;
using System.Text.RegularExpressions;

namespace GF_Operations
{
    // https://link.springer.com/chapter/10.1007/978-3-642-14764-7_40
    public static class BinaryPolynomial32
    {
        private static int _bitSize = 32;

        public static ulong Multiply(ulong a, ulong b)
        {
            ulong t = 0;
            while (b > 0)
            {
                if ((b & 1) == 1)
                {
                    t ^= a;
                }

                b >>= 1;
                a <<= 1;
            }
            return t;
        }
        public static ulong Add(ulong a, ulong b)
        {
            return a ^ b;
        }

        public static string StringRepresentation(ulong element)
        {
            StringBuilder sb = new StringBuilder();

            if(element == 0)
            {
                return "0";
            }

            for (int i = _bitSize - 1; i >= 0; --i)
            {
                if ((element & ((ulong)1 << (_bitSize - 1))) != 0)
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
                element <<= 1;
            }

            return sb.ToString();
        }

        private static Lazy<Regex> _formatValidationRegex = new Lazy<Regex>(() => new Regex(@"^(?:\s*\+?\s*(x\^[0-9]{1,2}|x|1))+$", RegexOptions.Compiled));
        private static Lazy<Regex> _polynomialTermCapturingRegex = new Lazy<Regex>(() => new Regex(@"(x\^[0-9]{1,2}|x|1)", RegexOptions.Compiled));

        public static bool TryParse(string polynomString, out ulong result)
        {
            ulong resultBits = 0;

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
                    if(index > _bitSize - 1)
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
                resultBits ^= (ulong)(1 << index);
            }

            result = resultBits;
            return true;
        }
    }
}
