using System;
using System.Linq;

using CommandLine;

namespace CryptoTask1.CommandLineArgsParsing.Options
{
    public class BaseOptions
    {
        private string _numberStr;
        [Option('n', "number", Required = true, HelpText = "Number to perform operation on")]
        public string NumberStr
        {
            get => throw new NotSupportedException(@"This prooperty is public for argument parsing and can't be private
                (thanks to CommandLine library for not providing argument validation out of the box). Use 'Number' property instead.");
            set
            {
                _numberStr = value;
            }
        }

        private bool _parsed = false;
        private uint _number;
        public uint Number
        {
            get
            {
                if (_parsed)
                {
                    return _number;
                }
                _number = BinaryParser.ParseUint(_numberStr);
                _parsed = true;
                return _number;
            }
        }
    }
}