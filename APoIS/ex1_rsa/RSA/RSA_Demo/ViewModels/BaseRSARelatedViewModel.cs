using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RSA_Demo
{
    class BaseRSARelatedViewModel : BaseViewModel
    {
        public BaseRSARelatedViewModel()
        {
            Modulus = 0;
            PrivateExponent = 0;
            PublicExponent = 0;
        }

        protected BigInteger _modulus;
        public BigInteger Modulus
        {
            get => _modulus;
            set
            {
                if (value != _modulus)
                {
                    _modulus = value;
                    OnPropertyChanged();
                }
            }
        }

        protected BigInteger _privateExponent;
        public BigInteger PrivateExponent
        {
            get => _privateExponent;
            set
            {
                if (value != _privateExponent)
                {
                    _privateExponent = value;
                    OnPropertyChanged();
                }
            }
        }

        protected BigInteger _publicExponent;
        public BigInteger PublicExponent
        {
            get => _publicExponent;
            set
            {
                if (value != _publicExponent)
                {
                    _publicExponent = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
