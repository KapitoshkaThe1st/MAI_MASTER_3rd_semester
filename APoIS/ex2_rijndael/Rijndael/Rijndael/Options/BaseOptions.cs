using CommandLine;

namespace Rijndael.Options
{
    class BaseOptions
    {
        [Option('k', "key", Required = true, HelpText = "key for Rijndael encoding/decoding")]
        public string Key { get; set; }

        [Option('b', "block-length", Required = true, HelpText = "key for Rijndael encoding/decoding")]
        public string BlockLength { get; set; }

        [Option('m', "mode", Default = "ECB", Required = false, HelpText = "Block cipher mode of operation (ECB, CBC, CFB, OFB)")]
        public string Mode { get; set; }
    }
}
