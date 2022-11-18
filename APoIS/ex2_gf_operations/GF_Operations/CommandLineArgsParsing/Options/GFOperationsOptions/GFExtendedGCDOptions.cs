using CommandLine;

namespace GF_Operations.CommandLineArgsParsing.Options
{
    [Verb("gf-gcd", HelpText = "Calculates GCD of two GF(256) elements")]
    public class GFExtendedGCDOptions : GFBinaryOperationWithModuloOptions
    {
    }
}
