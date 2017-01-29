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
    /// <summary>
    /// Used to time a user while they solve a puzzle.
    /// </summary>
    class PuzzleTimerViewModel : INotifyPropertyChanged
    {
        private User CurrentUser { get; }
        private readonly DispatcherTimer _timer;

        /// <summary>
        /// The time elapsed while the user has been solving the puzzle, incremented each second.
        /// </summary>
        public TimeSpan CurrenTimeSpan { get; set; } = TimeSpan.FromSeconds(-2);

        /// <summary>
        /// The PuzzleTimerViewModel constructor.
        /// </summary>
        /// <param name="currentUser">The currently singed in user.</param>
        public PuzzleTimerViewModel(User currentUser)
        {
            CurrentUser = currentUser;
            //Set up the new timer.
            _timer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};
            _timer.Tick += Tick;
        }
        public void Start()
        {
            CurrenTimeSpan += TimeSpan.FromSeconds(1);
            _timer.Start();
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(nameof(CurrenTimeSpan));
        }

        /// <summary>
        /// Handles the Puzzle timer's tick event (called every second). Increments CurrentTimeSpan by a second.
        /// </summary>
        /// <param name="sender">The object raising the event (the puzzle timer).</param>
        /// <param name="e">Any arguguments to be passed in.</param>
        private void Tick(object sender, object e)
        {
            CurrenTimeSpan = CurrenTimeSpan.Add(TimeSpan.FromSeconds(1));
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(nameof(CurrenTimeSpan));
        }

        /// <summary>
        /// Stops the puzzle timer.
        /// </summary>
        public void Stop() => _timer.Stop();

        /// <summary>
        /// Event raised when a bound property changes. Subscribed to by the UI.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when a bound property changes. Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChanged = PropertyChanged;
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
