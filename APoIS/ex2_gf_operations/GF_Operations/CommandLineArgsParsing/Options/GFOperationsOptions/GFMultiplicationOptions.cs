using CommandLine;

namespace GF_Operations.CommandLineArgsParsing.Options
{
    [Verb("gf-multiply", HelpText = "Multiplies two GF(256) elements")]
    public class GFMultiplicationOptions : GFBinaryOperationWithModuloOptions
    {
     
    }
}
