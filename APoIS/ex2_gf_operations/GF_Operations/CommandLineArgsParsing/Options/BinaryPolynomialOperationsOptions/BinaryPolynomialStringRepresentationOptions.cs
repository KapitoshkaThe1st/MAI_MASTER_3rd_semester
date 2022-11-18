using System;
using CommandLine;

namespace GF_Operations.CommandLineArgsParsing.Options.BinaryPolynomialOperationsOptions
{
    [Verb("bin-string-repr", HelpText = "Prints string representation of binary polynomial")]
    class BinaryPolynomialStringRepresentationOptions : BaseOptions
    {
        private string _polynomialString;
        [Option('p', "polynomial", Required = true, HelpText = "binary polynomial to perform operation on in binary format with 0b- prefix")]
        public string PolynomialString
        {
            get
            {
                ThrowNotSupported();
                return string.Empty;
            }
            set
            {
                _polynomialString = value;
            }
        }

        private bool _parsed = false;
        private uint _polynomial;
        public uint Polynomial
        {
            get
            {
                if (_parsed)
                {
                    return _polynomial;
                }

                if (!_polynomialString.StartsWith("0b"))
                    throw new ArgumentParsingException("Invalid format: missed 0b- prefix fot binary representation");

                _polynomial = Convert.ToUInt32(_polynomialString.Substring(2), 2);

                _parsed = true;
                return _polynomial;
            }
        }
    }
}
