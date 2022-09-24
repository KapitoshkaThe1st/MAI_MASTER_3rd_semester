using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using CryptoTask1;
using CryptoTask1.CommandLineArgsParsing;

namespace TestProject
{
    [TestClass]
    public class UnitTest1
    {
        private const int _bitSize = sizeof(uint) * 8;

        [TestMethod]
        public void Test_ParseUintFromBinaryRepresentation()
        {
            // ������ ������� ���������� ��-�� ������ ������
            Assert.ThrowsException<ArgumentParsingException>(() =>
            {
                BinaryParser.ParseUint("");
            });

            // ������ ������� ���������� ��-�� �� ����������� �������
            Assert.ThrowsException<ArgumentParsingException>(() =>
            {
                BinaryParser.ParseUint("a");
            });

            // ������ ������� ���������� ��-�� �� ����������� �������, ���� ���� �� � ����� ������� "����", ��� ���� � ���� uint
            Assert.ThrowsException<ArgumentParsingException>(() =>
            {
                BinaryParser.ParseUint("a00000000000000000000000000000000");
            });

            // ������ ������� ���������� ��-�� ��������, �� ������������ � ��� uint
            Assert.ThrowsException<ArgumentParsingException>(() =>
            {
                BinaryParser.ParseUint("100000000000000000000000000000000");
            });

            // ������ ����������, ���� ���� � ������ ������ ��������, ��� ����� � ���� uint, ���� �������� ����� ��������� � ���
            Assert.AreEqual(BinaryParser.ParseUint("000000000000000000000000000000000000000000000000"), (uint)0);
            Assert.AreEqual(BinaryParser.ParseUint("00000000000011111111111111111111111111111111"), uint.MaxValue);

            // ������ ���������� ����� 32 �������
            Assert.AreEqual(BinaryParser.ParseUint("00000000000000000000000000000000"), (uint)0);
            Assert.AreEqual(BinaryParser.ParseUint("00000000000000000000000000000001"), (uint)1);
            Assert.AreEqual(BinaryParser.ParseUint("11111111111111111111111111111111"), uint.MaxValue);

            // ������ ���������� � ������ ��������, ������� ��� ����� ��� � ���� uint
            Assert.AreEqual(BinaryParser.ParseUint("0"), (uint)0);
            Assert.AreEqual(BinaryParser.ParseUint("1"), (uint)1);
        }

        [TestMethod]
        public void Test_BitOperationsGetBit()
        {
            uint num = BinaryParser.ParseUint("01010101010101010101010101010101");

            uint[] bits = new uint[] { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 };

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                BitOperations.GetBit(num, -1);
            });

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                BitOperations.GetBit(num, 32);
            });

            for (int i = 0; i < _bitSize; ++i)
            {
                Assert.AreEqual(BitOperations.GetBit(num, i), bits[i]);
            }
        }

        [TestMethod]
        public void Test_BitOperationsTest()
        {
            uint num = BinaryParser.ParseUint("01010101010101010101010101010101");

            bool[] bits = (new uint[] { 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0 })
                .Select(x => x == 1)
                .ToArray();

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                BitOperations.Test(num, -1);
            });

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                BitOperations.Test(num, 32);
            });

            for (int i = 0; i < _bitSize; ++i)
            {
                Assert.AreEqual(BitOperations.Test(num, i), bits[i]);
            }
        }

        [TestMethod]
        public void Test_BitOperationsSetBit()
        {
            uint num = BinaryParser.ParseUint("00000000000000000000000000000000");

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                BitOperations.SetBit(num, -1);
            });

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                BitOperations.SetBit(num, 32);
            });

            uint prevNum = num;
            for (int i = 0; i < _bitSize; ++i)
            {
                num = BitOperations.SetBit(num, i);

                // ����� ��� ����������
                Assert.IsTrue(BitOperations.Test(num, i));

                num = BitOperations.SetBit(num, i);

                // �������� ����������� � ��� �������������� ����, �� ������ ��� ��������
                Assert.IsTrue(BitOperations.Test(num, i));

                // ��������� �� ����������
                for (int j = 0; j < _bitSize; ++j)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    Assert.AreEqual(BitOperations.Test(num, j), BitOperations.Test(prevNum, j));
                }
                prevNum = num;
            }
        }

        [TestMethod]
        public void Test_BitOperationsResetBit()
        {
            uint num = BinaryParser.ParseUint("11111111111111111111111111111111");

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                BitOperations.ResetBit(num, -1);
            });

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                BitOperations.ResetBit(num, 32);
            });

            uint prevNum = num;
            for (int i = 0; i < _bitSize; ++i)
            {
                num = BitOperations.ResetBit(num, i);

                // ����� ��� ����
                Assert.IsFalse(BitOperations.Test(num, i));

                num = BitOperations.ResetBit(num, i);

                // �������� ����������� � ��� ������� ����, �� ������ ��� ��������
                Assert.IsFalse(BitOperations.Test(num, i));

                // ��������� �� ����������
                for (int j = 0; j < _bitSize; ++j)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    Assert.AreEqual(BitOperations.Test(num, j), BitOperations.Test(prevNum, j));
                }
                prevNum = num;
            }
        }

        [TestMethod]
        public void Test_BitOperationsSwapBits()
        {
            uint num = BinaryParser.ParseUint("01010101010101010101010101010101");
            
            for(int i = 0; i < _bitSize; ++i)
            {
                for (int j = 0; j < _bitSize; ++j)
                {
                    uint swappedNum = BitOperations.SwapBits(num, i, j);

                    uint numIthBit = BitOperations.GetBit(num, i);
                    uint numJthBit = BitOperations.GetBit(num, j);

                    // ���� i-� � j-� ���� � �������� ����� �� �����, ���������, ��� ����� �����, ��� �������� �������
                    if (numIthBit != numJthBit)
                    {
                        Assert.AreEqual(BitOperations.GetBit(swappedNum, i), numJthBit);
                        Assert.AreEqual(BitOperations.GetBit(swappedNum, j), numIthBit);
                    }
                    
                    // � ��� ��������� �� ����������
                    for(int k = 0; k < _bitSize; ++k)
                    {
                        if(k == i || k == j)
                        {
                            continue;
                        }

                        Assert.AreEqual(BitOperations.GetBit(num, k), BitOperations.GetBit(swappedNum, k));
                    }
                }
            }
        }

        [TestMethod]
        public void Test_BitOperationsClearLeastSignificantBits()
        {
            uint num = BinaryParser.ParseUint("11111111111111111111111111111111");

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                BitOperations.ClearLeastSignificantBits(num, -1);
            });

            Assert.AreEqual(BitOperations.ClearLeastSignificantBits(num, 32), (uint)0);

            for (int i = 0; i < _bitSize; ++i)
            {
                uint result = num;

                // ���������� �������� ��� � ����� ���� ����������� ������ ����������
                for (int k = 0; k < 2; ++k)
                {
                    result = BitOperations.ClearLeastSignificantBits(result, i);

                    // ������ i ����� ����������
                    for (int j = 0; j < i; ++j)
                    {
                        Assert.IsFalse(BitOperations.Test(result, j));
                    }

                    // ������ ��������� ��� ��� 1
                    for (int j = i; j < _bitSize; ++j)
                    {
                        Assert.IsTrue(BitOperations.Test(result, j));
                    }
                }
            }
        }

        [TestMethod]
        public void Test_BitOperationsClearMostSignificantBits()
        {
            uint num = BinaryParser.ParseUint("11111111111111111111111111111111");

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                BitOperations.ClearMostSignificantBits(num, -1);
            });

            Assert.AreEqual(BitOperations.ClearLeastSignificantBits(num, 32), (uint)0);

            for (int i = 0; i < _bitSize; ++i)
            {
                uint result = num;

                // ���������� �������� ��� � ����� ���� ����������� ������ ����������
                for (int k = 0; k < 2; ++k)
                {
                    result = BitOperations.ClearMostSignificantBits(result, i);

                    int bound = _bitSize - i;
                    for (int j = 0; j < bound; ++j)
                    {
                        Assert.IsTrue(BitOperations.Test(result, j));
                    }

                    for (int j = bound; j < _bitSize; ++j)
                    {
                        Assert.IsFalse(BitOperations.Test(result, j));
                    }
                }
            }
        }

        [TestMethod]
        public void Test_BitOperationsOuterBits()
        {
            uint num = BinaryParser.ParseUint("11111111111111111111111111111111");

            var testCases = new (int count, uint answer)[] {
                ( 16, BinaryParser.ParseUint("11111111111111111111111111111111")),
                ( 15, BinaryParser.ParseUint("00111111111111111111111111111111")),
                ( 14, BinaryParser.ParseUint("00001111111111111111111111111111")),
                ( 13, BinaryParser.ParseUint("00000011111111111111111111111111")),
                ( 12, BinaryParser.ParseUint("00000000111111111111111111111111")),
                ( 11, BinaryParser.ParseUint("00000000001111111111111111111111")),
                ( 10, BinaryParser.ParseUint("00000000000011111111111111111111")),
                ( 9,  BinaryParser.ParseUint("00000000000000111111111111111111")),
                ( 8,  BinaryParser.ParseUint("00000000000000001111111111111111")),
                ( 7,  BinaryParser.ParseUint("00000000000000000011111111111111")),
                ( 6,  BinaryParser.ParseUint("00000000000000000000111111111111")),
                ( 5,  BinaryParser.ParseUint("00000000000000000000001111111111")),
                ( 4,  BinaryParser.ParseUint("00000000000000000000000011111111")),
                ( 3,  BinaryParser.ParseUint("00000000000000000000000000111111")),
                ( 2,  BinaryParser.ParseUint("00000000000000000000000000001111")),
                ( 1,  BinaryParser.ParseUint("00000000000000000000000000000011")),
                ( 0,  BinaryParser.ParseUint("00000000000000000000000000000000")),
            };

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                BitOperations.OuterBits(num, -1);
            });

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                BitOperations.OuterBits(num, _bitSize / 2 + 1);
            });

            foreach (var (count, answer) in testCases)
            {
                Assert.AreEqual(BitOperations.OuterBits(num, count), answer);
            }
        }

        [TestMethod]
        public void Test_BitOperationsInnerBits()
        {
            uint num = BinaryParser.ParseUint("11111111111111111111111111111111");

            var testCases = new (int count, uint answer)[] {
                ( 0, BinaryParser.ParseUint("11111111111111111111111111111111")),
                ( 1, BinaryParser.ParseUint("00111111111111111111111111111111")),
                ( 2, BinaryParser.ParseUint("00001111111111111111111111111111")),
                ( 3, BinaryParser.ParseUint("00000011111111111111111111111111")),
                ( 4, BinaryParser.ParseUint("00000000111111111111111111111111")),
                ( 5, BinaryParser.ParseUint("00000000001111111111111111111111")),
                ( 6, BinaryParser.ParseUint("00000000000011111111111111111111")),
                ( 7,  BinaryParser.ParseUint("00000000000000111111111111111111")),
                ( 8,  BinaryParser.ParseUint("00000000000000001111111111111111")),
                ( 9,  BinaryParser.ParseUint("00000000000000000011111111111111")),
                ( 10,  BinaryParser.ParseUint("00000000000000000000111111111111")),
                ( 11,  BinaryParser.ParseUint("00000000000000000000001111111111")),
                ( 12,  BinaryParser.ParseUint("00000000000000000000000011111111")),
                ( 13,  BinaryParser.ParseUint("00000000000000000000000000111111")),
                ( 14,  BinaryParser.ParseUint("00000000000000000000000000001111")),
                ( 15,  BinaryParser.ParseUint("00000000000000000000000000000011")),
                ( 16,  BinaryParser.ParseUint("00000000000000000000000000000000")),
            };

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                BitOperations.InnerBits(num, -1);
            });

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                BitOperations.InnerBits(num, _bitSize / 2 + 1);
            });

            foreach (var (count, answer) in testCases)
            {
                Assert.AreEqual(BitOperations.InnerBits(num, count), answer);
            }
        }

        [TestMethod]
        public void Test_BitOperationsParity()
        {
            var testCases = new (uint num, uint answer)[]
            {
                (BinaryParser.ParseUint("11111111111111111111111111111111"), 0),
                (BinaryParser.ParseUint("00000000000000000000000000000000"), 0),
                (BinaryParser.ParseUint("00000000000000000000000000000001"), 1),
                (BinaryParser.ParseUint("10000000000000000000000000000000"), 1),
                (BinaryParser.ParseUint("10101010101010101010101010101010"), 0),
                (BinaryParser.ParseUint("01010101010101010101010101010101"), 0),
                (BinaryParser.ParseUint("1010101010101010101010101010101"), 0),
                (BinaryParser.ParseUint("0101010101010101010101010101010"), 1),
                (BinaryParser.ParseUint("0000000000000000000000000000111"), 1),
                (BinaryParser.ParseUint("0000000000000000000000000000011"), 0)
            };

            foreach (var (num, answer) in testCases)
            {
                Assert.AreEqual(BitOperations.Parity(num), answer);
            }
        }

        [TestMethod]
        public void Test_BitOperationsRotateLeft()
        {
            uint num = BinaryParser.ParseUint("11111111111111111111111111111110");

            var testCases = new (int count, uint answer)[] {
                ( 0,   BinaryParser.ParseUint("11111111111111111111111111111110")),
                ( 1,   BinaryParser.ParseUint("11111111111111111111111111111101")),
                ( 2,   BinaryParser.ParseUint("11111111111111111111111111111011")),
                ( 3,   BinaryParser.ParseUint("11111111111111111111111111110111")),
                ( 4,   BinaryParser.ParseUint("11111111111111111111111111101111")),
                ( 5,   BinaryParser.ParseUint("11111111111111111111111111011111")),
                ( 6,   BinaryParser.ParseUint("11111111111111111111111110111111")),
                ( 7,   BinaryParser.ParseUint("11111111111111111111111101111111")),
                ( 8,   BinaryParser.ParseUint("11111111111111111111111011111111")),
                ( 9,   BinaryParser.ParseUint("11111111111111111111110111111111")),
                ( 10,  BinaryParser.ParseUint("11111111111111111111101111111111")),
                ( 11,  BinaryParser.ParseUint("11111111111111111111011111111111")),
                ( 12,  BinaryParser.ParseUint("11111111111111111110111111111111")),
                ( 13,  BinaryParser.ParseUint("11111111111111111101111111111111")),
                ( 14,  BinaryParser.ParseUint("11111111111111111011111111111111")),
                ( 15,  BinaryParser.ParseUint("11111111111111110111111111111111")),
                ( 16,  BinaryParser.ParseUint("11111111111111101111111111111111")),
                ( 17,  BinaryParser.ParseUint("11111111111111011111111111111111")),
                ( 18,  BinaryParser.ParseUint("11111111111110111111111111111111")),
                ( 19,  BinaryParser.ParseUint("11111111111101111111111111111111")),
                ( 20,  BinaryParser.ParseUint("11111111111011111111111111111111")),
                ( 21,  BinaryParser.ParseUint("11111111110111111111111111111111")),
                ( 22,  BinaryParser.ParseUint("11111111101111111111111111111111")),
                ( 23,  BinaryParser.ParseUint("11111111011111111111111111111111")),
                ( 24,  BinaryParser.ParseUint("11111110111111111111111111111111")),
                ( 25,  BinaryParser.ParseUint("11111101111111111111111111111111")),
                ( 26,  BinaryParser.ParseUint("11111011111111111111111111111111")),
                ( 27,  BinaryParser.ParseUint("11110111111111111111111111111111")),
                ( 28,  BinaryParser.ParseUint("11101111111111111111111111111111")),
                ( 29,  BinaryParser.ParseUint("11011111111111111111111111111111")),
                ( 30,  BinaryParser.ParseUint("10111111111111111111111111111111")),
                ( 31,  BinaryParser.ParseUint("01111111111111111111111111111111")),
                ( 32,  BinaryParser.ParseUint("11111111111111111111111111111110")),
            };

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                BitOperations.RotateRight(num, -1);
            });

            foreach (var (count, answer) in testCases)
            {
                Assert.AreEqual(BitOperations.RotateLeft(num, count), answer);
            }
        }

        [TestMethod]
        public void Test_BitOperationsRotateRight()
        {
            uint num = BinaryParser.ParseUint("11111111111111111111111111111110");

            var testCases = new (int count, uint answer)[] {
                ( 32, BinaryParser.ParseUint("11111111111111111111111111111110")),
                ( 31, BinaryParser.ParseUint("11111111111111111111111111111101")),
                ( 30, BinaryParser.ParseUint("11111111111111111111111111111011")),
                ( 29, BinaryParser.ParseUint("11111111111111111111111111110111")),
                ( 28, BinaryParser.ParseUint("11111111111111111111111111101111")),
                ( 27, BinaryParser.ParseUint("11111111111111111111111111011111")),
                ( 26, BinaryParser.ParseUint("11111111111111111111111110111111")),
                ( 25, BinaryParser.ParseUint("11111111111111111111111101111111")),
                ( 24, BinaryParser.ParseUint("11111111111111111111111011111111")),
                ( 23, BinaryParser.ParseUint("11111111111111111111110111111111")),
                ( 22, BinaryParser.ParseUint("11111111111111111111101111111111")),
                ( 21, BinaryParser.ParseUint("11111111111111111111011111111111")),
                ( 20, BinaryParser.ParseUint("11111111111111111110111111111111")),
                ( 19, BinaryParser.ParseUint("11111111111111111101111111111111")),
                ( 18, BinaryParser.ParseUint("11111111111111111011111111111111")),
                ( 17, BinaryParser.ParseUint("11111111111111110111111111111111")),
                ( 16, BinaryParser.ParseUint("11111111111111101111111111111111")),
                ( 15, BinaryParser.ParseUint("11111111111111011111111111111111")),
                ( 14, BinaryParser.ParseUint("11111111111110111111111111111111")),
                ( 13, BinaryParser.ParseUint("11111111111101111111111111111111")),
                ( 12, BinaryParser.ParseUint("11111111111011111111111111111111")),
                ( 11, BinaryParser.ParseUint("11111111110111111111111111111111")),
                ( 10, BinaryParser.ParseUint("11111111101111111111111111111111")),
                ( 9,  BinaryParser.ParseUint("11111111011111111111111111111111")),
                ( 8,  BinaryParser.ParseUint("11111110111111111111111111111111")),
                ( 7,  BinaryParser.ParseUint("11111101111111111111111111111111")),
                ( 6,  BinaryParser.ParseUint("11111011111111111111111111111111")),
                ( 5,  BinaryParser.ParseUint("11110111111111111111111111111111")),
                ( 4,  BinaryParser.ParseUint("11101111111111111111111111111111")),
                ( 3,  BinaryParser.ParseUint("11011111111111111111111111111111")),
                ( 2,  BinaryParser.ParseUint("10111111111111111111111111111111")),
                ( 1,  BinaryParser.ParseUint("01111111111111111111111111111111")),
                ( 0,  BinaryParser.ParseUint("11111111111111111111111111111110")),
            };

            Assert.ThrowsException<IndexOutOfRangeException>(() =>
            {
                BitOperations.RotateRight(num, -1);
            });

            foreach (var (count, answer) in testCases)
            {
                Assert.AreEqual(BitOperations.RotateRight(num, count), answer);
            }
        }
    }
}
