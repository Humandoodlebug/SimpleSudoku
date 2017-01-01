using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace SC.SimpleSudoku.ViewModels
{
    internal class UserViewModel : INotifyPropertyChanged
    {
        public User CurrentUser { get; }

        public UserViewModel(User currentUser)
        {
            CurrentUser = currentUser;
        }

        public string Username
        {
            get { return CurrentUser.Username; }
            set
            {
                CurrentUser.Username = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get { return CurrentUser.Password; }
            set
            {
                CurrentUser.Password = value;
                OnPropertyChanged();
            }
        }
        public int NumPuzzlesSolved
        {
            get { return CurrentUser.NumPuzzlesSolved; }
            set
            {
                CurrentUser.NumPuzzlesSolved = value;
                OnPropertyChanged();
            }
        }
        public TimeSpan AverageSolvingTime
        {
            get { return CurrentUser.AverageSolvingTime; }
            set
            {
                CurrentUser.AverageSolvingTime = value;
                OnPropertyChanged();
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(nameof(AverageSolvingTimeString));
            }
        }

        public string AverageSolvingTimeString => AverageSolvingTime.ToString("g");

        public int AveragePuzzleDifficulty
        {
            get { return CurrentUser.AveragePuzzleDifficulty; }
            set
            {
                CurrentUser.AveragePuzzleDifficulty = value;
                OnPropertyChanged();
            }
        }
        public int AverageScore
        {
            get { return CurrentUser.AverageScore; }
            set
            {
                CurrentUser.AverageScore = value;
                OnPropertyChanged();
            }
        }
        public long TotalScore
        {
            get { return CurrentUser.TotalScore; }
            set
            {
                CurrentUser.TotalScore = value;
                OnPropertyChanged();
            }
        }
        public int CurrentPuzzleSeed
        {
            get { return CurrentUser.CurrentPuzzleSeed; }
            set
            {
                CurrentUser.CurrentPuzzleSeed = value;
                OnPropertyChanged();
            }
        }
        public string CurrentPuzzleData
        {
            get { return CurrentUser.CurrentPuzzleData; }
            set
            {
                CurrentUser.CurrentPuzzleData = value;
                OnPropertyChanged();
            }
        }
        public List<Puzzle_Attempt> PuzzleAttempts
        {
            get { return CurrentUser.PuzzleAttempts; }
            set
            {
                CurrentUser.PuzzleAttempts = value;
                OnPropertyChanged();
            }
        }
        public List<Old_Password> OldPasswords
        {
            get { return CurrentUser.OldPasswords; }
            set
            {
                CurrentUser.OldPasswords = value;
                OnPropertyChanged();
            }
        }

        // ReSharper disable once InconsistentNaming
        public int CurrentBasePuzzleID
        {
            get { return CurrentUser.CurrentBasePuzzleID; }
            set
            {
                CurrentUser.CurrentBasePuzzleID = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan CurrentSolvingTime
        {
            get { return CurrentUser.CurrentSolvingTime; }
            set
            {
                CurrentUser.CurrentSolvingTime = value;
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
