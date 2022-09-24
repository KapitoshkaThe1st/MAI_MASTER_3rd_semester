using CommandLine;

namespace CryptoTask1.CommandLineArgsParsing.Options
{
    [Verb("clear-bits", HelpText = "Clear <count> least significant bits setting them to 0")]
    class ClearBitsOptions : BaseOptions
    {
        [Option('c', "count", Required = true, HelpText = "Count of least significant bits to clear")]
        public int Count { get; set; }
    }
}
