using System;

namespace Rijndael
{
    public sealed class CBCRijndael : ChainedRijndael
    {
        public CBCRijndael(uint[] initializationVector, byte[] key, BlockLength blockLength = BlockLength.Bit128) : base(initializationVector, key, blockLength) { }

        public override void EncodeBlock(uint[] block)
        {
            Add(block, prevBlockCipher);

            base.EncodeBlock(block);

            Update(block);
        }
        public override void DecodeBlock(uint[] block)
        {
            CopyToTemp();
            Update(block);

            base.DecodeBlock(block);

            Add(block, temp);
        }
    }
}
