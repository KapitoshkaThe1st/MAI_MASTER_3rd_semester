using CommandLine;
using System;

namespace GF_Operations.CommandLineArgsParsing.Options.BinaryPolynomialOperationsOptions
{
    class BinaryPolynomialBinaryOperationOptions : BaseOptions
    {
        private string _polynomialString1;
        [Option("polynomial1", Required = true, HelpText = "binary polynomial which is the first operand to perform operation on")]
        public string PolynomialStr1
        {
            get
            {
                ThrowNotSupported();
                return string.Empty;
            }
            set
            {
                _polynomialString1 = value;
            }
        }

        private string _polynomialString2;
        [Option("polynomial2", Required = true, HelpText = "binary polynomial which is the second operand to perform operation on")]
        public string PolynomialStr2
        {
            get
            {
                ThrowNotSupported();
                return string.Empty;
            }
            set
            {
                _polynomialString2 = value;
            }
        }

        private bool _parsed1 = false;
        private ulong _polynomial1;
        public ulong Polynomial1
        {
            get
            {
                if (_parsed1)
                {
                    return _polynomial1;
                }

                if (!BinaryPolynomial32.TryParse(_polynomialString1, out _polynomial1))
                {
                    throw new ArgumentParsingException("Not valid format for parameter.");
                }
                _parsed1 = true;
                return _polynomial1;
            }
        }

        private bool _parsed2 = false;
        private ulong _polynomial2;
        public ulong Polynomial2
        {
            get
            {
                if (_parsed2)
                {
                    return _polynomial2;
                }

                if (!BinaryPolynomial32.TryParse(_polynomialString2, out _polynomial2))
                {
                    throw new ArgumentParsingException("Not valid format for parameter.");
                }
                _parsed2 = true;
                return _polynomial2;
            }
        }
    }
}
