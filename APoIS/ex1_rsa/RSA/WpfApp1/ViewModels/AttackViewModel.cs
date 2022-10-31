using System;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using RSA_Attack;

namespace RSA_Demo
{
    enum AttackType
    {
        [Description("Modulus factorization attack")]
        ModulusFactorization,
        [Description("Wiener attack")]
        WienerAttack
    }

    class AttackViewModel : BaseRSARelatedViewModel
    {
        private ProgressBarManager _progressBarManager;

        public AttackViewModel()
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

        private AttackType _attackType = AttackType.ModulusFactorization;
        public AttackType AttackType
        {
            get => _attackType;
            set
            {
                if (_attackType != value)
                {
                    _attackType = value;
                    OnPropertyChanged();
                }
            }
        }

        private BaseRSAAttack CreateAttack(Action<float> updateCallback, Action<bool> endCallback) => _attackType switch
        {
            AttackType.ModulusFactorization => new ModulusFactorizationAttack(updateCallback, endCallback),
            AttackType.WienerAttack => new WienerAttack(updateCallback, endCallback),
            _ => throw new ArgumentException("Unknown attack type")
        };

        private string AttackDescription() => _attackType switch
        {
            AttackType.ModulusFactorization => "modulus factorization attack",
            AttackType.WienerAttack => "Wiener attack",
            _ => throw new ArgumentException("Unknown attack type")
        };

        private ICommand _performAttackCommand;
        public ICommand PerformAttackCommand => _performAttackCommand ?? (_performAttackCommand = new BaseCommand(PerformAttack));

        private void PerformAttack(object o)
        {
            Task.Factory.StartNew(() =>
            {
                BaseRSAAttack attack = CreateAttack(
                    step => Application.Current.Dispatcher.Invoke(() =>
                    {
                        _progressBarManager.Update(step);
                    }),
                    succes => Application.Current.Dispatcher.Invoke(() =>
                    {
                        _progressBarManager.End();
                        _progressBarManager.Description += succes ? "\nsucceded" : "\nfailed";
                    }));

                var attackResult = attack.Attack(Modulus, PublicExponent);

                if (attackResult.Success)
                {
                    PrivateExponent = attackResult.PrivateExponent;
                    P = attackResult.P;
                    Q = attackResult.Q;
                }
            });

            _progressBarManager.Start("Performing " + AttackDescription());
        }
    }
}
