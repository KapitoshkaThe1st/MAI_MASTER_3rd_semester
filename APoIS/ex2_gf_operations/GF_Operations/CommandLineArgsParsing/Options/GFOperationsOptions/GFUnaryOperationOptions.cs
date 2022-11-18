using CommandLine;

namespace GF_Operations.CommandLineArgsParsing.Options
{
    public class GFUnaryOperationOptions : BaseOptions
    {
        private string _gfElementString;
        [Option('e', "element", Required = true, HelpText = "GF(256) element to perform operation on")]
        public string GFElementStr
        {
            get
            {
                ThrowNotSupported();
                return string.Empty;
            }
            set
            {
                _gfElementString = value;
            }
        }

        private bool _parsed = false;
        private uint _gfElement;
        public uint GFElement
        {
            get
            {
                if (_parsed)
                {
                    return _gfElement;
                }

                if (!GF256.TryParse(_gfElementString, out _gfElement))
                {
                    throw new ArgumentParsingException("Not valid format for parameter.");
                }
                _parsed = true;
                return _gfElement;
            }
        }
    }
}
