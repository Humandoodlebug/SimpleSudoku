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
    /// <summary>
    /// Exposes information about a user as bindable properties for the UI.
    /// </summary>
    internal class UserViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The underlying user being exposed.
        /// </summary>
        public User CurrentUser { get; }

        /// <summary>
        /// The UserViewModel constructor.
        /// </summary>
        /// <param name="currentUser">The underlying user to be exposed.</param>
        public UserViewModel(User currentUser)
        {
            CurrentUser = currentUser;
        }

        /// <summary>
        /// Exposes the user's username.
        /// </summary>
        public string Username
        {
            get { return CurrentUser.Username; }
            set
            {
                CurrentUser.Username = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Exposes the user's password.
        /// </summary>
        public string Password
        {
            get { return CurrentUser.Password; }
            set
            {
                CurrentUser.Password = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Exposes the number of puzzles solved by the user.
        /// </summary>
        public int NumPuzzlesSolved
        {
            get { return CurrentUser.NumPuzzlesSolved; }
            set
            {
                CurrentUser.NumPuzzlesSolved = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Exposes the user's average solving time.
        /// </summary>
        public TimeSpan AverageSolvingTime
        {
            get { return CurrentUser.AverageSolvingTime; }
            set
            {
                CurrentUser.AverageSolvingTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Exposes the user's average puzzle difficulty.
        /// </summary>
        public double AveragePuzzleDifficulty
        {
            get { return CurrentUser.AveragePuzzleDifficulty; }
            set
            {
                CurrentUser.AveragePuzzleDifficulty = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Exposes the user's average score.
        /// </summary>
        public double AverageScore
        {
            get { return CurrentUser.AverageScore; }
            set
            {
                CurrentUser.AverageScore = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Exposes the user's total score.
        /// </summary>
        public long TotalScore
        {
            get { return CurrentUser.TotalScore; }
            set
            {
                CurrentUser.TotalScore = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Exposes the seed of the puzzle last saved by the user.
        /// </summary>
        public int CurrentPuzzleSeed
        {
            get { return CurrentUser.CurrentPuzzleSeed; }
            set
            {
                CurrentUser.CurrentPuzzleSeed = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Exposes the puzzle data for the puzzle last saved by the user as a string.
        /// </summary>
        public string CurrentPuzzleData
        {
            get { return CurrentUser.CurrentPuzzleData; }
            set
            {
                CurrentUser.CurrentPuzzleData = value;
                OnPropertyChanged();
            }
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Exposes the base puzzle ID of the puzzle last saved by the user.
        /// </summary>
        public int CurrentBasePuzzleID
        {
            get { return CurrentUser.CurrentBasePuzzleId; }
            set
            {
                CurrentUser.CurrentBasePuzzleId = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Exposes the elapsed time in the puzzle last saved by the user.
        /// </summary>
        public TimeSpan CurrentSolvingTime
        {
            get { return CurrentUser.CurrentSolvingTime; }
            set
            {
                CurrentUser.CurrentSolvingTime = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Raised when a property changes in the class. Notifies subscribers, such as the UI.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event in the UI.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.
        /// Is usually retrieved automatically by the compiler.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
