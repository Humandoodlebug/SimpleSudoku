using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using SC.SimpleSudoku.Model;

// ReSharper disable ExplicitCallerInfoArgument

namespace SC.SimpleSudoku.ViewModels
{
    internal sealed class MainViewModel : INotifyPropertyChanged
    {
        private static readonly User DefaultUser = new User
        {
            Username = "Sign in",
            IsMistakeHighlightingOn = true,
            IsPuzzleTimerVisible = true,
            IsLeaderboardVisible = false
        };

        private NavigationState _currentNavState = new NavigationState();

        private UserViewModel _currentUser = new UserViewModel(DefaultUser);

        private bool _isSignedIn;
        private string _loginErrorMessage;

        private OptionsViewModel _options = new OptionsViewModel(DefaultUser
            /*TODO: Pass in the currently signed in user.*/);

        private string _enteredUsername;
        private string _enteredPassword;

        public MainViewModel()
        {
#if DEBUG
            if (DesignMode.DesignModeEnabled)
                return;
#endif
            Database = new SudokuDataContext();
            Application.Current.Suspending += OnSuspending;
        }

        private SudokuDataContext Database { get; }

        public string EnteredUsername
        {
            get { return _enteredUsername; }
            set
            {
                if (value == _enteredUsername)
                    return;
                _enteredUsername = value; 
                OnPropertyChanged();
                
            }
        }

        public string EnteredPassword
        {
            get { return _enteredPassword; }
            set
            {
                if (value == _enteredPassword)
                    return;
                _enteredPassword = value;
                OnPropertyChanged();
                
            }
        }

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

        public ICommand SignOutCommand => new DelegateCommand(obj => SignOut());

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnSuspending(object o, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            Database.SaveChanges();
            Database.Dispose();
            deferral.Complete();
        }

        private void SignOut()
        {
            IsSignedIn = false;
            Database.SaveChangesAsync();
            CurrentUser = new UserViewModel(DefaultUser);
            Options = new OptionsViewModel(DefaultUser);
        }

        private async void SignUp()
        {
            if ((EnteredUsername.Length < 3) || (EnteredUsername.Length > 20))
                LoginErrorMessage = "The username must be between 3 and 20 characters.";
            else if (
                Database.Users.Any(
                    x => string.Equals(x.Username, EnteredUsername, StringComparison.CurrentCultureIgnoreCase)))
                LoginErrorMessage = "Sorry, that username is already taken. Please try something else.";
            else if (string.IsNullOrEmpty(EnteredPassword) || (EnteredPassword.Length < 6) || (EnteredPassword.Length > 42) ||
                     !EnteredPassword.Any(char.IsLower) || !EnteredPassword.Any(char.IsUpper) ||
                     (!EnteredPassword.Any(char.IsDigit) && EnteredPassword.Any(char.IsSymbol)))
                LoginErrorMessage =
                    "The password must be between 6 and 42 characters, containing upper case letters, lower case letters and numbers or symbols.";
            else
            {
                var user = new User
                {
                    Username = EnteredUsername,
                    Password = EnteredPassword
                };
                Database.Users.Add(user);
                await Database.SaveChangesAsync();
                SignIn();
            }
        }

        private void SignIn()
        {
            var user =
                Database.Users.FirstOrDefault(
                    x => string.Equals(x.Username, EnteredUsername, StringComparison.CurrentCultureIgnoreCase));
            if (string.IsNullOrEmpty(EnteredUsername))
                LoginErrorMessage = "The username field cannot be left blank.";
            else if ((user == null) || (user.Password != EnteredPassword))
                LoginErrorMessage = "Sorry, this username/password combination is not recognised, please try again.";
            else
            {
                CurrentUser = new UserViewModel(user);
                Options = new OptionsViewModel(user);
                IsSignedIn = true;
                EnteredUsername = string.Empty;
                EnteredPassword = string.Empty;
                LoginErrorMessage = string.Empty;
            }
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

        internal sealed class NavigationState : INotifyPropertyChanged
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

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
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