using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RSA_Demo
{
    class ProgressBarManager
    {
        private Stopwatch _sw;

        private Timer _timer;

        private ProgressBarWindow _window;
        private ProgressBarWindowViewModel _viewModel;

        private static Lazy<ProgressBarManager> _instance = new(() => new ProgressBarManager());
        public static ProgressBarManager GetInstance()
        {
            return _instance.Value;
        }

        private ProgressBarManager()
        {
            _viewModel = new ProgressBarWindowViewModel();
            _viewModel.CloseEvent += Close;

            _window = new ProgressBarWindow()
            {
                DataContext = _viewModel
            };

            _sw = new Stopwatch();
        }

        protected void UpdateTime()
        {
            var elapsedTime = _sw.Elapsed;
            Application.Current.Dispatcher.Invoke(() =>
            {
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
            SetProgress(100f);
            _timer?.Dispose();
        }

        public void SetProgress(float progress)
        {
            _viewModel.CurrentProgress = progress;
        }

        public string Description
        {
            get => _viewModel.Description;
            set => _viewModel.Description = value;
        }

        public void Close()
        {
            _window.Close();
        }
    }
}
