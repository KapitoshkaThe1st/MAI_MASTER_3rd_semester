using CommandLine;

namespace Rijndael.Options
{
    [Verb("file-decode", HelpText = "Decodes file encoded with DES algorithm")]
    class FileDecodingOptions : FileOperationOptions
    {
    }
}
