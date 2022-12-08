using System;

namespace Rijndael
{
    public abstract class ChainedRijndael : Rijndael
    {
        protected uint[] initializationVector;
        protected uint[] prevBlockCipher;
        protected uint[] temp;

        public ChainedRijndael(uint[] initializationVector, byte[] key, BlockLength blockLength = BlockLength.Bit128) : base(key, blockLength)
        {
            this.initializationVector = initializationVector;
            temp = new uint[initializationVector.Length];
            Reset();
        }

        protected void Reset()
        {
            prevBlockCipher = (uint[])initializationVector.Clone();
        }

        public override byte[] EncodeString(string text)
        {
            Reset();
            return base.EncodeString(text);
        }

        public override string DecodeString(byte[] data)
        {
            Reset();
            return base.DecodeString(data);
        }

        public override void DecodeFile(string inputFilePath, string outputFilePath)
        {
            Reset();
            base.DecodeFile(inputFilePath, outputFilePath);
        }
        public override void EncodeFile(string inputFilePath, string outputFilePath)
        {
            Reset();
            base.EncodeFile(inputFilePath, outputFilePath);
        }

        protected void Add(uint[] block1, uint[] block2)
        {
            for (int i = 0; i < block1.Length; ++i)
            {
                block1[i] ^= block2[i];
            }
        }

        protected void Update(uint[] block)
        {
            Array.Copy(block, prevBlockCipher, block.Length);
        }

        protected void CopyToTemp()
        {
            Array.Copy(prevBlockCipher, temp, prevBlockCipher.Length);
        }
    }
}
