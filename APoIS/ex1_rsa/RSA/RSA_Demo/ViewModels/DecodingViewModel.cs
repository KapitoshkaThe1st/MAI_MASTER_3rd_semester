using RSA;
using System.Numerics;
using System.Windows.Input;

namespace RSA_Demo
{
    class DecodingViewModel : BaseRSARelatedViewModel
    {
        private BigInteger _cipher = BigInteger.Zero;
        public BigInteger Cipher
        {
            get => _cipher;
            set
            {
                if (_cipher != value)
                {
                    _cipher = value;
                    OnPropertyChanged();
                }
            }
        }

        private BigInteger _message = BigInteger.Zero;
        public BigInteger Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }

        private ICommand _decodeCommand;

        public ICommand DecodeCommand => _decodeCommand ?? (_decodeCommand = new BaseCommand(Decode));

        private void Decode(object obj)
        {
            RSADecoder decoder = new RSADecoder(Modulus, PrivateExponent);
            Message = decoder.Decode(Cipher);
        }
    }
}
