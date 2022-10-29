using System.Numerics;
using System.Windows.Input;

using RSA;

namespace RSA_Demo
{
    class EncodingViewModel : BaseRSARelatedViewModel
    {
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

        private ICommand _encodeCommand;
        public ICommand EncodeCommand => _encodeCommand ?? (_encodeCommand = new BaseCommand(Encode));
        private void Encode(object obj)
        {
            RSAEncoder encoder = new RSAEncoder(Modulus, PublicExponent);
            Cipher = encoder.Encode(Message);
        }
    }
}
