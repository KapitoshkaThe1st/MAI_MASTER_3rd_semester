using System;
using System.Linq;

using CommandLine;

using Rijndael.Options;

namespace Rijndael
{
    class Application
    {
        private static byte[] ParseKey(string keyHex)
        {
            byte[] key;
            try
            {
                key = Convert.FromHexString(keyHex);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"invalid key: {ex.Message}");
            }

            ThrowIfInvalidKey(key);

            return key;
        }

        private static void ThrowIfInvalidKey(byte[] key)
        {
            int keyLengthInBits = key.Length * 8; 
            if (keyLengthInBits != 128 && keyLengthInBits != 192 && keyLengthInBits != 256) // ключ не подходит по длине
            {
                throw new ArgumentException("invalid key: key must be 128-bit, 192-bit or 256-bit length");
            }
        }

        private static Rijndael.BlockLength ParseBlockLength(string blockLengthString)
        {
            return blockLengthString switch
            {
                "128" => Rijndael.BlockLength.Bit128,
                "192" => Rijndael.BlockLength.Bit192,
                "256" => Rijndael.BlockLength.Bit256,
                _ => throw new ArgumentException($"invalid block length: {blockLengthString}")
            };
        }

        private static void ThrowIfInvalidKey(ulong key)
        {
            if (key > 0xFFFFFFFFFFFFFF) // больше 56 бит ключ
            {
                throw new ArgumentException("invalid key: key must be 56-bit length");
            }
        }

        private static void StringEncoding(StringEncodingOptions options)
        {
            byte[] key = ParseKey(options.Key);
            Rijndael.BlockLength blockLength = ParseBlockLength(options.BlockLength);

            var rijndael = new Rijndael(key, blockLength);
            var bytes = rijndael.EncodeString(options.String);

            foreach (var b in bytes)
            {
                Console.Write(Convert.ToString(b, 16).PadLeft(2, '0'));
            }
            Console.WriteLine();
        }

        private static void StringDecoding(StringDecodingOptions options)
        {
            if (options.Cipher.Length % 16 != 0)
            {
                throw new ArgumentException("invalid cipher format: cipher's length must be multiple of 8 byte");
            }

            byte[] bytes;
            try
            {
                bytes = Convert.FromHexString(options.Cipher);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"invalid cipher format: {ex.Message}");
            }

            byte[] key = ParseKey(options.Key);
            Rijndael.BlockLength blockLength = ParseBlockLength(options.BlockLength);

            var rijndael = new Rijndael(key, blockLength);
            string decodedString = rijndael.DecodeString(bytes);

            Console.WriteLine(decodedString);
        }

        private static void FileEncoding(FileEncodingOptions options)
        {
            byte[] key = ParseKey(options.Key);
            Rijndael.BlockLength blockLength = ParseBlockLength(options.BlockLength);

            var rijndael = new Rijndael(key, blockLength);

            try
            {
                rijndael.EncodeFile(options.InputFilePath, options.OutputFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"file encoding error: {ex.Message}");
            }
        }

        private static void FileDecoding(FileDecodingOptions options)
        {
            byte[] key = ParseKey(options.Key);
            Rijndael.BlockLength blockLength = ParseBlockLength(options.BlockLength);

            var rijndael = new Rijndael(key, blockLength);

            try
            {
                rijndael.DecodeFile(options.InputFilePath, options.OutputFilePath);

            }
            catch (Exception ex)
            {
                throw new Exception($"file decoding error: {ex.Message}");
            }
        }

        public void Process(string[] args)
        {
            var result = Parser.Default.ParseArguments<StringEncodingOptions, StringDecodingOptions, FileEncodingOptions, FileDecodingOptions>(args);

            try
            {
                result
                .WithParsed<StringEncodingOptions>(opts => StringEncoding(opts))
                .WithParsed<StringDecodingOptions>(opts => StringDecoding(opts))
                .WithParsed<FileEncodingOptions>(opts => FileEncoding(opts))
                .WithParsed<FileDecodingOptions>(opts => FileDecoding(opts))
                .WithNotParsed(errors =>
                {
                    Console.Error.WriteLine($"Command line argument parsing errors occurred: {errors.Count()}");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }
    }
}
