using CommandLine;

namespace Rijndael.Options
{
    [Verb("file-decode", HelpText = "Decodes file encoded with Rijndael algorithm")]
    class FileDecodingOptions : FileOperationOptions
    {
    }
}
