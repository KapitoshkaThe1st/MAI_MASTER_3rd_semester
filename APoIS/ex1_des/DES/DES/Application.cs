using System;
using System.Linq;
using System.IO;

using CommandLine;

using DES.Options;

namespace DES
{
    class Application
    {
        // расширение 56-битного ключа до битного битами контроля четности (в каждом байте окажется нечетное количество единиц)
        private static ulong KeyExpansion(ulong key56)
        {
            ulong result = 0;
            for (int i = 0; i < 8; ++i)
            {
                byte part = (byte)(key56 & 0b1111111);
                uint bitToAdd = BitOperations.Parity(part) == 0 ? 1U : 0U;
                part = (byte)(part | (bitToAdd << 7));

                result |= ((ulong)part << 8 * i);

                key56 >>= 7;
            }

            return result;
        }

        private static ulong ParseKey(string keyHex)
        {
            ulong key;
            try
            {
                key = Convert.ToUInt64(keyHex, 16);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"invalid key: {ex.Message}");
            }

            ThrowIfInvalidKey(key);

            return key;
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
            ulong key = ParseKey(options.Key);
            key = KeyExpansion(key);

            var des = new DES(key);
            var bytes = des.EncodeString(options.String);

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

            ulong key = ParseKey(options.Key);
            key = KeyExpansion(key);

            var des = new DES(key);
            string decodedString = des.DecodeString(bytes);

            Console.WriteLine(decodedString);
        }

        private const int _fileEncodingChunkSize = 1024 * 1024;
        private const int _blockSizeInBytes = 8;
        private static void FileEncoding(FileEncodingOptions options)
        {
            ulong key = ParseKey(options.Key);
            key = KeyExpansion(key);

            var des = new DES(key);

            if (!File.Exists(options.InputFilePath))
            {
                throw new FileNotFoundException($"file encoding error: input file '{options.InputFilePath}' does not exist");
            }

            long filesize = (new FileInfo(options.InputFilePath)).Length;

            try
            {
                using (BinaryReader br = new BinaryReader(new FileStream(options.InputFilePath, FileMode.Open)))
                using (BinaryWriter bw = new BinaryWriter(new FileStream(options.OutputFilePath, FileMode.Create)))
                {
                    bool firstIteration = true;
                    while (true)
                    {
                        if (firstIteration)
                        {
                            var filesizeBytes = des.Encode(BitConverter.GetBytes(filesize));
                            bw.Write(filesizeBytes);
                        }

                        byte[] bytes = br.ReadBytes(_fileEncodingChunkSize);

                        if(bytes.Length == 0)
                        {
                            break;
                        }

                        if (bytes.Length % _blockSizeInBytes == 0)
                        {
                            var encodedBytes = des.Encode(bytes);
                            bw.Write(encodedBytes);
                        }
                        else
                        {
                            int remainder = bytes.Length % _blockSizeInBytes;
                            int paddedLength = bytes.Length + (remainder == 0 ? 0 : _blockSizeInBytes - remainder);

                            byte[] paddedBytes = new byte[paddedLength];
                            Array.Copy(bytes, paddedBytes, bytes.Length);

                            var encodedBytes = des.Encode(paddedBytes);
                            bw.Write(encodedBytes);

                            break;
                        }
                        firstIteration = false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"file encoding error: {ex.Message}");
            }
        }

        private static void FileDecoding(FileDecodingOptions options)
        {
            if (!File.Exists(options.InputFilePath))
            {
                throw new FileNotFoundException($"file decoding error: input file '{options.InputFilePath}' does not exist");
            }

            long inputFileSize = (new FileInfo(options.InputFilePath)).Length;

            if (inputFileSize < 8)
            {
                throw new ArgumentException("file decoding error: file size must be at least 8 bytes long (information about file size before encoding)");
            }

            if (inputFileSize % _blockSizeInBytes != 0)
            {
                throw new ArgumentException("file decoding error: cipher-file's length must be multiple of 8 byte");
            }

            ulong key = ParseKey(options.Key);
            key = KeyExpansion(key);

            var des = new DES(key);

            try
            {
                using (BinaryReader br = new BinaryReader(new FileStream(options.InputFilePath, FileMode.Open)))
                using (BinaryWriter bw = new BinaryWriter(new FileStream(options.OutputFilePath, FileMode.Create)))
                {
                    bool firstIteration = true;
                    long bytesRemaining = 0;
                    while (true)
                    {
                        byte[] encodedBytes = br.ReadBytes(_fileEncodingChunkSize);

                        var bytes = des.Decode(encodedBytes);
                        int startIndex = 0;
                        if (firstIteration)
                        {
                            var span = new ReadOnlySpan<byte>(bytes, 0, sizeof(long));

                            bytesRemaining = BitConverter.ToInt64(span);
                            startIndex += sizeof(long);
                        }


                        if (bytesRemaining > _fileEncodingChunkSize)
                        {
                            bw.Write(new ReadOnlySpan<byte>(bytes, startIndex, bytes.Length - startIndex));
                        }
                        else
                        {
                            bw.Write(new ReadOnlySpan<byte>(bytes, startIndex, (int)bytesRemaining));
                            break;
                        }

                        bytesRemaining -= encodedBytes.Length - startIndex;
                        firstIteration = false;
                    }
                }
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
