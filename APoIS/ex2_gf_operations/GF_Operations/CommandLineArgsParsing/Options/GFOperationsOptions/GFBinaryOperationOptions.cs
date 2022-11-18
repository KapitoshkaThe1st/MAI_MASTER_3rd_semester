using CommandLine;
using System;

namespace GF_Operations.CommandLineArgsParsing.Options
{
    public class GFBinaryOperationOptions : BaseOptions
    {

        private string _gfElementString1;
        [Option("element1", Required = true, HelpText = "GF(256) element which is the first operand to perform operation on")]
        public string GFElementStr1
        {
            get {
                ThrowNotSupported();
                return string.Empty;
            }
            set
            {
                _gfElementString1 = value;
            }
        }

        private string _gfElementString2;
        [Option("element2", Required = true, HelpText = "GF(256) element which is the second operand to perform operation on")]
        public string GFElementStr2
        {
            get
            {
                ThrowNotSupported();
                return string.Empty;
            }
            set
            {
                _gfElementString2 = value;
            }
        }

        private bool _parsed1 = false;
        private uint _gfElement1;
        public uint GFElement1
        {
            get
            {
                if (_parsed1)
                {
                    return _gfElement1;
                }

                if(!GF256.TryParse(_gfElementString1, out _gfElement1))
                {
                    throw new ArgumentParsingException("Not valid format for parameter.");
                }
                _parsed1 = true;
                return _gfElement1;
            }
        }

        private bool _parsed2 = false;
        private uint _gfElement2;
        public uint GFElement2
        {
            get
            {
                if (_parsed2)
                {
                    return _gfElement2;
                }

                if (!GF256.TryParse(_gfElementString2, out _gfElement2))
                {
                    throw new ArgumentParsingException("Not valid format for parameter.");
                }
                _parsed2 = true;
                return _gfElement2;
            }
        }
    }
}
