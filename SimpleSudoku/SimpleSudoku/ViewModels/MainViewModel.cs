using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SC.SimpleSudoku.Model;
using static SC.SimpleSudoku.App;

// ReSharper disable ExplicitCallerInfoArgument

namespace SC.SimpleSudoku.ViewModels
{
    internal sealed class MainViewModel : INotifyPropertyChanged
    {
        private NavigationState _currentNavState = new NavigationState();

        private UserViewModel _currentUser = new UserViewModel(new User
        {
            Username = "Sign in",
            IsMistakeHighlightingOn = true,
            IsPuzzleTimerVisible = true,
            IsLeaderboardVisible = false
        });

        private OptionsViewModel _options = new OptionsViewModel(new User() /*TODO: Pass in the currently signed in user.*/);
        private bool _isSignedIn;
        private string _loginErrorMessage;

        public string EnteredUsername { get; set; }
        public string EnteredPassword { get; set; }

        public OptionsViewModel Options
        {
            get { return _options; }
            set
            {
                if (_options == value)
                    return;
                _options = value;
                OnPropertyChanged();
            }
        }

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

        public UserViewModel CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        public bool IsSignedIn
        {
            get { return _isSignedIn; }
            private set
            {
                if (_isSignedIn == value)
                    return;
                _isSignedIn = value;
                OnPropertyChanged();
            }
        }

        public ICommand NewPuzzleCommand => new DelegateCommand(obj => GotoNewPuzzle());

        public ICommand OptionsCommand => new DelegateCommand(obj => GotoOptions());

        public ICommand SignInCommand => new DelegateCommand(obj => SignIn());
        public ICommand SignUpCommand => new DelegateCommand(obj => SignUp());

        private void SignUp()
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string LoginErrorMessage
        {
            get { return _loginErrorMessage; }
            private set
            {
                if (_loginErrorMessage == value)
                    return;
                _loginErrorMessage = value;
                OnPropertyChanged();
            }
        }

        private void SignIn()
        {
                var user = Database.Users.FirstOrDefault(x => string.Equals(x.Username, EnteredUsername, StringComparison.CurrentCultureIgnoreCase));
                if (user == null)
                {
                    LoginErrorMessage = "Sorry, this username/password combination is not recognised, please try again.";
                    return;
                }
                if (user.Password != EnteredPassword)
                {
                LoginErrorMessage = "Sorry, this username/password combination is not recognised, please try again.";
                return;
                }
                CurrentUser = new UserViewModel(user);
                IsSignedIn = true;
            EnteredUsername = string.Empty;
            EnteredPassword = string.Empty;
            LoginErrorMessage = string.Empty;
        }

        private void GotoOptions()
        {
            CurrentNavState.RecordView();
            CurrentNavState.CurrentView = NavigationState.View.Options;
        }

        private void GotoNewPuzzle()
        {
            CurrentNavState.RecordView();
            CurrentNavState.CurrentView = NavigationState.View.PuzzleDifficulty;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal class NavigationState : INotifyPropertyChanged
        {
            public enum View
            {
                MainMenu,
                PuzzleDifficulty,
                Solving,
                Options
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
                    OnPropertyChanged(nameof(IsOptionsVisible));
                }
            }

            public bool IsMainMenuVisible => CurrentView == View.MainMenu;
            public bool IsPuzzleDifficultyVisible => CurrentView == View.PuzzleDifficulty;
            public bool IsSolvingVisible => CurrentView == View.Solving;
            public bool IsOptionsVisible => CurrentView == View.Options;

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
                PreviousNavState = new NavigationState {PreviousNavState = PreviousNavState, CurrentView = CurrentView};
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