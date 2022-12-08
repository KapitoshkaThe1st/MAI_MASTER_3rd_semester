using System;

namespace Rijndael
{
    public sealed class CBCRijndael : ChainedRijndael
    {
        public CBCRijndael(uint[] initializationVector, byte[] key, BlockLength blockLength = BlockLength.Bit128) : base(initializationVector, key, blockLength) 
        {
            Console.WriteLine("iv:");
            PrintBlock(initializationVector);
        }

        public override void EncodeBlock(uint[] block)
        {
            //Console.WriteLine("ENCODE BLOCK");
            //Console.WriteLine("prev:");
            //PrintBlock(prevBlockCipher);

            //Console.WriteLine("block:");
            //PrintBlock(block);
            
            Add(block, prevBlockCipher);

            //Console.WriteLine("prev added block:");
            //PrintBlock(block);

            base.EncodeBlock(block);

            //Console.WriteLine("encoded block:");
            //PrintBlock(block);

            Update(block);

            //Console.WriteLine("prev updated:");
            //PrintBlock(prevBlockCipher);
        }
        public override void DecodeBlock(uint[] block)
        {
            CopyToTemp();
            Update(block);

            //Console.WriteLine("prev updated:");
            //PrintBlock(prevBlockCipher);

            //Console.WriteLine("DECODE BLOCK");
            //Console.WriteLine("prev:");
            //PrintBlock(prevBlockCipher);

            //Console.WriteLine("block:");
            //PrintBlock(block);

            base.DecodeBlock(block);

            //Console.WriteLine("decoded block:");
            //PrintBlock(block);

            Add(block, temp);

            //Console.WriteLine("prev added block:");
            //PrintBlock(block);
        }
    }
}
