using CommandLine;

namespace CryptoTask1.CommandLineArgsParsing.Options
{
    [Verb("get", HelpText = "Get bit of number at index")]
    class KthBitOptions : BaseOptions
    {
        [Option('i', "index", Required = true, HelpText = "Index of bit to get")]
        public int Index { get; set; }
    }
}
