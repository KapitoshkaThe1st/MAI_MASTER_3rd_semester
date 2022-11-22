using System;
using System.Text.RegularExpressions;

namespace DES
{
    public static class BinaryFormatting
    {
        public static string Format(ulong num)
        {
            return Format(num, 8, 64);
        }

        public static string Format(ulong num, int bitsCount)
        {
            return Format(num, 8, bitsCount);
        }

        public static string Format(ulong num, int bitsToSeparateCount, int bitsCount)
        {
            var binRepr = Convert.ToString((long)num, 2).PadLeft(bitsCount, '0');
            return Regex.Replace(binRepr, $".{{{bitsToSeparateCount}}}", "$0 ");
        }

        public static string Format(uint num)
        {
            return Format(num, 8, 32);
        }

        public static string Format(uint num, int bitsCount)
        {
            return Format(num, 8, bitsCount);
        }

        public static string Format(uint num, int bitsToSeparateCount, int bitsCount)
        {
            var binRepr = Convert.ToString(num, 2).PadLeft(bitsCount, '0');
            return Regex.Replace(binRepr, $".{{{bitsToSeparateCount}}}", "$0 ");
        }
    }
}
