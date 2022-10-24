using System.Diagnostics;
using System.Windows.Input;

namespace RSADemo
{
    class ProgressBarWindowViewModel : BaseViewModel
    {
        private string _elapsedTimeString; 
        public string ElapsedTimeString
        {
            get => _elapsedTimeString;
            set
            {
                if (value != _elapsedTimeString)
                {
                    _elapsedTimeString = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if(value != _description)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public float MinProgress => 0.0f;
        public float MaxProgress => 100.0f;

        private float _currentProgress;
        public float CurrentProgress
        {
            get => _currentProgress;
            set
            {
                if (value != _currentProgress)
                {
                    _currentProgress = value;
                    OnPropertyChanged();
                }
                CloseButtonEnabled = value >= MaxProgress;
            }
        }

        private bool _closeButtonEnabled = false;
        public bool CloseButtonEnabled {
            get => _closeButtonEnabled;
            set
            {
                if(_closeButtonEnabled != value)
                {
                    _closeButtonEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public delegate void CloseDelegate();
        public event CloseDelegate CloseEvent;

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ?? (_closeCommand = new BaseCommand(Close));

        public void Close(object o)
        {
            CloseEvent?.Invoke();
        }
    }
}
