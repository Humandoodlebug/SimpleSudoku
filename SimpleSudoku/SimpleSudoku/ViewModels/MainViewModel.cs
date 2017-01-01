using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Microsoft.EntityFrameworkCore;
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

        public bool IsContinuePuzzleButtonEnabled => !string.IsNullOrEmpty(CurrentUser.CurrentPuzzleData);

        public PuzzleTimerViewModel PuzzleTimer
        {
            get { return _puzzleTimer; }
            private set
            {
                _puzzleTimer = value;
                OnPropertyChanged();
            }
        }

        private Sudoku CurrentPuzzle { get; set; }

        private byte[,] CurrentPuzzleData
        {
            get
            {
                byte[,] data = new byte[Cells.Count, Cells[0].Count];
                for (var i = 0; i < data.GetLength(0); i++)
                    for (var j = 0; j < data.GetLength(1); j++)
                    {
                        if (Cells[i][j].Content != null)
                            data[i, j] = Cells[i][j].Content.Value;
                        else data[i,j] = 0;
                    }
                return data;
            }
        }

        private ObservableCollection<ObservableCollection<CellViewModel>> _cells;
        private string _changePasswordBox;
        private string _changePasswordBox2;

        private NavigationState _currentNavState;

        private UserViewModel _currentUser = new UserViewModel(DefaultUser);
        private string _enteredPassword;

        private string _enteredUsername;
        private bool _isInPencilMode;

        private bool _isSignedIn;
        private string _loginErrorMessage;

        private OptionsViewModel _options = new OptionsViewModel(DefaultUser);

        private int _selectedColumn = 0;
        private int _selectedRow = -1;
        private int? _enteredSeed;
        private PuzzleTimerViewModel _puzzleTimer;

        public MainViewModel()
        {
            CurrentNavState = new NavigationState();
            CurrentNavState.PropertyChanged += CurrentNavState_PropertyChanged;
#if DEBUG
            if (DesignMode.DesignModeEnabled)
                return;
#endif
            Database = new SudokuDataContext();
            Application.Current.Suspending += OnSuspending;
            Application.Current.Resuming += OnResuming;

            //var cell = new CellViewModel {Content = null};
            //var obscol = new ObservableCollection<CellViewModel> {cell,cell,cell,cell,cell,cell,cell,cell,cell};
            //Cells = new ObservableCollection<ObservableCollection<CellViewModel>>
            //{
            //    obscol,
            //    obscol,
            //    obscol,
            //    obscol,
            //    obscol,
            //    obscol,
            //    obscol,
            //    obscol,
            //    obscol
            //};
        }

        private void CurrentNavState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentNavState.CurrentView))
            {
                if (CurrentNavState.CurrentView == NavigationState.View.Solving)
                    PuzzleTimer?.Start();
                else
                    PuzzleTimer?.Stop();
                //TODO: Handle Saving here.
            }
        }

        public ObservableCollection<ObservableCollection<CellViewModel>> Cells
        {
            get { return _cells; }
            set
            {
                _cells = value;
                OnPropertyChanged();
            }
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

        public ICommand SetValueCommand => new DelegateCommand(obj => SetValue(byte.Parse((string)obj)));

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

        public string ChangePasswordBox
        {
            get { return _changePasswordBox; }
            set
            {
                if (value == _changePasswordBox)
                    return;
                _changePasswordBox = value;
                OnPropertyChanged();
            }
        }

        public string ChangePasswordBox2
        {
            get { return _changePasswordBox2; }
            set
            {
                if (value == _changePasswordBox2)
                    return;
                _changePasswordBox2 = value;
                OnPropertyChanged();
            }
        }

        public ICommand ChangePasswordCommand => new DelegateCommand(obj => ChangePassword());

        public bool IsInPencilMode
        {
            get { return _isInPencilMode; }
            set
            {
                _isInPencilMode = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowPuzzleCommand => new DelegateCommand(obj => GeneratePuzzle((string)obj));

        public int? EnteredSeed
        {
            get { return _enteredSeed; }
            set
            {
                if (_enteredSeed == value)
                    return;
                _enteredSeed = value;
                OnPropertyChanged();
            }
        }

        public ICommand GoHomeCommand => new DelegateCommand(obj => GoHome());

        public ICommand ContinuePuzzleCommand => new DelegateCommand(obj => ContinuePuzzle());

        private void GoHome()
        {
            CurrentNavState.RecordView();
            CurrentNavState.CurrentView = NavigationState.View.MainMenu;
            SavePuzzle();
        }

        private byte[,] LoadPuzzleBytes(string puzzleString)
        {
            var puzzleStrings = puzzleString.Split(' ');
            var puzzleBytes = new byte[9, 9];
            var n = 0;
            for (var i = 0; i < 9; i++)
            {
                for (var j = 0; j < 9; j++)
                {
                    puzzleBytes[i, j] = byte.Parse(puzzleStrings[i][j].ToString());
                    n++;
                }
            }
            return puzzleBytes;
        }

        private void ContinuePuzzle()
        {
            CurrentPuzzle = new Sudoku(Database.BasePuzzles.First(x => x.ID == CurrentUser.CurrentBasePuzzleID), CurrentUser.CurrentPuzzleSeed);
            var puzzleData = LoadPuzzleBytes(CurrentUser.CurrentPuzzleData);
            Cells = new ObservableCollection<ObservableCollection<CellViewModel>>();
            for (var i = 0; i < 9; i++)
            {
                var row = new ObservableCollection<CellViewModel>();
                for (var j = 0; j < 9; j++)
                {
                    byte? content = puzzleData[i, j];
                    if (content == 0)
                        content = null;
                    row.Add(new CellViewModel(Options, i, j, CellSelectedHandler, content));
                }
                Cells.Add(row);
            }
            PuzzleTimer = new PuzzleTimerViewModel(CurrentUser.CurrentUser)
            {
                CurrenTimeSpan = CurrentUser.CurrentSolvingTime
            };
            CurrentNavState.RecordView();
            CurrentNavState.CurrentView = NavigationState.View.Solving;
        }

        private void GeneratePuzzle(string difficulty)
        {
            Base_Puzzle[] basePuzzles;
            switch (difficulty)
            {
                case "Easy":
                    basePuzzles = Database.BasePuzzles.Where(x => x.Difficulty == PuzzleDifficulty.Easy).ToArray();
                    break;
                case "Normal":
                    basePuzzles = Database.BasePuzzles.Where(x => x.Difficulty == PuzzleDifficulty.Medium).ToArray();
                    break;
                case "Hard":
                    basePuzzles = Database.BasePuzzles.Where(x => x.Difficulty == PuzzleDifficulty.Hard).ToArray();
                    break;
                case "Insane":
                    basePuzzles = Database.BasePuzzles.Where(x => x.Difficulty == PuzzleDifficulty.Insane).ToArray();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var basePuzzle = basePuzzles[new Random().Next(basePuzzles.Length)];
            CurrentPuzzle = new Sudoku(basePuzzle, EnteredSeed);
            if (!Database.Puzzles.Any(x => x.BasePuzzleID == basePuzzle.ID && x.Seed == CurrentPuzzle.Seed))
            {
                Database.Puzzles.Add(new Puzzle
                {
                    BasePuzzleID = basePuzzle.ID,
                    Difficulty = basePuzzle.Difficulty,
                    PuzzleAttempts = new List<Puzzle_Attempt>(),
                    Seed = CurrentPuzzle.Seed
                });
                Database.SaveChanges();
            }
            PuzzleTimer = new PuzzleTimerViewModel(CurrentUser.CurrentUser);
            DisplayPuzzle();
            PuzzleTimer.Start();
        }

        private void DisplayPuzzle()
        {
            Cells = new ObservableCollection<ObservableCollection<CellViewModel>>();
            for (var i = 0; i < 9; i++)
            {
                var row = new ObservableCollection<CellViewModel>();
                for (var j = 0; j < 9; j++)
                {
                    byte? content = CurrentPuzzle.ProblemData[i, j];
                    if (content == 0)
                        content = null;
                    row.Add(new CellViewModel(Options, i, j, CellSelectedHandler, content));
                }
                Cells.Add(row);
            }
            CurrentNavState.CurrentView = NavigationState.View.Solving;
        }

        private void CellSelectedHandler(object sender, int row, int column)
        {
            //TODO: Handle cell selection event here
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetValue(byte value)
        {
            if (_selectedRow == -1 || _selectedColumn == -1)
                return;
            Cells[_selectedRow][_selectedColumn].Content = value;
            
        }

        private void ChangePassword()
        {
            if ((ChangePasswordBox == ChangePasswordBox2) && (ChangePasswordBox.Length >= 6) &&
                (ChangePasswordBox.Length <= 42) && ChangePasswordBox.Any(char.IsUpper) &&
                ChangePasswordBox.Any(char.IsLower) &&
                (ChangePasswordBox.Any(char.IsDigit) || ChangePasswordBox.Any(char.IsSymbol)))
            {
                CurrentUser.Password = ChangePasswordBox;
                Database.SaveChangesAsync();
            }
        }

        public void SavePuzzle()
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < 9; i++)
            {
                for (var j = 0; j < 9; j++)
                {
                    stringBuilder.Append(CurrentPuzzleData[i, j]);
                }
                if (i < 8)
                    stringBuilder.Append(' ');
            }
            CurrentUser.CurrentPuzzleSeed = CurrentPuzzle.Seed;
            CurrentUser.CurrentBasePuzzleID = CurrentPuzzle.BasePuzzle.ID;
            CurrentUser.CurrentPuzzleData = stringBuilder.ToString();
            CurrentUser.CurrentSolvingTime = PuzzleTimer.CurrenTimeSpan;
            Database.SaveChanges();
            OnPropertyChanged(nameof(IsContinuePuzzleButtonEnabled));
        }

        //The Universal Windows Platform uses a 'Suspend API', meaning the app is suspended when minimized, or before closing. This method is called whenever that happens. 
        private void OnSuspending(object o, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral(); //Requests that the app suspension operation be delayed.
            if (CurrentNavState.CurrentView == NavigationState.View.Solving)
                SavePuzzle(); //Saves puzzle if user is currently solving one.
            Database.SaveChanges();
            Database.Database.CloseConnection(); //Closes the connection to the database.
            deferral.Complete();
        }

        //This method is called when the app is restored after being suspended.
        private void OnResuming(object sender, object e)
        {
            //Re-opens the connection to the database.
            Database.Database.OpenConnection();
        }

        private void SignOut()
        {
            IsSignedIn = false;
            Database.SaveChangesAsync();
            CurrentUser = new UserViewModel(DefaultUser);
            Options = new OptionsViewModel(DefaultUser);
            OnPropertyChanged(nameof(IsContinuePuzzleButtonEnabled));
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
            OnPropertyChanged(nameof(IsContinuePuzzleButtonEnabled));
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
    }
}