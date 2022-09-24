using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTask1.CommandLineArgsParsing
{
    public class BinaryParser
    {
        public static uint ParseUint(string numberString)
        {
            if (numberString.Length == 0)
            {
                throw new ArgumentParsingException("Not valid format for parameter. Empty string provided.");
            }

            uint result = 0;
            for (int i = 0; i < numberString.Length; ++i)
            {
                char bitChar = numberString[i];
                if (bitChar == '1')
                {
                    int intdexToSet = numberString.Length - 1 - i;
                    if (intdexToSet >= sizeof(uint) * 8)
                    {
                        throw new ArgumentParsingException("Not valid format for parameter. Number provided exceeds 'uint' type size");
                    }
                    result = BitOperations.SetBit(result, intdexToSet);
                }
                else if (bitChar != '0')
                {
                    throw new ArgumentParsingException("Not valid format for parameter. Must be string consisting of 0 or 1 only.");
                }
            }

            return result;
        }
    }
}
