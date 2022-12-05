using CommandLine;

namespace Rijndael.Options
{
    [Verb("string-decode", HelpText = "Decodes string encoded with DES algorithm")]
    class StringDecodingOptions : BaseOptions
    {
        [Option('c', "cipher", Required = true, HelpText = "cipher to be decoded")]
        public string Cipher { get; set; }
    }
}
