using System;

namespace CryptoTask1
{
    public class ArgumentParsingException : Exception
    {
        public ArgumentParsingException(string message) : base(message) { }
    }
}
