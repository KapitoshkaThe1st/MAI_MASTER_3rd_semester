using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using RSA;

namespace RSA_Demo
{
    class ModulusFactorizationAttackViewModel : BaseRSARelatedViewModel
    {
        private ProgressBarManager _progressBarManager;

        public ModulusFactorizationAttackViewModel()
        {
            _progressBarManager = ProgressBarManager.GetInstance();
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

        private ICommand _decomposeCommand;

        public ICommand DecomposeCommand => _decomposeCommand ?? (_decomposeCommand = new BaseCommand(Decompose));

        private void Decompose(object o)
        {
            Task.Factory.StartNew(() => {
                var limit = BigIntegerExtensions.Sqrt(Modulus);
                BigInteger p = 3;
                BigInteger q = 1;

                float step = 200.0f / (float)limit;

                for (; p <= limit; p += 2)
                {
                    q = BigInteger.DivRem(Modulus, p, out var rem);
                    if (rem == BigInteger.Zero)
                    {
                        break;
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        _progressBarManager.Update(step);
                    });
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _progressBarManager.End();
                });

                P = p;
                Q = q;

                var phiN = (p - 1) * (q - 1);

                PublicExponent = new BigInteger(65537);

                MathUtils.ModularMultiplicativeInverse(PublicExponent, phiN, out BigInteger privateExponent);

                PrivateExponent = privateExponent;
            });

            _progressBarManager.Start("RSA modulus factorization");
        }
    }
}
