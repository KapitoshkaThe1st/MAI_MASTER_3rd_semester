using CommandLine;

namespace Rijndael.Options
{
    [Verb("file-encode", HelpText = "Encodes file with Rijndael algorithm")]
    class FileEncodingOptions : FileOperationOptions
    {
    }
}
