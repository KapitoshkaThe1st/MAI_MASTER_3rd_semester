using CommandLine;

namespace Rijndael.Options
{
    class FileOperationOptions : BaseOptions
    {
        [Option('i', "input", Required = true, HelpText = "input file path")]
        public string InputFilePath { get; set; }

        [Option('o', "output", Required = true, HelpText = "output file path")]
        public string OutputFilePath { get; set; }
    }
}
