using CommandLine;

namespace CryptoTask1.CommandLineArgsParsing.Options
{
    [Verb("rotate-left", HelpText = "Perform left rotate (circular shift) of <number> by <count> positions")]
    class RotateLeftOptions : BaseOptions
    {
        [Option('c', "count", Required = true, HelpText = "rotate left by <count> positions")]
        public int Count { get; set; }
    }
}
