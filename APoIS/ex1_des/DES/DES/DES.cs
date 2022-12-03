using System;

namespace DES
{
    //https://page.math.tu-berlin.de/~kant/teaching/hess/krypto-ws2006/des.htm
    public sealed class DES
    {
        // первичная перестановка
        private static byte[] _ipTable = new byte[64] {
            58, 50, 42, 34, 26, 18, 10, 2,
            60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6,
            64, 56, 48, 40, 32, 24, 16, 8,
            57, 49, 41, 33, 25, 17,  9, 1,
            59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5,
            63, 55, 47, 39, 31, 23, 15, 7
        };

        // обратная к первичной перестановка
        private static byte[] _iipTable = new byte[64] {
            40, 8, 48, 16, 56, 24, 64, 32,
            39, 7, 47, 15, 55, 23, 63, 31,
            38, 6, 46, 14, 54, 22, 62, 30,
            37, 5, 45, 13, 53, 21, 61, 29,
            36, 4, 44, 12, 52, 20, 60, 28,
            35, 3, 43, 11, 51, 19, 59, 27,
            34, 2, 42, 10, 50, 18, 58, 26,
            33, 1, 41,  9, 49, 17, 57, 25
        };


        // перестановка с расширением
        private static byte[] _eTable = new byte[48]
        {
            32,  1,  2,  3,  4,  5,
             4,  5,  6,  7,  8,  9,
             8,  9, 10, 11, 12, 13,
            12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29,
            28, 29, 30, 31, 32,  1
        };

        // последняя перестановка в F
        private static byte[] _pTable = new byte[32]
        {
            16,  7, 20, 21,
            29, 12, 28, 17,
             1, 15, 23, 26,
             5, 18, 31, 10,
             2,  8, 24, 14,
            32, 27,  3,  9,
            19, 13, 30,  6,
            22, 11,  4, 25
        };

        // таблица подстановок s-block
        private static byte[,,] _sTables = new byte[8, 4, 16] {
            { // 0
                { 14,  4, 13, 1,  2, 15, 11,  8,  3, 10,  6, 12,  5,  9, 0,  7 },
                {  0, 15,  7, 4, 14,  2, 13,  1, 10,  6, 12, 11,  9,  5, 3,  8 },
                {  4,  1, 14, 8, 13,  6,  2, 11, 15, 12,  9,  7,  3, 10, 5,  0 },
                { 15, 12,  8, 2,  4,  9,  1,  7,  5, 11,  3, 14, 10,  0, 6, 13 }
            },
            { // 1
                { 15,  1,  8, 14,  6, 11,  3,  4,  9, 7,  2, 13, 12, 0,  5, 10 },
                {  3, 13,  4,  7, 15,  2,  8, 14, 12, 0,  1, 10,  6, 9, 11,  5 },
                {  0, 14,  7, 11, 10,  4, 13,  1,  5, 8, 12,  6,  9, 3,  2, 15 },
                { 13,  8, 10,  1,  3, 15,  4,  2, 11, 6,  7, 12,  0, 5, 14,  9 }
            },
            { // 2
                { 10,  0,  9, 14, 6,  3, 15,  5,  1, 13, 12,  7, 11,  4,  2,  8 },
                { 13,  7,  0,  9, 3,  4,  6, 10,  2,  8,  5, 14, 12, 11, 15,  1 },
                { 13,  6,  4,  9, 8, 15,  3,  0, 11,  1,  2, 12,  5, 10, 14,  7 },
                {  1, 10, 13,  0, 6,  9,  8,  7,  4, 15, 14,  3, 11,  5,  2, 12 }
            },
            { // 3
                {  7, 13, 14, 3,  0,  6,  9, 10,  1, 2, 8,  5, 11, 12,  4, 15 },
                { 13,  8, 11, 5,  6, 15,  0,  3,  4, 7, 2, 12,  1, 10, 14,  9 },
                { 10,  6,  9, 0, 12, 11,  7, 13, 15, 1, 3, 14,  5,  2,  8,  4 },
                {  3, 15,  0, 6, 10,  1, 13,  8,  9, 4, 5, 11, 12,  7,  2, 14 }
            },
            { // 4
                {  2, 12,  4,  1,  7, 10, 11,  6,  8,  5,  3, 15, 13, 0, 14,  9 },
                { 14, 11,  2, 12,  4,  7, 13,  1,  5,  0, 15, 10,  3, 9,  8,  6 },
                {  4,  2,  1, 11, 10, 13,  7,  8, 15,  9, 12,  5,  6, 3,  0, 14 },
                { 11,  8, 12,  7,  1, 14,  2, 13,  6, 15,  0,  9, 10, 4,  5,  3 }
            },
            { // 5
                { 12,  1, 10, 15, 9,  2,  6,  8,  0, 13,  3,  4, 14,  7,  5, 11 },
                { 10, 15,  4,  2, 7, 12,  9,  5,  6,  1, 13, 14,  0, 11,  3,  8 },
                {  9, 14, 15,  5, 2,  8, 12,  3,  7,  0,  4, 10,  1, 13, 11,  6 },
                {  4,  3,  2, 12, 9,  5, 15, 10, 11, 14,  1,  7,  6,  0,  8, 13 }
            },
            { // 6
                {  4, 11,  2, 14, 15, 0,  8, 13,  3, 12, 9,  7,  5, 10, 6,  1 },
                { 13,  0, 11,  7,  4, 9,  1, 10, 14,  3, 5, 12,  2, 15, 8,  6 },
                {  1,  4, 11, 13, 12, 3,  7, 14, 10, 15, 6,  8,  0,  5, 9,  2 },
                {  6, 11, 13,  8,  1, 4, 10,  7,  9,  5, 0, 15, 14,  2, 3, 12 }
            },
            { // 7
                { 13,  2,  8, 4,  6, 15, 11,  1, 10,  9,  3, 14,  5,  0, 12,  7 },
                {  1, 15, 13, 8, 10,  3,  7,  4, 12,  5,  6, 11,  0, 14, 9,   2 },
                {  7, 11,  4, 1,  9, 12, 14,  2,  0,  6, 10, 13, 15,  3,  5,  8 },
                {  2,  1, 14, 7,  4, 10,  8, 13, 15, 12,  9,  0,  3,  5,  6, 11 }
            },
        };

        // первая перестановка при генерации ключей
        private static byte[] _kpTable = new byte[56] {
            57, 49, 41, 33, 25, 17,  9,
            1,  58, 50, 42, 34, 26, 18,
            10,  2, 59, 51, 43, 35, 27,
            19, 11,  3, 60, 52, 44, 36,
            63, 55, 47, 39, 31, 23, 15,
             7, 62, 54, 46, 38, 30, 22,
            14,  6, 61, 53, 45, 37, 29,
            21, 13,  5, 28, 20, 12,  4
        };

        // последняя перестановка при генерации ключей
        private static byte[] _cpTable = new byte[48] {
            14, 17, 11, 24,  1,  5,
             3, 28, 15,  6, 21, 10,
            23, 19, 12,  4, 26,  8,
            16,  7, 27, 20, 13,  2,
            41, 52, 31, 37, 47, 55,
            30, 40, 51, 45, 33, 48,
            44, 49, 39, 56, 34, 53,
            46, 42, 50, 36, 29, 32
        };

        // таблица сдвигов для генерации раундовых ключей
        private static byte[] _shiftTable = new byte[16]
        {
            1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1
        };

        private ulong _key;
        private ulong[] _encodeRoundKeys;
        private ulong[] _decodeRoundKeys;

        // проверка целостности ключа подсчетом числа единиц в каждом бите (должно быть нечетное число единиц)
        private bool CheckParity(ulong key64)
        {
            for(int i = 0; i < 8; ++i)
            {
                if(BitOperations.Parity((byte)(key64 & 0xFF)) != 1)
                {
                    return false;
                }
                key64 >>= 8; 
            }

            return true;
        }

        public DES(ulong key64)
        {
            // проверка целостности ключа
            if (!CheckParity(key64))
            {
                throw new ArgumentException("key parity-test failed");
            }

            _key = key64;
            // генерация раундовых ключей для шифрования
            _encodeRoundKeys = GenerateKeys(_key);

            // получение раундовых ключей для дешифрования -- ключи для шифрования в обратном порядке
            _decodeRoundKeys = (ulong[])_encodeRoundKeys.Clone();
            Array.Reverse(_decodeRoundKeys);
        }

        // перестановка в общем виде
        private static ulong Permutation(ulong block, int inputBitLength, byte[] permutationTable)
        {
            ulong result = 0;

            int outputBitLength = permutationTable.Length;

            for (int i = 0; i < outputBitLength; ++i)
            {
                int bitIndex = inputBitLength - permutationTable[i];
                ulong bit = (block >> bitIndex) & 1;
                result |= bit << (outputBitLength - i - 1);
            }

            return result;
        }

        // первичная перестановка
        private static ulong InitialPermutation(ulong block64)
        {
            return Permutation(block64, 64, _ipTable);
        }

        // последняя перестановка -- обратная к первичной
        private static ulong FinalPermutation(ulong block64)
        {
            return Permutation(block64, 64, _iipTable);
        }

        // перестановка с расширением блока
        private static ulong BlockExpansion(uint block32)
        {
            return Permutation(block32, 32, _eTable);
        }

        // каждый из 8-ми кусочков из 6-ти бит преобразуются в 8 кусочков из 4-х бит при помощи s-block'а -- нелинейность
        private static uint SBlock(ulong block48)
        {
            uint result = 0;
            for (int k = 0; k < 8; ++k)
            {
                byte sblock = (byte)(block48 & 0b111111UL);

                int i = ((sblock >> 5) << 1) | (sblock & 1); // первый и последний бит шестизначного числа
                int j = (sblock >> 1) & ~(1 << 4); // все биты кроме первого и последнего

                // подстановка
                byte newBits = _sTables[7 - k, i, j];

                result |= ((uint)newBits << (4 * k));

                block48 >>= 6;
            }

            return result;
        }

        // последняя перестановка в F
        private static uint PPermutation(uint block32)
        {
            return (uint)Permutation(block32, 32, _pTable);
        }


        private static uint F(uint block32, ulong key48)
        {
            // расширение блока перестановкой с расширением
            ulong block48 = BlockExpansion(block32);

            // сложение с раундовым ключом
            ulong keyApplied = block48 ^ key48;
            
            // применение S-block'ов: на входе 8 6-битных кусочков, на выходе 8 4-х битных кусочков
            uint sBlocksApplied = SBlock(keyApplied);

            // еще одна перестановка
            uint pPermutationApplied = PPermutation(sBlocksApplied);

            return pPermutationApplied;
        }

        private static void Round(ref uint block32L, ref uint block32R, ulong key48)
        {
            uint newBlock32R = block32L ^ F(block32R, key48);
            block32L = block32R;
            block32R = newBlock32R;
        }

        private static ulong KeyPermutation(ulong key56)
        {
            return Permutation(key56, 56, _cpTable);
        }

        private static ulong KeyFirstPermutation(ulong key64)
        {
            return Permutation(key64, 64, _kpTable);
        }

        private static ulong[] GenerateKeys(ulong key64)
        {
            ulong[] result = new ulong[16];

            // начальная перестановка битов ключа-64 отбрасывание контрольных битов -> ключ-56
            ulong key56 = KeyFirstPermutation(key64);

            // делим на две частии по 28 бит: левая и правая
            uint c = (uint)(key56 >> 28);
            uint d = (uint)(key56 & 0xFFFFFFF);

            // генерируем 16 ключей, по ключу на раунд
            for (int i = 0; i < 16; ++i)
            {
                // циклический сдвиг каждой части в пределах 28 бит на число, зависещае от номера ключа 
                c = BitOperations.RotateLeft(c, 28, _shiftTable[i]);
                d = BitOperations.RotateLeft(d, 28, _shiftTable[i]);

                // склеивание частей и применение конечной перестановки для ключа
                result[i] = KeyPermutation(((ulong)c << 28) | d);
            }

            return result;
        }

        private static ulong Encode(ulong block64, ulong[] roundKeys)
        {
            // применяем начальную перестановку
            block64 = InitialPermutation(block64);

            uint l = (uint)(block64 >> 32);
            uint r = (uint)(block64 & 0xFFFFFFFF);

            // 16 раундов кодирования
            for (int i = 0; i < 16; ++i)
            {
                Round(ref l, ref r, roundKeys[i]);
            }

            // склейка частей обратно с бестолковой перестановкой, как в стандарте
            block64 = ((ulong)r << 32) | l;

            // последнаяя перестановка, обратная к начальной
            return FinalPermutation(block64);
        }

        private ulong EncodeBlock(ulong block64)
        {
            return Encode(block64, _encodeRoundKeys);
        }

        private ulong DecodeBlock(ulong block64)
        {
            return Encode(block64, _decodeRoundKeys);
        }

        private const int blockSizeInBytes = 8;

        public byte[] Encode(byte[] data)
        {
            int remainder = data.Length % blockSizeInBytes;
            int cipherByteLength = data.Length + (remainder == 0 ? 0 : blockSizeInBytes - remainder);

            byte[] result = new byte[cipherByteLength];

            for(int i = 0; i < data.Length; i += blockSizeInBytes)
            {
                ulong block;
                if(i + blockSizeInBytes > data.Length)
                {
                    block = 0;
                    for(int j = 0; j < remainder; ++j)
                    {
                        byte curByte = data[i + j];

                        int shift = j * 8;

                        block |= ((ulong)curByte << shift);
                    }
                }
                else
                {
                    block = BitConverter.ToUInt64(new ReadOnlySpan<byte>(data, i, blockSizeInBytes));
                }

                ulong blockCipher = EncodeBlock(block);

                Array.Copy(BitConverter.GetBytes(blockCipher), 0, result, i, blockSizeInBytes);
            }

            return result;
        }

        public byte[] Decode(byte[] data)
        {
            int remainder = data.Length % blockSizeInBytes;

            if(remainder != 0)
            {
                throw new ArgumentException("length of data to decode must be multiple of 8 bytes");
            }

            byte[] result = new byte[data.Length];

            for (int i = 0; i < data.Length; i += blockSizeInBytes)
            {
                ulong blockCipher = BitConverter.ToUInt64(new ReadOnlySpan<byte>(data, i, blockSizeInBytes));
                ulong block = DecodeBlock(blockCipher);

                Array.Copy(BitConverter.GetBytes(block), 0, result, i, blockSizeInBytes);
            }

            return result;
        }

        public byte[] EncodeString(string text)
        {
            return Encode(System.Text.Encoding.UTF8.GetBytes(text));
        }

        public string DecodeString(byte[] data)
        {
            byte[] bytes = Decode(data);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
