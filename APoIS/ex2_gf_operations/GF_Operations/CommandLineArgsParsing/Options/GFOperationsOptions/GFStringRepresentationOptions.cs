using System;
using CommandLine;

namespace GF_Operations.CommandLineArgsParsing.Options
{
    [Verb("gf-string-repr", HelpText = "Prints string representation of GF(256) element")]
    public class GFStringRepresentationOptions : BaseOptions
    {
        private string _gfElementString;
        [Option('e', "element", Required = true, HelpText = "element to perform operation on in binary format with 0b- prefix")]
        public string GFElementString
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

        private bool _gfElementParsed = false;
        private uint _gfElement;
        public uint GFElement
        {
            get
            {
                if (_gfElementParsed)
                {
                    return _gfElement;
                }

                if (!_gfElementString.StartsWith("0b"))
                    throw new ArgumentParsingException("Invalid format: missed 0b- prefix fot binary representation");

                _gfElement = Convert.ToUInt32(_gfElementString.Substring(2), 2);

                _gfElementParsed = true;
                return _gfElement;
            }
        }
    }
}
