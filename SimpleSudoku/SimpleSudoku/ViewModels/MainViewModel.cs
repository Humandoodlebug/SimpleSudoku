using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SC.SimpleSudoku.Model;

// ReSharper disable ExplicitCallerInfoArgument

namespace SC.SimpleSudoku.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private NavigationState _currentNavState = new NavigationState();
        public CellViewModel[][] CurrentSudokuPuzzle { get; set; }

        public NavigationState CurrentNavState
        {
            get { return _currentNavState; }
            set
            {
                if (_currentNavState == value)
                    return;
                _currentNavState = value;
                OnPropertyChanged();
            }
        }

        public User CurrentUser { get; set; }


        public ICommand NewPuzzleCommand => new DelegateCommand(obj => NewPuzzle());

        public event PropertyChangedEventHandler PropertyChanged;

        public void NewPuzzle()
        {
            CurrentNavState.RecordView();
            CurrentNavState.CurrentView = NavigationState.View.PuzzleDifficulty;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal class NavigationState : INotifyPropertyChanged
        {
            public enum View
            {
                MainMenu,
                PuzzleDifficulty,
                Solving
            }

            private View _currentView;
            private NavigationState _previousNavState;

            public View CurrentView
            {
                get { return _currentView; }
                set
                {
                    if (_currentView == value)
                        return;
                    _currentView = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsMainMenuVisible));
                    OnPropertyChanged(nameof(IsPuzzleDifficultyVisible));
                    OnPropertyChanged(nameof(IsSolvingVisible));
                }
            }

            public bool IsMainMenuVisible => CurrentView == View.MainMenu;
            public bool IsPuzzleDifficultyVisible => CurrentView == View.PuzzleDifficulty;
            public bool IsSolvingVisible => CurrentView == View.Solving;

            public NavigationState PreviousNavState
            {
                get { return _previousNavState; }
                set
                {
                    if (_previousNavState == value)
                        return;
                    _previousNavState = value;
                    OnPropertyChanged();
                }
            }


            public event PropertyChangedEventHandler PropertyChanged;

            public void RecordView()
            {
                PreviousNavState = new NavigationState { PreviousNavState = PreviousNavState, CurrentView = CurrentView };
            }

            public bool GoBack()
            {
                if (PreviousNavState == null)
                return false;
                CurrentView = PreviousNavState.CurrentView;

                PreviousNavState = PreviousNavState.PreviousNavState;
                return true;
            }

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal class CellViewModel : INotifyPropertyChanged
        {
            public byte Content { get; set; }
            public bool ReadOnly { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}