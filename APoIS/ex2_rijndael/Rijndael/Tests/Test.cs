using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.IO;

using Rijndael;

namespace Tests
{
    [TestClass]
    public class Test
    {
        static byte[][] keys = new byte[][]
        {
            new byte[]
            {
                0x2b, 0x7e, 0x15, 0x16,
                0x28, 0xae, 0xd2, 0xa6,
                0xab, 0xf7, 0x15, 0x88,
                0x09, 0xcf, 0x4f, 0x3c,
            },
            new byte[]
            {
                0x2b, 0x7e, 0x15, 0x16,
                0x28, 0xae, 0xd2, 0xa6,
                0xab, 0xf7, 0x15, 0x88,
                0x09, 0xcf, 0x4f, 0x3c,
                0xac, 0xbe, 0xda, 0x13,
                0x88, 0x77, 0x66, 0x55,
            },
            new byte[]
            {
                0x2b, 0x7e, 0x15, 0x16,
                0x28, 0xae, 0xd2, 0xa6,
                0xab, 0xf7, 0x15, 0x88,
                0x09, 0xcf, 0x4f, 0x3c,
                0xac, 0xbe, 0xda, 0x13,
                0x88, 0x77, 0x66, 0x55,
                0xac, 0xbe, 0xda, 0x13,
                0x00, 0x11, 0xff, 0xee,
            }
        };

        static Rijndael.Rijndael.BlockLength[] blockLengthes = new Rijndael.Rijndael.BlockLength[]
        {
            Rijndael.Rijndael.BlockLength.Bit128,
            Rijndael.Rijndael.BlockLength.Bit192,
            Rijndael.Rijndael.BlockLength.Bit256
        };

        static (uint[] block, Rijndael.Rijndael.BlockLength blockLength)[] blocks = new (uint[] block, Rijndael.Rijndael.BlockLength blockLength)[]
        {
            (new uint[]
            {
                0x03020100,
                0x07060504,
                0x0b0a0908,
                0x0f0e0d0c,
            }, Rijndael.Rijndael.BlockLength.Bit128),
            (new uint[]
            {
                0x03020100,
                0x07060504,
                0x0b0a0908,
                0x0f0e0d0c,
                0x13121110,
                0x17161514,
            }, Rijndael.Rijndael.BlockLength.Bit192),
            (new uint[]
            {
                0x03020100,
                0x07060504,
                0x0b0a0908,
                0x0f0e0d0c,
                0x13121110,
                0x17161514,
                0x1b1a1918,
                0x1f1e1d1c
            }, Rijndael.Rijndael.BlockLength.Bit256)
        };

        private static uint[][] _ivs = new uint[][]
        {
            new uint[]
            {
                0x01234567U,
                0x89abcdefU,
                0x01234567U,
                0x89abcdefU,
            },
            new uint[]
            {
                0x01234567U,
                0x89abcdefU,
                0x01234567U,
                0x89abcdefU,
                0x01234567U,
                0x89abcdefU
            },
            new uint[]
            {
                0x01234567U,
                0x89abcdefU,
                0x01234567U,
                0x89abcdefU,
                0x01234567U,
                0x89abcdefU,
                0x01234567U,
                0x89abcdefU
            },
        };

        static void GenerateFile(string filePath, int size)
        {
            byte[] data = new byte[size];
            Random rng = new Random();
            rng.NextBytes(data);
            File.WriteAllBytes(filePath, data);
        }

        static bool CompareFiles(string filePath1, string filePath2)
        {
            return Enumerable.SequenceEqual(File.ReadAllBytes(filePath1), File.ReadAllBytes(filePath2));
        }

        static int IVIndexByBlockLength(Rijndael.Rijndael.BlockLength blockLength)
        {
            return blockLength switch
            {
                Rijndael.Rijndael.BlockLength.Bit128 => 0,
                Rijndael.Rijndael.BlockLength.Bit192 => 1,
                Rijndael.Rijndael.BlockLength.Bit256 => 2,
                _ => -1
            };
        }

        static bool TestFileEncodingDecoding(string filePath)
        {
            string encodedFilePath = filePath + ".rij";
            string decodedFilePath = "Decoded" + filePath;

            foreach (var key in keys)
            {
                foreach (var blockLength in blockLengthes)
                {
                    Console.WriteLine($"key length: {key.Length * 8}, block length: {blockLength}");

                    var iv = _ivs[IVIndexByBlockLength(blockLength)];

                    Rijndael.Rijndael[] rjs = new Rijndael.Rijndael[]
                    {
                        new Rijndael.Rijndael(key, blockLength),
                        new CBCRijndael(iv, key, blockLength),
                        new CFBRijndael(iv, key, blockLength),
                        new OFBRijndael(iv, key, blockLength),
                    };

                    foreach (var rj in rjs)
                    {
                        rj.EncodeFile(filePath, encodedFilePath);
                        rj.DecodeFile(encodedFilePath, decodedFilePath);

                        if (!CompareFiles(filePath, decodedFilePath))
                        {
                            Console.WriteLine($"files are not equal: filePath: {filePath}, decodedFilePath: {decodedFilePath}");
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        static bool TestFileEncodingDecoding(int fileSize)
        {
            string filePath = $"file.bin";
            GenerateFile(filePath, fileSize);

            string encodedFilePath = filePath + ".rij";
            string decodedFilePath = "Decoded" + filePath;

            foreach (var key in keys)
            {
                foreach (var blockLength in blockLengthes)
                {
                    Console.WriteLine($"file size: {fileSize}, key length: {key.Length * 8}, block length: {blockLength}");

                    var iv = _ivs[IVIndexByBlockLength(blockLength)];

                    Rijndael.Rijndael[] rjs = new Rijndael.Rijndael[]
                    {
                        new Rijndael.Rijndael(key, blockLength),
                        new CBCRijndael(iv, key, blockLength),
                        new CFBRijndael(iv, key, blockLength),
                        new OFBRijndael(iv, key, blockLength),
                    };

                    foreach (var rj in rjs)
                    {
                        rj.EncodeFile(filePath, encodedFilePath);
                        rj.DecodeFile(encodedFilePath, decodedFilePath);

                        if (!CompareFiles(filePath, decodedFilePath))
                        {
                            Console.WriteLine($"files are not equal: filePath: {filePath}, decodedFilePath: {decodedFilePath}");
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static void CreateEmptyFile(string filePath)
        {
            using (File.Create(filePath)) ;
        }

        [TestMethod]
        public void TestOnEmptyFile()
        {
            string emptyFilePath = "empty.txt";
            CreateEmptyFile(emptyFilePath);
            Assert.IsTrue(TestFileEncodingDecoding(emptyFilePath));
        }

        [TestMethod]
        public void TestOnLargeFiles()
        {
            for (int i = 1; i < 5; ++i)
            {
                int fileSize = i * 1024 * 1024;
                Assert.IsTrue(TestFileEncodingDecoding(fileSize));
            }
        }

        [TestMethod]
        public void TestBlockEncoding()
        {
            foreach (var key in keys)
            {
                foreach (var (block, blockLength) in blocks)
                {
                    Rijndael.Rijndael rj = new Rijndael.Rijndael(key, blockLength);

                    var blockCopy = (uint[])block.Clone();

                    rj.EncodeBlock(blockCopy);
                    rj.DecodeBlock(blockCopy);

                    Assert.IsTrue(Enumerable.SequenceEqual(blockCopy, block));
                }
            }
        }
    }
}
