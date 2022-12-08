using System;
using System.Linq;

using CommandLine;

using Rijndael.Options;

namespace Rijndael
{
    enum BlockChainingMode
    {
        ECB,
        CBC,
        CFB,
        OFB
    }

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

        private static BlockChainingMode ParseBlockChainingMode(string blockChainingModeString)
        {
            try
            {
                return Enum.Parse<BlockChainingMode>(blockChainingModeString);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"unknown block chaining mode: {blockChainingModeString}");
            }
        }

        private static uint[] ParseIV(string ivString)
        {
            try
            {
                var bytes = Convert.FromHexString(ivString);

                if(bytes.Length % sizeof(uint) != 0)
                {
                    throw new ArgumentException($"invalid initial vector: initial vector length must be multiple of {sizeof(uint)}");
                }

                int size = bytes.Length / sizeof(uint);

                uint[] result = new uint[size];

                for(int i = 0; i < bytes.Length; i += 4)
                {
                    result[i / 4] = WordOperations.MakeWord(bytes[i], bytes[i + 1], bytes[i + 2], bytes[i + 3]);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"invalid initial vector: {ex.Message}");
            }
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
        private static Rijndael EncryptorFactory(BaseOptions options)
        {
            byte[] key = ParseKey(options.Key);
            Rijndael.BlockLength blockLength = ParseBlockLength(options.BlockLength);

            BlockChainingMode blockChainingMode = ParseBlockChainingMode(options.Mode);

            uint[] iv = null;
            if (blockChainingMode != BlockChainingMode.ECB)
            {
                if(options.IV == BaseOptions.DefaultIVString)
                {
                    throw new ArgumentException($"block chaining mode error: block chaining mode {blockChainingMode} requires initial vector");
                }

                int blockLengthInBits = (int)blockLength;
                if (options.IV.Length / 2 * 8 != (int)blockLength)
                {
                    throw new ArgumentException($"invalid initial vector: initial vector must be {blockLengthInBits}");
                }

                iv = ParseIV(options.IV);
            }

            return blockChainingMode switch
            {
                BlockChainingMode.ECB => new Rijndael(key, blockLength),
                BlockChainingMode.CBC => new CBCRijndael(iv, key, blockLength),
                BlockChainingMode.CFB => new CFBRijndael(iv, key, blockLength),
                BlockChainingMode.OFB => new OFBRijndael(iv, key, blockLength),
                _ => throw new ArgumentException($"Unknown block chaining mode '{blockChainingMode}'")
            };
        }

        private static void StringEncoding(StringEncodingOptions options)
        {
            var rijndael = EncryptorFactory(options);
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

            var rijndael = EncryptorFactory(options);
            string decodedString = rijndael.DecodeString(bytes);

            Console.WriteLine(decodedString);
        }

        private static void FileEncoding(FileEncodingOptions options)
        {
            var rijndael = EncryptorFactory(options);

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
            var rijndael = EncryptorFactory(options);

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
