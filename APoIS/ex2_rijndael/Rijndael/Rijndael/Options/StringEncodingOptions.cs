using CommandLine;

namespace Rijndael.Options
{
    [Verb("string-encode", HelpText = "Encodes string with DES algorithm")]
    class StringEncodingOptions : BaseOptions
    {
        [Option('s', "string", Required = true, HelpText = "string to be encoded")]
        public string String { get; set; }
    }
}
