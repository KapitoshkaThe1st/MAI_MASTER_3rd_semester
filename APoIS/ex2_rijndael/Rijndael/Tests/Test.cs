using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.IO;

using RijndaelAlgo = Rijndael.Rijndael;

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

        static RijndaelAlgo.BlockLength[] blockLengthes = new RijndaelAlgo.BlockLength[]
        {
            RijndaelAlgo.BlockLength.Bit128,
            RijndaelAlgo.BlockLength.Bit192,
            RijndaelAlgo.BlockLength.Bit256
        };

        static (uint[] block, RijndaelAlgo.BlockLength blockLength)[] blocks = new (uint[] block, RijndaelAlgo.BlockLength blockLength)[]
        {
            (new uint[]
            {
                0x03020100,
                0x07060504,
                0x0b0a0908,
                0x0f0e0d0c,
            }, RijndaelAlgo.BlockLength.Bit128),
            (new uint[]
            {
                0x03020100,
                0x07060504,
                0x0b0a0908,
                0x0f0e0d0c,
                0x13121110,
                0x17161514,
            }, RijndaelAlgo.BlockLength.Bit192),
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
            }, RijndaelAlgo.BlockLength.Bit256)
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

        static bool TestFileEncodingDecoding(string filePath)
        {
            string encodedFilePath = filePath + ".rij";
            string decodedFilePath = "Decoded" + filePath;

            foreach (var key in keys)
            {
                foreach (var blockLength in blockLengthes)
                {
                    Console.WriteLine($"key length: {key.Length * 8}, block length: {blockLength}");
                    RijndaelAlgo rj = new RijndaelAlgo(key, blockLength);

                    rj.EncodeFile(filePath, encodedFilePath);
                    rj.DecodeFile(encodedFilePath, decodedFilePath);

                    if (!CompareFiles(filePath, decodedFilePath))
                    {
                        Console.WriteLine($"files are not equal: filePath: {filePath}, decodedFilePath: {decodedFilePath}");
                        return false;
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
                    RijndaelAlgo rj = new RijndaelAlgo(key, blockLength);

                    rj.EncodeFile(filePath, encodedFilePath);
                    rj.DecodeFile(encodedFilePath, decodedFilePath);

                    if (!CompareFiles(filePath, decodedFilePath))
                    {
                        Console.WriteLine($"files are not equal: filePath: {filePath}, decodedFilePath: {decodedFilePath}");
                        return false;
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
                    RijndaelAlgo rj = new RijndaelAlgo(key, blockLength);

                    var blockCopy = (uint[])block.Clone();

                    rj.EncodeBlock(blockCopy);
                    rj.DecodeBlock(blockCopy);

                    Assert.IsTrue(Enumerable.SequenceEqual(blockCopy, block));
                }
            }
        }
    }
}
