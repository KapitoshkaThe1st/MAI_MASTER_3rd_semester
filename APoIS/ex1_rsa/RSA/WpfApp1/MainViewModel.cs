using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;

namespace RSADemo
{
    class BaseCommand : ICommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public BaseCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

    class ProgressBarManager
    {
        private Stopwatch _sw;

        private Timer _timer;

        private ProgressBarWindow _window;
        private ProgressBarWindowViewModel _viewModel;

        public ProgressBarManager()
        {
            _viewModel = new ProgressBarWindowViewModel();
            _viewModel.CloseEvent += Close;

            _window = new ProgressBarWindow() {
                DataContext = _viewModel
            };

            _sw = new Stopwatch();
        }

        protected void UpdateTime()
        {
            var elapsedTime = _sw.Elapsed;
            Application.Current.Dispatcher.Invoke(() => {
                _viewModel.ElapsedTimeString = $"{elapsedTime.Hours:00}:{elapsedTime.Minutes:00}:{elapsedTime.Seconds:00}";
            });
            //Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }

        public void Start(string description)
        {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMilliseconds(100);

            _timer = new Timer((e) =>
            {
                UpdateTime();
            }, null, startTimeSpan, periodTimeSpan);

            _sw.Restart();

            _viewModel.Description = description;
            _viewModel.CurrentProgress = _viewModel.MinProgress;
            _window.ShowDialog();
        }

        public void Update(float progressChange)
        {
            UpdateTime();
            _viewModel.CurrentProgress += progressChange;
        }

        public void End()
        {
            _timer?.Dispose();
        }

        public void SetProgress(float progress)
        {
            _viewModel.CurrentProgress = progress;
        }

        public void SetDescription(string description)
        {
            _viewModel.Description = description;
        }

        public void Close()
        {
            _window.Close();
        }
    }

    class MainViewModel : BaseViewModel
    {
        private string _publicKeyStringRepresentation;
        public string PublicKeyStringRepresentation
        {
            get => _publicKeyStringRepresentation;
            set
            {
                if (value != _publicKeyStringRepresentation)
                {
                    _publicKeyStringRepresentation = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _privateKeyStringRepresentation;
        public string PrivateKeyStringRepresentation
        {
            get => _privateKeyStringRepresentation;
            set
            {
                if (value != _privateKeyStringRepresentation)
                {
                    _privateKeyStringRepresentation = value;
                    OnPropertyChanged();
                }
            }
        }

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

        ProgressBarManager _progressBarManager = new ProgressBarManager();

        private ICommand _encodeCommand;
        public ICommand EncodeCommand => _encodeCommand ?? (_encodeCommand = new BaseCommand(Encode));
        
        private void Encode(object obj)
        {
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 100; ++i)
                {
                    Thread.Sleep(10);
                    Application.Current.Dispatcher.Invoke(() => { _progressBarManager.Update(1f); });
                }
                Application.Current.Dispatcher.Invoke(() => { _progressBarManager.End(); });
            });

            _progressBarManager.Start("Encoding");
        }

        private ICommand _decodeCommand;

        public ICommand DecodeCommand => _decodeCommand ?? (_decodeCommand = new BaseCommand(Decode));

        private void Decode(object obj)
        {
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 4; ++i)
                {
                    Thread.Sleep(5000);
                    Application.Current.Dispatcher.Invoke(() => { _progressBarManager.Update(25f); });
                }
                Application.Current.Dispatcher.Invoke(() => { _progressBarManager.End(); });
            });

            _progressBarManager.Start("Decoding");
        }

        private ICommand _generateKeysCommand;

        public ICommand GenerateKeysCommand => _generateKeysCommand ?? (_generateKeysCommand = new BaseCommand(GenerateKeys));

        private void GenerateKeys(object obj)
        {
            Task.Factory.StartNew(() =>
            {
                Application.Current.Dispatcher.Invoke(() => { _progressBarManager.SetDescription("1"); });

                for (int i = 0; i < 100; ++i)
                {
                    Thread.Sleep(10);
                    if (i == 33)
                    {
                        Application.Current.Dispatcher.Invoke(() => { _progressBarManager.SetDescription("2\nfsddbrbh\nsfwefwf\naef"); });
                    }

                    if (i == 66)
                    {
                        Application.Current.Dispatcher.Invoke(() => { _progressBarManager.SetDescription("3\nfsddbrbh\nsfwefwf\naef"); });
                    }

                    Application.Current.Dispatcher.Invoke(() => { _progressBarManager.Update(1f); });
                }

                 Application.Current.Dispatcher.Invoke(() => {
                     _progressBarManager.SetProgress(100);
                     _progressBarManager.SetDescription("DONE");
                     _progressBarManager.End();
                 });
            });

            _progressBarManager.Start("Generating keys");
        }
    }
}
