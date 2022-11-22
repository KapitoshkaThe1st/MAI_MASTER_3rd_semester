using CommandLine;

namespace DES.Options
{
    [Verb("file-decode", HelpText = "Decodes file encoded with DES algorithm")]
    class FileDecodingOptions : FileOperationOptions
    {
    }
}
