namespace Rijndael
{
    public sealed class CFBRijndael : ChainedRijndael
    {
        public CFBRijndael(uint[] initializationVector, byte[] key, BlockLength blockLength = BlockLength.Bit128) : base(initializationVector, key, blockLength) { }

        public override void EncodeBlock(uint[] block)
        {
            base.EncodeBlock(prevBlockCipher);

            Add(block, prevBlockCipher);

            Update(block);
        }

        public override void DecodeBlock(uint[] block)
        {
            CopyToTemp();
            Update(block);

            base.EncodeBlock(temp);

            Add(block, temp);
        }
    }
}
