using CommandLine;

namespace CryptoTask1.CommandLineArgsParsing.Options
{
    [Verb("set", HelpText = "Get bit of number at index to 1")]
    class SetBitOptions : BaseOptions
    {
        [Option('i', "index", Required = true, HelpText = "Index of bit to set")]
        public int Index { get; set; }
    }
}
