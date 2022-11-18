using CommandLine;

namespace GF_Operations.CommandLineArgsParsing.Options.BinaryPolynomialOperationsOptions
{
    class BinaryPolynomialUnaryOperationOptions : BaseOptions
    {
        private string _polynomialString;
        [Option('p', "polynomial", Required = true, HelpText = "binary polynomial to perform operation on")]
        public string PolynomialStr
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
        private ulong _polynomial;
        public ulong Polynomial
        {
            get
            {
                if (_parsed)
                {
                    return _polynomial;
                }

                if (!BinaryPolynomial32.TryParse(_polynomialString, out _polynomial))
                {
                    throw new ArgumentParsingException("Not valid format for parameter.");
                }
                _parsed = true;
                return _polynomial;
            }
        }
    }
}
