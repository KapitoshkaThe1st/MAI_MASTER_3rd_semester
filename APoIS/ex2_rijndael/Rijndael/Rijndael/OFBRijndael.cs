namespace Rijndael
{
    public sealed class OFBRijndael : ChainedRijndael
    {
        public OFBRijndael(uint[] initializationVector, byte[] key, BlockLength blockLength = BlockLength.Bit128) : base(initializationVector, key, blockLength) { }

        public override void EncodeBlock(uint[] block)
        {
            base.EncodeBlock(prevBlockCipher);

            Add(block, prevBlockCipher);
        }

        public override void DecodeBlock(uint[] block)
        {
            base.EncodeBlock(prevBlockCipher);

            Add(block, prevBlockCipher);
        }
    }
}
