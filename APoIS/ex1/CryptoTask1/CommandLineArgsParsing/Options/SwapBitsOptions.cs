using CommandLine;

namespace CryptoTask1.CommandLineArgsParsing.Options
{
    [Verb("swap", HelpText = "Swap bits of number at indices <index1> and <index2>")]
    class SwapBitsOptions : BaseOptions
    {
        [Option("index1", Required = true, HelpText = "First index")]
        public int Index1 { get; set; }

        [Option("index2", Required = true, HelpText = "Second index")]
        public int Index2 { get; set; }
    }
}
