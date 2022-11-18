using CommandLine;

namespace GF_Operations.CommandLineArgsParsing.Options
{
    [Verb("gf-parse", HelpText = "Parses GF(256) element in polynomial form and prints it's decimal, hexadeximal and binary representation")]
    public class GFPolynomialFormParsingOptions : GFUnaryOperationOptions
    {

    }
}
