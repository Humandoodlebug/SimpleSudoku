using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.UI.Xaml;

namespace SC.SimpleSudoku.ViewModels
{
    class PuzzleTimerViewModel : INotifyPropertyChanged
    {
        private User CurrentUser { get; }
        private readonly DispatcherTimer _timer;
        public TimeSpan CurrenTimeSpan { get; set; } = TimeSpan.FromSeconds(-2);

        public string TimerString => $"Elapsed Time: {Math.Truncate(CurrenTimeSpan.TotalHours):00}:{CurrenTimeSpan.Minutes:00}:{CurrenTimeSpan.Seconds:00}";

        public PuzzleTimerViewModel(User currentUser)
        {
            CurrentUser = currentUser;
            _timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            _timer.Tick += Tick;
        }
        public void Start()
        {
            CurrenTimeSpan += TimeSpan.FromSeconds(1);
            _timer.Start();
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(nameof(TimerString));
        }

        private void Tick(object sender, object e)
        {
            CurrenTimeSpan = CurrenTimeSpan.Add(TimeSpan.FromSeconds(1));
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(nameof(TimerString));
        }
        public void Stop() => _timer.Stop();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
