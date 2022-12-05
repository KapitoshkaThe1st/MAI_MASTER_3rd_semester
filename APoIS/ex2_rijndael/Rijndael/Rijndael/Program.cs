using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using GF_Operations;
using System.IO;

using Rijndael.Options;

namespace Rijndael
{
    public enum BlockLength
    {
        Bit128 = 128,
        Bit192 = 192,
        Bit256 = 256
    }

    public sealed class Rinjdael
    {
        private int _nk;
        private int _nb;
        private int _nr;

        private byte[] _key;

        private uint[] _expandedKey;

        private static int GetNb(BlockLength blockLength)
        {
            return ((int)blockLength / (4 * 8));
        }

        public Rinjdael(byte[] key, BlockLength blockLength = BlockLength.Bit128)
        {
            _nk = key.Length / 4;
            _nb = GetNb(blockLength);
            _nr = GetNr(_nk, _nb);

            _key = key;
            _expandedKey = GenerateExpandedKey(key);
        }

        //private static uint[] _rCon = new uint[] {
        //    0x0000, 0x0001, 0x0002, 0x0004, 0x0008, 0x0010, 0x0020, 0x0040, 0x0080, 0x001B, 0x0036, 0x006C, 0x00D8, 0x00AB, 0x004D
        //};

        private static uint[] _rCon = new uint[] {
            0x0000, 0x0001, 0x0002, 0x0004, 0x0008, 0x0010, 0x0020, 0x0040, 0x0080, 0x001B, 0x0036, 0x006C, 0x00D8, 0x00AB, 0x004D, 0x009A, 0x002F, 0x005E, 0x00BC, 0x0063, 0x00C6, 0x0097, 0x0035, 0x006A, 0x00D4, 0x00B3
        };

        private static int[,] _nrTable = new int[,]
        {
        // nk  4   6   8       nb
            { 10, 12, 14 }, // 4
            { 12, 12, 14 }, // 6
            { 14, 14, 14 }, // 8
        };

        private static int[,] _c = new int[,]
        {
           // c1 c2 c3      nb
            { 1, 2, 3 }, // 4
            { 1, 2, 3 }, // 6
            { 1, 3, 4 }, // 8
        };

        private static uint[] _gfInverse = new uint[] {
            0x00, 0x01, 0x8D, 0xF6, 0xCB, 0x52, 0x7B, 0xD1, 0xE8, 0x4F, 0x29, 0xC0, 0xB0, 0xE1, 0xE5, 0xC7,
            0x74, 0xB4, 0xAA, 0x4B, 0x99, 0x2B, 0x60, 0x5F, 0x58, 0x3F, 0xFD, 0xCC, 0xFF, 0x40, 0xEE, 0xB2,
            0x3A, 0x6E, 0x5A, 0xF1, 0x55, 0x4D, 0xA8, 0xC9, 0xC1, 0x0A, 0x98, 0x15, 0x30, 0x44, 0xA2, 0xC2,
            0x2C, 0x45, 0x92, 0x6C, 0xF3, 0x39, 0x66, 0x42, 0xF2, 0x35, 0x20, 0x6F, 0x77, 0xBB, 0x59, 0x19,
            0x1D, 0xFE, 0x37, 0x67, 0x2D, 0x31, 0xF5, 0x69, 0xA7, 0x64, 0xAB, 0x13, 0x54, 0x25, 0xE9, 0x09,
            0xED, 0x5C, 0x05, 0xCA, 0x4C, 0x24, 0x87, 0xBF, 0x18, 0x3E, 0x22, 0xF0, 0x51, 0xEC, 0x61, 0x17,
            0x16, 0x5E, 0xAF, 0xD3, 0x49, 0xA6, 0x36, 0x43, 0xF4, 0x47, 0x91, 0xDF, 0x33, 0x93, 0x21, 0x3B,
            0x79, 0xB7, 0x97, 0x85, 0x10, 0xB5, 0xBA, 0x3C, 0xB6, 0x70, 0xD0, 0x06, 0xA1, 0xFA, 0x81, 0x82,
            0x83, 0x7E, 0x7F, 0x80, 0x96, 0x73, 0xBE, 0x56, 0x9B, 0x9E, 0x95, 0xD9, 0xF7, 0x02, 0xB9, 0xA4,
            0xDE, 0x6A, 0x32, 0x6D, 0xD8, 0x8A, 0x84, 0x72, 0x2A, 0x14, 0x9F, 0x88, 0xF9, 0xDC, 0x89, 0x9A,
            0xFB, 0x7C, 0x2E, 0xC3, 0x8F, 0xB8, 0x65, 0x48, 0x26, 0xC8, 0x12, 0x4A, 0xCE, 0xE7, 0xD2, 0x62,
            0x0C, 0xE0, 0x1F, 0xEF, 0x11, 0x75, 0x78, 0x71, 0xA5, 0x8E, 0x76, 0x3D, 0xBD, 0xBC, 0x86, 0x57,
            0x0B, 0x28, 0x2F, 0xA3, 0xDA, 0xD4, 0xE4, 0x0F, 0xA9, 0x27, 0x53, 0x04, 0x1B, 0xFC, 0xAC, 0xE6,
            0x7A, 0x07, 0xAE, 0x63, 0xC5, 0xDB, 0xE2, 0xEA, 0x94, 0x8B, 0xC4, 0xD5, 0x9D, 0xF8, 0x90, 0x6B,
            0xB1, 0x0D, 0xD6, 0xEB, 0xC6, 0x0E, 0xCF, 0xAD, 0x08, 0x4E, 0xD7, 0xE3, 0x5D, 0x50, 0x1E, 0xB3,
            0x5B, 0x23, 0x38, 0x34, 0x68, 0x46, 0x03, 0x8C, 0xDD, 0x9C, 0x7D, 0xA0, 0xCD, 0x1A, 0x41, 0x1C
        };

        private static byte[] _sBox = new byte[] {
            0x63, 0x7C, 0x77, 0x7B, 0xF2, 0x6B, 0x6F, 0xC5, 0x30, 0x01, 0x67, 0x2B, 0xFE, 0xD7, 0xAB, 0x76,
            0xCA, 0x82, 0xC9, 0x7D, 0xFA, 0x59, 0x47, 0xF0, 0xAD, 0xD4, 0xA2, 0xAF, 0x9C, 0xA4, 0x72, 0xC0,
            0xB7, 0xFD, 0x93, 0x26, 0x36, 0x3F, 0xF7, 0xCC, 0x34, 0xA5, 0xE5, 0xF1, 0x71, 0xD8, 0x31, 0x15,
            0x04, 0xC7, 0x23, 0xC3, 0x18, 0x96, 0x05, 0x9A, 0x07, 0x12, 0x80, 0xE2, 0xEB, 0x27, 0xB2, 0x75,
            0x09, 0x83, 0x2C, 0x1A, 0x1B, 0x6E, 0x5A, 0xA0, 0x52, 0x3B, 0xD6, 0xB3, 0x29, 0xE3, 0x2F, 0x84,
            0x53, 0xD1, 0x00, 0xED, 0x20, 0xFC, 0xB1, 0x5B, 0x6A, 0xCB, 0xBE, 0x39, 0x4A, 0x4C, 0x58, 0xCF,
            0xD0, 0xEF, 0xAA, 0xFB, 0x43, 0x4D, 0x33, 0x85, 0x45, 0xF9, 0x02, 0x7F, 0x50, 0x3C, 0x9F, 0xA8,
            0x51, 0xA3, 0x40, 0x8F, 0x92, 0x9D, 0x38, 0xF5, 0xBC, 0xB6, 0xDA, 0x21, 0x10, 0xFF, 0xF3, 0xD2,
            0xCD, 0x0C, 0x13, 0xEC, 0x5F, 0x97, 0x44, 0x17, 0xC4, 0xA7, 0x7E, 0x3D, 0x64, 0x5D, 0x19, 0x73,
            0x60, 0x81, 0x4F, 0xDC, 0x22, 0x2A, 0x90, 0x88, 0x46, 0xEE, 0xB8, 0x14, 0xDE, 0x5E, 0x0B, 0xDB,
            0xE0, 0x32, 0x3A, 0x0A, 0x49, 0x06, 0x24, 0x5C, 0xC2, 0xD3, 0xAC, 0x62, 0x91, 0x95, 0xE4, 0x79,
            0xE7, 0xC8, 0x37, 0x6D, 0x8D, 0xD5, 0x4E, 0xA9, 0x6C, 0x56, 0xF4, 0xEA, 0x65, 0x7A, 0xAE, 0x08,
            0xBA, 0x78, 0x25, 0x2E, 0x1C, 0xA6, 0xB4, 0xC6, 0xE8, 0xDD, 0x74, 0x1F, 0x4B, 0xBD, 0x8B, 0x8A,
            0x70, 0x3E, 0xB5, 0x66, 0x48, 0x03, 0xF6, 0x0E, 0x61, 0x35, 0x57, 0xB9, 0x86, 0xC1, 0x1D, 0x9E,
            0xE1, 0xF8, 0x98, 0x11, 0x69, 0xD9, 0x8E, 0x94, 0x9B, 0x1E, 0x87, 0xE9, 0xCE, 0x55, 0x28, 0xDF,
            0x8C, 0xA1, 0x89, 0x0D, 0xBF, 0xE6, 0x42, 0x68, 0x41, 0x99, 0x2D, 0x0F, 0xB0, 0x54, 0xBB, 0x16
        };

        private static byte[] _inverseSBox = new byte[] {
            0x52, 0x09, 0x6A, 0xD5, 0x30, 0x36, 0xA5, 0x38, 0xBF, 0x40, 0xA3, 0x9E, 0x81, 0xF3, 0xD7, 0xFB,
            0x7C, 0xE3, 0x39, 0x82, 0x9B, 0x2F, 0xFF, 0x87, 0x34, 0x8E, 0x43, 0x44, 0xC4, 0xDE, 0xE9, 0xCB,
            0x54, 0x7B, 0x94, 0x32, 0xA6, 0xC2, 0x23, 0x3D, 0xEE, 0x4C, 0x95, 0x0B, 0x42, 0xFA, 0xC3, 0x4E,
            0x08, 0x2E, 0xA1, 0x66, 0x28, 0xD9, 0x24, 0xB2, 0x76, 0x5B, 0xA2, 0x49, 0x6D, 0x8B, 0xD1, 0x25,
            0x72, 0xF8, 0xF6, 0x64, 0x86, 0x68, 0x98, 0x16, 0xD4, 0xA4, 0x5C, 0xCC, 0x5D, 0x65, 0xB6, 0x92,
            0x6C, 0x70, 0x48, 0x50, 0xFD, 0xED, 0xB9, 0xDA, 0x5E, 0x15, 0x46, 0x57, 0xA7, 0x8D, 0x9D, 0x84,
            0x90, 0xD8, 0xAB, 0x00, 0x8C, 0xBC, 0xD3, 0x0A, 0xF7, 0xE4, 0x58, 0x05, 0xB8, 0xB3, 0x45, 0x06,
            0xD0, 0x2C, 0x1E, 0x8F, 0xCA, 0x3F, 0x0F, 0x02, 0xC1, 0xAF, 0xBD, 0x03, 0x01, 0x13, 0x8A, 0x6B,
            0x3A, 0x91, 0x11, 0x41, 0x4F, 0x67, 0xDC, 0xEA, 0x97, 0xF2, 0xCF, 0xCE, 0xF0, 0xB4, 0xE6, 0x73,
            0x96, 0xAC, 0x74, 0x22, 0xE7, 0xAD, 0x35, 0x85, 0xE2, 0xF9, 0x37, 0xE8, 0x1C, 0x75, 0xDF, 0x6E,
            0x47, 0xF1, 0x1A, 0x71, 0x1D, 0x29, 0xC5, 0x89, 0x6F, 0xB7, 0x62, 0x0E, 0xAA, 0x18, 0xBE, 0x1B,
            0xFC, 0x56, 0x3E, 0x4B, 0xC6, 0xD2, 0x79, 0x20, 0x9A, 0xDB, 0xC0, 0xFE, 0x78, 0xCD, 0x5A, 0xF4,
            0x1F, 0xDD, 0xA8, 0x33, 0x88, 0x07, 0xC7, 0x31, 0xB1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xEC, 0x5F,
            0x60, 0x51, 0x7F, 0xA9, 0x19, 0xB5, 0x4A, 0x0D, 0x2D, 0xE5, 0x7A, 0x9F, 0x93, 0xC9, 0x9C, 0xEF,
            0xA0, 0xE0, 0x3B, 0x4D, 0xAE, 0x2A, 0xF5, 0xB0, 0xC8, 0xEB, 0xBB, 0x3C, 0x83, 0x53, 0x99, 0x61,
            0x17, 0x2B, 0x04, 0x7E, 0xBA, 0x77, 0xD6, 0x26, 0xE1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0C, 0x7D
        };

        private byte[] _mixColumnC = new byte[]
        {
            0x2, 0x3, 0x1, 0x1
        };


        private byte[] _inverseMixColumnC = new byte[]
        {
            0xE, 0xB, 0xD, 0x9
        };

        private static uint MakeWord(byte b0, byte b1, byte b2, byte b3)
        {
            return ((((((uint)b3 << 8) | (uint)b2) << 8) | (uint)b1) << 8) | b0; 
        }

        private static byte GetByte(uint word, int i)
        {
            return (byte)((word >> (i * 8)) & 0xFFU);
        }

        private static uint SubByte(uint word)
        {
            return MakeWord(_sBox[GetByte(word, 0)], _sBox[GetByte(word, 1)], _sBox[GetByte(word, 2)], _sBox[GetByte(word, 3)]);
        }

        private static uint InverseSubByte(uint word)
        {
            return MakeWord(_inverseSBox[GetByte(word, 0)], _inverseSBox[GetByte(word, 1)], _inverseSBox[GetByte(word, 2)], _inverseSBox[GetByte(word, 3)]);
        }

        private uint RotByte(uint word)
        {
            return MakeWord(GetByte(word, 1), GetByte(word, 2), GetByte(word, 3), GetByte(word, 0));
        }

        private string ByteToStringHex(byte b)
        {
            return Convert.ToString(b, 16).PadLeft(2, '0');
        } 

        private uint[] GenerateExpandedKey(byte[] key)
        {
            uint[] expandedKey = new uint[(_nr + 1) * _nb];

            for (int i = 0; i < _nk; ++i)
            {
                expandedKey[i] = MakeWord(key[4 * i], key[4 * i + 1], key[4 * i + 2], key[4 * i + 3]);
            }

            if (_nk <= 6)
            {
                GenerateExpandedKeyBelowEndEqual6(expandedKey);
            }
            else
            {
                GenerateExpandedKeyAbove6(expandedKey);
            }

            Console.WriteLine("expanded key:");
            PrintBlock(expandedKey);

            return expandedKey;
        }

        private void GenerateExpandedKeyBelowEndEqual6(uint[] expandedKey)
        {
            for (int i = _nk; i < _nb * (_nr + 1); i++)
            {
                uint temp = expandedKey[i - 1];
                if (i % _nk == 0)
                {
                    temp = SubByte(RotByte(temp)) ^ _rCon[i / _nk];
                }
                expandedKey[i] = expandedKey[i - _nk] ^ temp;
            }
        }

        private void GenerateExpandedKeyAbove6(uint[] expandedKey)
        {
            for (int i = _nk; i < _nb * (_nr + 1); i++)
            {
                uint temp = expandedKey[i - 1];
                if (i % _nk == 0)
                {
                    temp = SubByte(RotByte(temp)) ^ _rCon[i / _nk];
                }
                else if (i % _nk == 4)
                {
                    temp = SubByte(temp);
                }
                expandedKey[i] = expandedKey[i - _nk] ^ temp;
            }
        }

        ReadOnlySpan<uint> GetRoundKey(int roundNumber)
        {
            Console.WriteLine($"used round key {roundNumber}");
            return new ReadOnlySpan<uint>(_expandedKey, roundNumber * _nb, _nb);
        }

        private static int GetTableIndex(int n)
        {
            return (n - 4) >> 2; // (n-4) / 2
        }

        private static int GetNr(int nk, int nb)
        {
            return _nrTable[GetTableIndex(nk), GetTableIndex(nb)]; // _nr[(nk-4) / 2, (nb-4) / 2]
        }

        private int C(int k)
        {
            return _c[GetTableIndex(_nb), k - 1];
        }

        private static void SubBytes(uint[] state)
        {
            for (int i = 0; i < state.Length; ++i)
            {
                state[i] = SubByte(state[i]);
            }
        }

        private static void InverseSubBytes(uint[] state)
        {
            for (int i = 0; i < state.Length; ++i)
            {
                state[i] = InverseSubByte(state[i]);
            }
        }

        private static int Index(int i, int j, int nb)
        {
            return i * nb + j;
        }

        private byte[] _shiftTemp = new byte[8]; // с запасом для работы с блоками длины 256 и для обратной ShiftRow в том числе

        private void ShiftRowAux(uint[] state, Func<int, int> cFunc)
        {
            Span<uint> stateCopy = stackalloc uint[state.Length];
            state.CopyTo(stateCopy);

            for(int i = 0; i < _nb; ++i)
            {
                int b0WordIndex = i;
                int b1WordIndex = (i + cFunc(1)) % _nb;
                int b2WordIndex = (i + cFunc(2)) % _nb;
                int b3WordIndex = (i + cFunc(3)) % _nb;

                byte b0 = GetByte(stateCopy[b0WordIndex], 0);
                byte b1 = GetByte(stateCopy[b1WordIndex], 1);
                byte b2 = GetByte(stateCopy[b2WordIndex], 2);
                byte b3 = GetByte(stateCopy[b3WordIndex], 3);

                state[i] = MakeWord(b0, b1, b2, b3);
            }
        }

        private void ShiftRow(uint[] state)
        {
            ShiftRowAux(state, i => C(i));
        }

        private void InverseShiftRow(uint[] state)
        {
            ShiftRowAux(state, i => _nb - C(i));
        }

        private static uint rijndaelModulo = 0x11b;

        //private void MixColumnAux(byte[] state, byte[] polynom)
        //{
        //    for (int j = 0; j < _nb; ++j)
        //    {
        //        for (int i = 0; i < 4; ++i)
        //        {
        //            uint p = 0;
        //            for (int k = 0; k < 4; ++k)
        //            {
        //                uint m = GF256.Multiply(state[Index(k, j, _nb)], polynom[(i + k) % 4], rijndaelModulo);
        //                p = GF256.Add(p, m);
        //            }
        //            state[Index(i, j, _nb)] = (byte)p;
        //        }
        //    }
        //}

        private uint Mult(uint a, uint b)
        {
            return GF256.Multiply(a, b, rijndaelModulo);
        }

        private uint Add(uint a, uint b)
        {
            return GF256.Add(a, b);
        }

        private void MixColumnAux(uint[] state, byte[] polynom)
        {
            uint c0 = polynom[0];
            uint c1 = polynom[1];
            uint c2 = polynom[2];
            uint c3 = polynom[3];

            for (int i = 0; i < _nb; ++i)
            {
                uint word = state[i];
                byte b0 = GetByte(word, 0);
                byte b1 = GetByte(word, 1);
                byte b2 = GetByte(word, 2);
                byte b3 = GetByte(word, 3);

                //Console.WriteLine($"b0 = {ByteToStringHex(b0)}");
                //Console.WriteLine($"b1 = {ByteToStringHex(b1)}");
                //Console.WriteLine($"b2 = {ByteToStringHex(b2)}");
                //Console.WriteLine($"b3 = {ByteToStringHex(b3)}");

                //byte c0b0 = (byte)Mult(c0, b0);
                //byte c1b1 = (byte)Mult(c1, b1);
                //byte c2b2 = (byte)Mult(c2, b2);
                //byte c3b3 = (byte)Mult(c3, b3);

                //Console.WriteLine($"c0 * b0 = {ByteToStringHex(c0b0)}");
                //Console.WriteLine($"c1 * b1 = {ByteToStringHex(c1b1)}");
                //Console.WriteLine($"c2 * b2 = {ByteToStringHex(c2b2)}");
                //Console.WriteLine($"c3 * b3 = {ByteToStringHex(c3b3)}");

                byte newB0 = (byte)Add(Add(Mult(c0, b0), Mult(c1, b1)), Add(Mult(c2, b2), Mult(c3, b3)));
                byte newB1 = (byte)Add(Add(Mult(c3, b0), Mult(c0, b1)), Add(Mult(c1, b2), Mult(c2, b3)));
                byte newB2 = (byte)Add(Add(Mult(c2, b0), Mult(c3, b1)), Add(Mult(c0, b2), Mult(c1, b3)));
                byte newB3 = (byte)Add(Add(Mult(c1, b0), Mult(c2, b1)), Add(Mult(c3, b2), Mult(c0, b3)));

                //Console.WriteLine($"new b0 = {ByteToStringHex(newB0)}");
                //Console.WriteLine($"new b1 = {ByteToStringHex(newB1)}");
                //Console.WriteLine($"new b2 = {ByteToStringHex(newB2)}");
                //Console.WriteLine($"new b3 = {ByteToStringHex(newB3)}");

                state[i] = MakeWord(newB0, newB1, newB2, newB3);
            }
        }

        private void MixColumn(uint[] state)
        {
            MixColumnAux(state, _mixColumnC);
        }

        private void InverseMixColumn(uint[] state)
        {
            MixColumnAux(state, _inverseMixColumnC);
        }

        private void AddRoundKey(uint[] state, ReadOnlySpan<uint> roundKey)
        {
            for (int i = 0; i < state.Length; ++i)
            {
                state[i] ^= roundKey[i];
            }
        }

        private void EncodeRound(uint[] state, ReadOnlySpan<uint> roundKey)
        {
            SubBytes(state);

            Console.WriteLine("after sub bytes:");
            PrintBlock(state);

            ShiftRow(state);

            Console.WriteLine("after shift row:");
            PrintBlock(state);

            MixColumn(state);

            Console.WriteLine("after mix column:");
            PrintBlock(state);

            AddRoundKey(state, roundKey);
        }

        private void DecodeRound(uint[] state, ReadOnlySpan<uint> roundKey)
        {
            AddRoundKey(state, roundKey);
            Console.WriteLine("after add round key:");
            PrintBlock(state);

            InverseMixColumn(state);

            Console.WriteLine("after mix column:");
            PrintBlock(state);

            InverseShiftRow(state);

            Console.WriteLine("after shift row:");
            PrintBlock(state);

            InverseSubBytes(state);
        }

        private void FinalEncodeRound(uint[] state, ReadOnlySpan<uint> roundKey)
        {
            SubBytes(state);

            Console.WriteLine("after sub bytes (final):");
            PrintBlock(state);

            ShiftRow(state);

            Console.WriteLine("after shift row (final):");
            PrintBlock(state);

            AddRoundKey(state, roundKey);
            Console.WriteLine("after add round key (final):");
            PrintBlock(state);
        }
        private void FinalDecodeRound(uint[] state, ReadOnlySpan<uint> roundKey)
        {
            AddRoundKey(state, roundKey);

            Console.WriteLine("after add round key (final):");
            PrintBlock(state);

            InverseShiftRow(state);

            Console.WriteLine("after inverse shift row (final):");
            PrintBlock(state);

            InverseSubBytes(state);

            Console.WriteLine("after inverse sub bytes (final):");
            PrintBlock(state);
        }

        public void Test(uint[] block)
        {
            MixColumn(block);
            InverseMixColumn(block);
        }

        public void EncodeBlock(uint[] block)
        {
            Console.WriteLine("ENCODING");

            if (block.Length != _nb)
            {
                throw new ArgumentException($"block must have length of {_nb * 4} bytes");
            }

            Console.WriteLine("input: ");
            PrintBlock(block);

            AddRoundKey(block, GetRoundKey(0));

            Console.WriteLine("added round key: ");
            PrintBlock(block);

            for (int i = 0; i < _nr - 1; ++i)
            {
                EncodeRound(block, GetRoundKey(i + 1));

                Console.WriteLine($"after round {i}: ");
                PrintBlock(block);
            }

            FinalEncodeRound(block, GetRoundKey(_nr));

            Console.WriteLine($"after final round: ");
            PrintBlock(block);
        }

        public void DecodeBlock(uint[] block)
        {
            Console.WriteLine("DECODING");

            if (block.Length != _nb)
            {
                throw new ArgumentException($"block must have length of {_nb * 4} bytes");
            }

            Console.WriteLine("input: ");
            PrintBlock(block);

            FinalDecodeRound(block, GetRoundKey(_nr));

            Console.WriteLine("added round key: ");
            PrintBlock(block);

            for (int i = _nr - 1; i >= 1 ; --i)
            {
                DecodeRound(block, GetRoundKey(i));

                Console.WriteLine($"after round {i}: ");
                PrintBlock(block);
            }

            AddRoundKey(block, GetRoundKey(0));

            Console.WriteLine($"after final round: ");
            PrintBlock(block);
        }

        private void PrintBlock(uint[] block)
        {
            for (int i = 0; i < block.Length; ++i)
            {
                Console.WriteLine($"{ByteToStringHex(GetByte(block[i], 0))} {ByteToStringHex(GetByte(block[i], 1))} {ByteToStringHex(GetByte(block[i], 2))} {ByteToStringHex(GetByte(block[i], 3))}");
            }
        }
        public IEnumerable<byte> EncodeDecodeBytesHelper(IEnumerable<byte> bytes, Action<uint[]> action)
        {
            uint[] block = new uint[_nb];

            var enumerator = bytes.GetEnumerator();

            bool stop = false;

            while (!stop)
            {

                for (int i = 0; i < _nb * sizeof(uint); ++i)
                {
                    if (!enumerator.MoveNext())
                    {
                        stop = true;
                        break;
                    }

                    int wordIndex = i / 4;
                    int byteIndex = i % 4;

                    if (byteIndex != 3)
                    {
                        block[wordIndex] |= ((uint)enumerator.Current) << (byteIndex * 8);
                    }
                }

                action(block);

                foreach (var word in block)
                {
                    for(int i = 0; i < 4; ++i)
                    {
                        yield return GetByte(word, i);
                    }
                }
            }
        }

        public IEnumerable<byte> EncodeBytes(IEnumerable<byte> bytes)
        {
            return EncodeDecodeBytesHelper(bytes, EncodeBlock);
        }

        public IEnumerable<byte> DecodeBytes(IEnumerable<byte> bytes)
        {
            return EncodeDecodeBytesHelper(bytes, DecodeBlock);
        }

        public IEnumerable<byte> EncodeString(string text)
        {
            return EncodeBytes(System.Text.Encoding.UTF8.GetBytes(text));
        }

        public string DecodeString(IEnumerable<byte> data)
        {
            IEnumerable<byte> bytes = DecodeBytes(data);
            return System.Text.Encoding.UTF8.GetString(bytes.ToArray());
        }

    }

    static class Extensions
    {
        private const int _chunkSize = 1024;
        public static IEnumerable<byte> ReadFrom(string filePath)
        {
            using BinaryReader binReader = new BinaryReader(File.Open(filePath, FileMode.Open));

            byte[] buffer = new byte[_chunkSize];

            while (true)
            {
                int bytesRead = binReader.Read(buffer);

                if(bytesRead == 0)
                {
                    break;
                }

                for (int i = 0; i < bytesRead; ++i)
                {
                    yield return buffer[i];
                }
            }
        }

        public static void WriteTo(string filePath, IEnumerable<byte> bytes)
        {
            using BinaryWriter binReader = new BinaryWriter(File.Open(filePath, FileMode.Create));

            byte[] buffer = new byte[_chunkSize];
            var enumerator = bytes.GetEnumerator();

            bool stop = false;

            int i = 0;
            while (!stop)
            {                
                if (!enumerator.MoveNext())
                {
                    stop = true;
                    break;
                }

                buffer[i] = enumerator.Current;
                i++;

                if (i == _chunkSize)
                {
                    binReader.Write(buffer);
                    i = 0;
                }
            }
            binReader.Write(buffer, 0, i);
        }
    } 

    class Program
    {
        private static string ByteToStringHex(byte b)
        {
            return Convert.ToString(b, 16).PadLeft(2, '0');
        }

        private static byte GetByte(uint word, int i)
        {
            return (byte)((word >> (i * 8)) & 0xFFU);
        }

        private static void PrintBlock(uint[] block)
        {
            for (int i = 0; i < block.Length; ++i)
            {
                Console.WriteLine($"{ByteToStringHex(GetByte(block[i], 0))} {ByteToStringHex(GetByte(block[i], 1))} {ByteToStringHex(GetByte(block[i], 2))} {ByteToStringHex(GetByte(block[i], 3))}");
            }
        }

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

        static (uint[] block, BlockLength blockLength)[] blocks = new (uint[] block, BlockLength blockLength)[]
        {
            (new uint[]
            {
                0x03020100,
                0x07060504,
                0x0b0a0908,
                0x0f0e0d0c,
            }, BlockLength.Bit128),
            (new uint[]
            {
                0x03020100,
                0x07060504,
                0x0b0a0908,
                0x0f0e0d0c,
                0x13121110,
                0x17161514,
            }, BlockLength.Bit192),
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
            }, BlockLength.Bit256)
        };

        static void TestRijndael()
        {
            bool success = true;
            foreach(var key in keys)
            {
                foreach(var (block, blockLength) in blocks)
                {
                    Rinjdael rj = new Rinjdael(key, blockLength);

                    var blockCopy = (uint[])block.Clone();

                    rj.EncodeBlock(blockCopy);
                    rj.DecodeBlock(blockCopy);

                    if(!Enumerable.SequenceEqual(blockCopy, block))
                    {
                        success = false;
                        break;
                    }
                }
            }

            if (success)
            {
                Console.WriteLine("SUCCESS");
            }
            else
            {
                Console.WriteLine("FAIL");
            }
        }

        private static void ThrowIfInvalidKey(ulong key)
        {
            if (key > 0xFFFFFFFFFFFFFF) // больше 56 бит ключ
            {
                throw new ArgumentException("invalid key: key must be 56-bit length");
            }
        }

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

            //ThrowIfInvalidKey(key);

            return key;
        }

        private static void StringEncoding(StringEncodingOptions options)
        {
            byte[] key = ParseKey(options.Key);

            var rijndael = new Rinjdael(key);
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

            var rijndael = new Rinjdael(key);
            string decodedString = rijndael.DecodeString(bytes);

            Console.WriteLine(decodedString);
        }

        static void EncodeFile(string filePath, string encodedFilePath, Rinjdael rinjdael)
        {
            long fileSize = (new FileInfo(filePath)).Length;

            IEnumerable<byte> bytes = BitConverter.GetBytes(fileSize).Concat(Extensions.ReadFrom(filePath));
            Extensions.WriteTo(encodedFilePath, rinjdael.EncodeBytes(bytes));
        }

        static void DecodeFile(string filePath, string decodedFilePath, Rinjdael rinjdael)
        {
            IEnumerable<byte> decodedBytes = rinjdael.DecodeBytes(Extensions.ReadFrom(filePath));
            long decodedFilesize = BitConverter.ToInt64(decodedBytes.Take(sizeof(long)).ToArray());

            Extensions.WriteTo(decodedFilePath, decodedBytes.Skip(sizeof(long)).Take((int)decodedFilesize));
        }

        static void Main(string[] args)
        {
            string filePath = "TestFile.txt";
            string encodedFilePath = "TestFile.txt.rij";
            string decodedFilePath = "DecodedTestFile.txt";

            //Extensions.WriteTo(decodedFilePath, Extensions.ReadFrom(filePath));

            byte[] key = new byte[]
            {
                0x2b, 0x7e, 0x15, 0x16,
                0x28, 0xae, 0xd2, 0xa6,
                0xab, 0xf7, 0x15, 0x88,
                0x09, 0xcf, 0x4f, 0x3c,
            };

            Rinjdael rj = new Rinjdael(key, BlockLength.Bit128);

            byte[] bytes = new byte[]{
                0x2b, 0x7e, 0x15, 0x16,
                0x28, 0xae, 0xd2, 0xa6,
                0xab, 0xf7, 0x15, 0x88,
                0x09, 0xcf, 0x4f, 0x3c,
            };

            var encoded = rj.EncodeBytes(bytes).ToList();
            var decoded = rj.DecodeBytes(encoded).ToList();

            Console.WriteLine($"SUCCESS: {Enumerable.SequenceEqual(bytes, decoded)}");

            //EncodeFile(filePath, encodedFilePath, rj);
            //DecodeFile(encodedFilePath, decodedFilePath, rj);
        }
    }
}
