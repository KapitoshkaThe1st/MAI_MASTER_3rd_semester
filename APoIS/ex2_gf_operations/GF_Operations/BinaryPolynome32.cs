using System;
using System.Text;
using System.Text.RegularExpressions;

namespace GF_Operations
{
    // https://link.springer.com/chapter/10.1007/978-3-642-14764-7_40
    public struct BinaryPolynome32
    {
        public static readonly BinaryPolynome32 Zero = new BinaryPolynome32(0);
        public static readonly BinaryPolynome32 One = new BinaryPolynome32(1);

        private const int _bitSize = sizeof(uint) * 8;
        private uint _bits;

        public BinaryPolynome32(uint bits)
        {
            _bits = bits;
        }

        private static uint BinaryMultiplication(uint a, uint b)
        {
            uint t = 0;
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
        private static uint BinaryAddition(uint a, uint b)
        {
            return a ^ b;
        }

        public static BinaryPolynome32 operator* (BinaryPolynome32 f, BinaryPolynome32 g)
        {
            return new BinaryPolynome32(BinaryMultiplication(f._bits, g._bits));
        }

        public static BinaryPolynome32 operator +(BinaryPolynome32 f, BinaryPolynome32 g)
        {
            return new BinaryPolynome32(BinaryAddition(f._bits, g._bits));
        }

        public static BinaryPolynome32 operator -(BinaryPolynome32 f, BinaryPolynome32 g)
        {
            return f + g;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            uint data = _bits;

            if(data == 0)
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

        private static Lazy<Regex> _formatValidationRegex = new Lazy<Regex>(() => new Regex(@"^(?:\s*\+?\s*(x\^[0-9]{1,2}|x|1))+$", RegexOptions.Compiled));
        private static Lazy<Regex> _polynomialTermCapturingRegex = new Lazy<Regex>(() => new Regex(@"(x\^[0-9]{1,2}|x|1)", RegexOptions.Compiled));

        public static bool TryParse(string polynomString, out BinaryPolynome32 result)
        {
            uint resultBits = 0;

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
                    if(index > _bitSize - 1)
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
                resultBits ^= (uint)(1 << index);
            }

            result = new BinaryPolynome32(resultBits);
            return true;
        }
    }
}
