using CommandLine;

namespace GF_Operations.CommandLineArgsParsing.Options
{
    [Verb("gf-inverse", HelpText = "Calculates multiplicative inverse of GF(256) element")]
    public class GFMultiplicativeInverseOptions : GFUnaryOperationWithModuloOptions
    {
    }
}
