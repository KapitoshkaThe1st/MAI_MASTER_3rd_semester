using CommandLine;

namespace DES.Options
{
    class BaseOptions
    {
        [Option('k', "key", Required = true, HelpText = "key for DES encoding/decoding")]
        public string Key { get; set; }
    }
}
