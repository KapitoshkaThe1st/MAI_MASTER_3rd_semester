using CommandLine;

namespace CryptoTask1.CommandLineArgsParsing.Options
{
    [Verb("inner-bits", HelpText = "Drop middle <count> least- and most significant bits preserving the middle of <number>")]
    class InnerBitsOptions : BaseOptions
    {
        [Option('c', "count", Required = true, HelpText = "Count of most significant and least significant bits to drop")]
        public int Count { get; set; }
    }
}
