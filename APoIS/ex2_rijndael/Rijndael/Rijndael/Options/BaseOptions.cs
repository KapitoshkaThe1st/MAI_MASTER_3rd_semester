using CommandLine;

namespace Rijndael.Options
{
    class BaseOptions
    {
        public const string DefaultIVString = "DEFAULT";

        [Option('k', "key", Required = true, HelpText = "key for Rijndael encoding/decoding")]
        public string Key { get; set; }

        [Option('b', "block-length", Required = true, HelpText = "key for Rijndael encoding/decoding")]
        public string BlockLength { get; set; }

        [Option('m', "mode", Default = "ECB", Required = false, HelpText = "Block chaining mode (ECB, CBC, CFB, OFB)")]
        public string Mode { get; set; }

        [Option("iv", Default = DefaultIVString, Required = false, HelpText = "Initial vector for CBC, CFB, OFB block chaining modes")]
        public string IV { get; set; }
    }
}
