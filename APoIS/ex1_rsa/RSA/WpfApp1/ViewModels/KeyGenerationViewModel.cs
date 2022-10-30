using RSA;
using RSA.PrimalityTest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RSA_Demo
{
    enum PrimalityTestType
    {
        [Description("Fermat test")]
        FermatTest,
        [Description("Solovay-Strassen test")]
        SolovayStrassenTest,
        [Description("Miller-Rabin test")]
        MillerRabinTest
    }

    class KeyGenerationViewModel : BaseRSARelatedViewModel
    {
        private ProgressBarManager _progressBarManager;

        public KeyGenerationViewModel()
        {
            _progressBarManager = ProgressBarManager.GetInstance();
            _keyGenerators = new Dictionary<(RSAKeyType keyType, PrimalityTestType testType), RSAKeyGenerator>();
        }

        private RSAKeyType _rsaKeyType = RSAKeyType.RSA2048;
        public RSAKeyType RSAKeyType
        {
            get => _rsaKeyType;
            set
            {
                if(_rsaKeyType != value)
                {
                    _rsaKeyType = value;
                    OnPropertyChanged();
                }
            }
        }

        private PrimalityTestType _primalityTestType = PrimalityTestType.MillerRabinTest;
        public PrimalityTestType PrimalityTestType
        {
            get => _primalityTestType;
            set
            {
                if (_primalityTestType != value)
                {
                    _primalityTestType = value;
                    OnPropertyChanged();
                }
            }
        }

        private BigInteger _p = BigInteger.Zero;
        public BigInteger P
        {
            get => _p;
            set
            {
                if (_p != value)
                {
                    _p = value;
                    OnPropertyChanged();
                }
            }
        }

        private BigInteger _q = BigInteger.Zero;
        public BigInteger Q
        {
            get => _q;
            set
            {
                if (_q != value)
                {
                    _q = value;
                    OnPropertyChanged();
                }
            }
        }

        private const float _requiredPrimalityProbability = 0.995f;

        private Dictionary<(RSAKeyType keyType, PrimalityTestType testType), RSAKeyGenerator> _keyGenerators;

        private RSAKeyGenerator CreateKeyGenerator() => _primalityTestType switch
        {
            PrimalityTestType.FermatTest => new RSAKeyGenerator(new FermatTest(_requiredPrimalityProbability), _rsaKeyType),
            PrimalityTestType.SolovayStrassenTest => new RSAKeyGenerator(new SolovayStrassenTest(_requiredPrimalityProbability), _rsaKeyType),
            PrimalityTestType.MillerRabinTest => new RSAKeyGenerator(new MillerRabinTest(_requiredPrimalityProbability), _rsaKeyType),
            _ => throw new ArgumentException("Unknown primality test")
        };

        private RSAKeyGenerator GetKeyGenerator()
        {
            var key = (_rsaKeyType, _primalityTestType);
            if(!_keyGenerators.TryGetValue(key, out var result))
            {
                _keyGenerators[key] = result = CreateKeyGenerator();
            }

            return result;
        }

        private ICommand _generateKeysCommand;
        public ICommand GenerateKeysCommand => _generateKeysCommand ?? (_generateKeysCommand = new BaseCommand(GenerateKeys));

        private void GenerateKeys(object obj)
        {
            Task.Factory.StartNew(() =>
            {
                var keyGenerator = GetKeyGenerator();
                keyGenerator.Generate(out var modulus, out var publicExponent, out var privateExponent);

                Modulus = modulus;
                PublicExponent = publicExponent;
                PrivateExponent = privateExponent;

                // дебильно, но по-другому сложно доставать P и Q
                P = keyGenerator.P;
                Q = keyGenerator.Q;

                Application.Current.Dispatcher.Invoke(() => {
                    _progressBarManager.End();
                });
            });

            _progressBarManager.Start("Generating keys");
        }
    }
}
