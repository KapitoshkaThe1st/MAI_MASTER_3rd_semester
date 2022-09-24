using CommandLine;

namespace CryptoTask1.CommandLineArgsParsing.Options
{
    [Verb("reset", HelpText = "Set bit of number at index to 0")]
    class ResetBitOptions : BaseOptions
    {
        [Option('i', "index", Required = true, HelpText = "Index of bit to reset")]
        public int Index { get; set; }
    }
}
