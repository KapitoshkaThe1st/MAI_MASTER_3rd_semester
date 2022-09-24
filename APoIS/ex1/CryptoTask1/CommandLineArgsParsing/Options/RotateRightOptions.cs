using CommandLine;

namespace CryptoTask1.CommandLineArgsParsing.Options
{
    [Verb("rotate-right", HelpText = "Perform right rotate (circular shift) of <number> by <count> positions")]
    class RotateRightOptions : BaseOptions
    {
        [Option('c', "count", Required = true, HelpText = "rotate right by <count> positions")]
        public int Count { get; set; }
    }
}
