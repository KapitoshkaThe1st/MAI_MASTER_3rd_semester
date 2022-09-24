using CommandLine;

namespace CryptoTask1.CommandLineArgsParsing.Options
{
    [Verb("parity", HelpText = "Compute parity of <number> (1 if an odd number of bits set, 0 otherwise)")]
    class ParityOptions : BaseOptions { }
}
