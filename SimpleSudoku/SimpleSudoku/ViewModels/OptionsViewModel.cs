using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SC.SimpleSudoku.ViewModels
{
    /// <summary>
    ///     Exposes a user's options to the User Interface for binding.
    /// </summary>
    internal class OptionsViewModel : INotifyPropertyChanged
    {
        private readonly User _currentUser;

        /// <summary>
        ///     A constructor, called when an OptionsViewModel object is created.
        /// </summary>
        /// <param name="currentUser">The user to create bindable options for.</param>
        public OptionsViewModel(User currentUser)
        {
            _currentUser = currentUser;
        }

        /// <summary>
        ///     Exposes whether the user wants mistakes they make when solving puzzles to be highlighted (outlined in red).
        /// </summary>
        public bool IsMistakeHighlightingOn
        {
            get { return _currentUser.IsMistakeHighlightingOn; }
            set
            {
                if (_currentUser.IsMistakeHighlightingOn == value)
                    return;
                _currentUser.IsMistakeHighlightingOn = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Exposes whether the user wants the user leaderboard to be available in the main menu and the puzzle leaderboard to
        ///     show after a puzzle is solved.
        /// </summary>
        public bool IsLeaderboardVisible
        {
            get { return _currentUser.IsLeaderboardVisible; }
            set
            {
                if (_currentUser.IsLeaderboardVisible == value)
                    return;
                _currentUser.IsLeaderboardVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Exposes whether the puzzle timer should be visible when solving puzzles.
        /// </summary>
        public bool IsPuzzleTimerVisible
        {
            get { return _currentUser.IsPuzzleTimerVisible; }
            set
            {
                if (_currentUser.IsPuzzleTimerVisible == value)
                    return;
                _currentUser.IsPuzzleTimerVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Event raised when the value of a bound property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when the value of a bound property changes. Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}