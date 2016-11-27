using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SC.SimpleSudoku.ViewModels
{
    internal class UserViewModel : INotifyPropertyChanged
    {
        private readonly User _currentUser;

        public UserViewModel(User currentUser)
        {
            _currentUser = currentUser;
        }

        public string Username
        {
            get { return _currentUser.Username; }
            set
            {
                _currentUser.Username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get { return _currentUser.Password; }
            set
            {
                _currentUser.Password = value;
                OnPropertyChanged();
            }
        }
        public int NumPuzzlesSolved
        {
            get { return _currentUser.NumPuzzlesSolved; }
            set
            {
                _currentUser.NumPuzzlesSolved = value;
                OnPropertyChanged();
            }
        }
        public DateTime AverageSolvingTime
        {
            get { return _currentUser.AverageSolvingTime; }
            set
            {
                _currentUser.AverageSolvingTime = value;
                OnPropertyChanged();
            }
        }
        public int AveragePuzzleDifficulty
        {
            get { return _currentUser.AveragePuzzleDifficulty; }
            set
            {
                _currentUser.AveragePuzzleDifficulty = value;
                OnPropertyChanged();
            }
        }
        public int AverageScore
        {
            get { return _currentUser.AverageScore; }
            set
            {
                _currentUser.AverageScore = value;
                OnPropertyChanged();
            }
        }
        public long TotalScore
        {
            get { return _currentUser.TotalScore; }
            set
            {
                _currentUser.TotalScore = value;
                OnPropertyChanged();
            }
        }
        public int CurrentPuzzleSeed
        {
            get { return _currentUser.CurrentPuzzleSeed; }
            set
            {
                _currentUser.CurrentPuzzleSeed = value;
                OnPropertyChanged();
            }
        }
        public string CurrentPuzzleData
        {
            get { return _currentUser.CurrentPuzzleData; }
            set
            {
                _currentUser.CurrentPuzzleData = value;
                OnPropertyChanged();
            }
        }
        public bool IsMistakeHighlightingOn
        {
            get { return _currentUser.IsMistakeHighlightingOn; }
            set
            {
                _currentUser.IsMistakeHighlightingOn = value;
                OnPropertyChanged();
            }
        }
        public bool IsLeaderboardVisible
        {
            get { return _currentUser.IsLeaderboardVisible; }
            set
            {
                _currentUser.IsLeaderboardVisible = value;
                OnPropertyChanged();
            }
        }
        public bool IsPuzzleTimerVisible
        {
            get { return _currentUser.IsPuzzleTimerVisible; }
            set
            {
                _currentUser.IsPuzzleTimerVisible = value;
                OnPropertyChanged();
            }
        }
        public List<Puzzle_Attempt> PuzzleAttempts
        {
            get { return _currentUser.PuzzleAttempts; }
            set
            {
                _currentUser.PuzzleAttempts = value;
                OnPropertyChanged();
            }
        }
        public List<Old_Password> OldPasswords
        {
            get { return _currentUser.OldPasswords; }
            set
            {
                _currentUser.OldPasswords = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
