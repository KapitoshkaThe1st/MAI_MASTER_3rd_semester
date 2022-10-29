using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Numerics;

using RSA;

namespace RSA_Demo
{
    class MainViewModel : BaseViewModel
    {
        private string _text;
        public string Text
        {
            get => _text;
            set
            {
                if (value != _text)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _cipherText;
        public string CipherText
        {
            get => _cipherText;
            set
            {
                if (value != _cipherText)
                {
                    _cipherText = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MinProgress => 0;
        public int MaxProgress => 100;

        private int _progress = 0;
        public int Progress
        {
            get => _progress;
            set
            {
                if (value != _progress)
                {
                    _progress = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
