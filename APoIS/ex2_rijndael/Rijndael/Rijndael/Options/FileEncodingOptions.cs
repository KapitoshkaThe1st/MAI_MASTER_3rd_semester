using CommandLine;

namespace Rijndael.Options
{
    [Verb("file-encode", HelpText = "Encodes file with DES algorithm")]
    class FileEncodingOptions : FileOperationOptions
    {
    }
}
