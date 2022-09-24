using CommandLine;

namespace CryptoTask1.CommandLineArgsParsing.Options
{
    [Verb("outer-bits", HelpText = "Get <count> both least- and most significant bits of <number>, dropping the middle.")]
    class OuterBitsOptions : BaseOptions
    {
        [Option('c', "count", Required = true, HelpText = "Count of most significant and least significant bits to preserve")]
        public int Count { get; set; }
    }
}
