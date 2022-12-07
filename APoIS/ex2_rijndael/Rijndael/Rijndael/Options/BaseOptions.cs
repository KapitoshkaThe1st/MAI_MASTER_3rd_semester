using CommandLine;

namespace Rijndael.Options
{
    class BaseOptions
    {
        [Option('k', "key", Required = true, HelpText = "key for Rijndael encoding/decoding")]
        public string Key { get; set; }

        [Option('b', "block-length", Required = true, HelpText = "key for Rijndael encoding/decoding")]
        public string BlockLength { get; set; }
    }
}
