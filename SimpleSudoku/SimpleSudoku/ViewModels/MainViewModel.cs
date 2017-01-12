using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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

        private ObservableCollection<ObservableCollection<CellViewModel>> _cells;
        private string _changePasswordBox1;
        private string _changePasswordBox2;

        private NavigationState _currentNavState;

        private UserViewModel _currentUser = new UserViewModel(DefaultUser);
        private string _enteredPassword;
        private int? _enteredSeed;

        private string _enteredUsername;
        private bool _isInPencilMode;

        private bool _isSignedIn;
        private string _loginErrorMessage;

        private OptionsViewModel _options = new OptionsViewModel(DefaultUser);
        private Puzzle_Attempt _previousAttempt;
        private List<Puzzle_Attempt> _puzzleAttempts;
        private PuzzleTimerViewModel _puzzleTimer;

        private int _selectedColumn = -1;
        private int _selectedRow = -1;
        private User[] _user;
        private string _changePasswordErrorMessage;

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

        public Sudoku CurrentPuzzle { get; set; }

        private byte[,] CurrentPuzzleData
        {
            get
            {
                var data = new byte[Cells.Count, Cells[0].Count];
                for (var i = 0; i < data.GetLength(0); i++)
                for (var j = 0; j < data.GetLength(1); j++)
                    if (Cells[i][j].Content != null)
                        data[i, j] = Cells[i][j].Content.Value;
                    else data[i, j] = 0;
                return data;
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

        public ICommand SetValueCommand => new DelegateCommand(obj => SetValue(byte.Parse((string) obj)));

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

        public string ChangePasswordBox1
        {
            get { return _changePasswordBox1; }
            set
            {
                if (value == _changePasswordBox1)
                    return;
                _changePasswordBox1 = value;
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

        public ICommand ShowPuzzleCommand => new DelegateCommand(obj => GeneratePuzzle((string) obj));

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

        public ICommand RevealCommand => new DelegateCommand(obj => Reveal());

        public User[] UserLeaderboard
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        public List<Puzzle_Attempt> PuzzleAttempts
        {
            get { return _puzzleAttempts; }
            private set
            {
                _puzzleAttempts = value;
                OnPropertyChanged();
            }
        }

        public Puzzle_Attempt PreviousAttempt
        {
            get { return _previousAttempt; }
            set
            {
                _previousAttempt = value;
                OnPropertyChanged();
            }
        }

        public ICommand ShowUserLeaderboardCommand => new DelegateCommand(obj => ShowUserLeaderboard());

        public string ChangePasswordErrorMessage
        {
            get { return _changePasswordErrorMessage; }
            private set
            {
                if (value == _changePasswordErrorMessage)
                    return;
                _changePasswordErrorMessage = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ShowUserLeaderboard()
        {
            //Select the users from the database who have completed at least 3 puzzles, ordering them by score in descending order:
            var leaderboard = from user in Database.Users
                where user.NumPuzzlesSolved > 2
                orderby user.AverageScore descending
                select user;
            //Put the users into the public 'UserLeaderboard' property to be displayed onscreen:
            UserLeaderboard = leaderboard.ToArray();
            //Show the leaderboard:
            CurrentNavState.RecordView();
            CurrentNavState.CurrentView = NavigationState.View.UserLeaderboard;
        }

        private void CurrentNavState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentNavState.CurrentView))
                if (CurrentNavState.CurrentView == NavigationState.View.Solving)
                    PuzzleTimer?.Start();
                else
                    PuzzleTimer?.Stop();
        }

        /// <summary>
        /// Reveals the correct answer for the selected cell.
        /// </summary>
        private void Reveal()
        {
            if (_selectedRow == -1 || _selectedColumn == -1)
                return;
            Cells[_selectedRow][_selectedColumn].IsWrong = true;
            Cells[_selectedRow][_selectedColumn].Content = CurrentPuzzle.SolutionData[_selectedRow, _selectedColumn];
            Cells[_selectedRow][_selectedColumn].IsWrong = false;
            Cells[_selectedRow][_selectedColumn].IsReadOnly = true;
            _selectedRow = -1;
            _selectedColumn = -1;
            if (Cells.All(x => x.All(y => !y.IsWrong && y.Content != null)))
                CompletePuzzle();
        }

        /// <summary>
        /// Displays the main menu.
        /// </summary>
        private void GoHome()
        {
            CurrentNavState.RecordView();
            var previousView = CurrentNavState.CurrentView;
            CurrentNavState.CurrentView = NavigationState.View.MainMenu;
            if (previousView == NavigationState.View.Solving)
            {
                SavePuzzle();
            }
            else if (previousView == NavigationState.View.PuzzleLeaderboard)
            {
                CurrentUser.CurrentPuzzleData = null;
                OnPropertyChanged(nameof(IsContinuePuzzleButtonEnabled));
                Database.SaveChanges();
            }
        }

        private byte[,] LoadPuzzleBytes(string puzzleString)
        {
            var puzzleStrings = puzzleString.Split(' ');
            var puzzleBytes = new byte[9, 9];
            for (var i = 0; i < 9; i++)
            for (var j = 0; j < 9; j++)
            {
                puzzleBytes[i, j] = byte.Parse(puzzleStrings[i][j].ToString());
            }
            return puzzleBytes;
        }

        private void ContinuePuzzle()
        {
            _selectedRow = -1;
            _selectedColumn = -1;
            CurrentPuzzle =
                new Sudoku(Database.BasePuzzles.Single(x => x.BasePuzzleID == CurrentUser.CurrentBasePuzzleID),
                    CurrentUser.CurrentPuzzleSeed);
            var puzzleData = LoadPuzzleBytes(CurrentUser.CurrentPuzzleData);
            Cells = new ObservableCollection<ObservableCollection<CellViewModel>>();
            if (CurrentUser.CurrentUser.CurrentPuzzleMistakes == null)
                CurrentUser.CurrentUser.CurrentPuzzleMistakes = new List<Mistake>();
            for (var i = 0; i < 9; i++)
            {
                var row = new ObservableCollection<CellViewModel>();
                for (var j = 0; j < 9; j++)
                {
                    byte? content = puzzleData[i, j];
                    var isReadOnly = true;
                    if (content == 0)
                        content = null;
                    var isWrong = false;
                    if (CurrentPuzzle.ProblemData[i, j] == 0)
                    {
                        isReadOnly = false;
                        if (content != null && CurrentPuzzle.SolutionData[i, j] != content.Value)
                            isWrong = true;
                    }
                    row.Add(new CellViewModel(Options, i, j, CellSelectedHandler, content, isReadOnly, Database.Mistakes,
                        isWrong, CurrentUser.Username, Database));
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
            BasePuzzle[] basePuzzles;
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

            if (EnteredSeed == null)
                EnteredSeed = new Random().Next();

            var basePuzzle = basePuzzles[EnteredSeed.Value % basePuzzles.Length];
            CurrentPuzzle = new Sudoku(basePuzzle, EnteredSeed);
            Database.RemoveRange(Database.Mistakes.Where(x => x.Username == CurrentUser.Username));

            if (!Database.Puzzles.Any(x => x.PuzzleSeed == CurrentPuzzle.Seed))
                Database.Puzzles.Add(new Puzzle
                {
                    BasePuzzleId = basePuzzle.BasePuzzleID,
                    PuzzleSeed = CurrentPuzzle.Seed
                });
            PuzzleTimer = new PuzzleTimerViewModel(CurrentUser.CurrentUser);
            DisplayPuzzle();
            CurrentUser.CurrentUser.CurrentPuzzleStartTime = DateTime.Now;
            Database.SaveChanges();
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
                    var isReadOnly = true;
                    byte? content = CurrentPuzzle.ProblemData[i, j];
                    if (content == 0)
                    {
                        content = null;
                        isReadOnly = false;
                    }
                    row.Add(new CellViewModel(Options, i, j, CellSelectedHandler, content, isReadOnly, Database.Mistakes,
                        false, CurrentUser.Username, Database));
                }
                Cells.Add(row);
            }
            CurrentNavState.CurrentView = NavigationState.View.Solving;
        }

        private void CellSelectedHandler(object sender, int row, int column)
        {
            if (Cells[row][column].IsReadOnly)
            {
                Cells[row][column].IsSelected = false;
                return;
            }
            if (_selectedRow != -1 && _selectedColumn != -1)
                Cells[_selectedRow][_selectedColumn].IsSelected = false;
            _selectedRow = row;
            _selectedColumn = column;
            Cells[_selectedRow][_selectedColumn].IsSelected = true;
        }

        private void ShowPuzzleLeaderboard()
        {
            //Get all the puzzle attempts for the current puzzle:
            var allPuzzleAttempts =
                Database.PuzzleAttempts.Where(
                    x => x.PuzzleSeed == CurrentPuzzle.Seed && x.BasePuzzleID == CurrentPuzzle.BasePuzzle.BasePuzzleID);
            var puzzleAttempts = new List<Puzzle_Attempt>();

            //This foreach loop finds each user's best score and adds it to the the PuzzleAttempts list, which the leaderboard binds to.
            foreach (var attempt in allPuzzleAttempts)
            {
                var found = false;
                for (var i = 0; i < puzzleAttempts.Count; i++)
                {
                    if (puzzleAttempts[i].Username != attempt.Username) continue;
                    if (puzzleAttempts[i].Score < attempt.Score)
                        puzzleAttempts[i] = attempt;
                    found = true;
                    break;
                }

                //If the user isn't in the puzzleAttempts list yet, add a new entry for them:
                if (!found)
                    puzzleAttempts.Add(attempt);
            }
            //Set the public property 'PuzzleAttempts', making the list publicly available for the UI to bind to and in descending order of score:
            PuzzleAttempts = puzzleAttempts.OrderByDescending(x => x.Score).ToList();
            //Show the puzzle leaderboard:
            CurrentNavState.CurrentView = NavigationState.View.PuzzleLeaderboard;
        }

        /// <summary>
        ///     Called by the UI when the user presses a number key on the keyboard.
        ///     Sets the currently selected cell's content.
        /// </summary>
        /// <param name="value">The number key that has been pressed.</param>
        public void SetValue(byte value)
        {
            //If no cell is selected, do nothing:
            if (_selectedRow == -1 || _selectedColumn == -1)
                return;
            //If NOT in pencil mode (small numbers for note taking), 
            //hide all small note numbers and set the cells content 
            //to the 'value' parameter (the key that has been pressed):
            if (!IsInPencilMode)
            {
                Cells[_selectedRow][_selectedColumn].Is1Visible = false;
                Cells[_selectedRow][_selectedColumn].Is2Visible = false;
                Cells[_selectedRow][_selectedColumn].Is3Visible = false;
                Cells[_selectedRow][_selectedColumn].Is4Visible = false;
                Cells[_selectedRow][_selectedColumn].Is5Visible = false;
                Cells[_selectedRow][_selectedColumn].Is6Visible = false;
                Cells[_selectedRow][_selectedColumn].Is7Visible = false;
                Cells[_selectedRow][_selectedColumn].Is8Visible = false;
                Cells[_selectedRow][_selectedColumn].Is9Visible = false;

                Cells[_selectedRow][_selectedColumn].Content = value;
                //Check if the user has entered the correct answer. If not, highlight the cell red:
                Cells[_selectedRow][_selectedColumn].IsWrong = value !=
                                                               CurrentPuzzle.SolutionData[_selectedRow, _selectedColumn];
                //If all cells have been filled in correctly, end the puzzle:
                if (Cells.All(x => x.All(y => !y.IsWrong && y.Content != null)))
                    CompletePuzzle();
            }
            else //Runs if pencil mode is on. Toggles the visibility of the
                //relevant note number, depending on the key pressed.
            {
                //Make sure the cell isn't filled in:
                Cells[_selectedRow][_selectedColumn].Content = null;
                Cells[_selectedRow][_selectedColumn].IsWrong = false;
                //Toggle the visibility of the relevant note number:
                switch (value)
                {
                    case 1:
                        Cells[_selectedRow][_selectedColumn].Is1Visible =
                            !Cells[_selectedRow][_selectedColumn].Is1Visible;
                        break;
                    case 2:
                        Cells[_selectedRow][_selectedColumn].Is2Visible =
                            !Cells[_selectedRow][_selectedColumn].Is2Visible;
                        break;
                    case 3:
                        Cells[_selectedRow][_selectedColumn].Is3Visible =
                            !Cells[_selectedRow][_selectedColumn].Is3Visible;
                        break;
                    case 4:
                        Cells[_selectedRow][_selectedColumn].Is4Visible =
                            !Cells[_selectedRow][_selectedColumn].Is4Visible;
                        break;
                    case 5:
                        Cells[_selectedRow][_selectedColumn].Is5Visible =
                            !Cells[_selectedRow][_selectedColumn].Is5Visible;
                        break;
                    case 6:
                        Cells[_selectedRow][_selectedColumn].Is6Visible =
                            !Cells[_selectedRow][_selectedColumn].Is6Visible;
                        break;
                    case 7:
                        Cells[_selectedRow][_selectedColumn].Is7Visible =
                            !Cells[_selectedRow][_selectedColumn].Is7Visible;
                        break;
                    case 8:
                        Cells[_selectedRow][_selectedColumn].Is8Visible =
                            !Cells[_selectedRow][_selectedColumn].Is8Visible;
                        break;
                    case 9:
                        Cells[_selectedRow][_selectedColumn].Is9Visible =
                            !Cells[_selectedRow][_selectedColumn].Is9Visible;
                        break;
                }
            }
        }

        /// <summary>
        ///     Runs when a puzzle is finished. Saves the puzzle attempt and shows the user the puzzle leaderboard, if they have
        ///     not turned it off in the options.
        /// </summary>
        private void CompletePuzzle()
        {
            //Stop the puzzle timer, since the user has completed the puzzle:
            PuzzleTimer.Stop();
            //Count the number of mistakes the user has made:
            var mistakeCount = Database.Mistakes.Count(x => x.Username == CurrentUser.Username);
            //Create a new puzzle attempt to represent the puzzle the user just solved:
            PreviousAttempt =
                new Puzzle_Attempt
                {
                    DateTimeCompleted = DateTime.Now,
                    SolvingTime = PuzzleTimer.CurrenTimeSpan,
                    MistakeCount = mistakeCount,
                    Username = CurrentUser.Username,
                    PuzzleSeed = CurrentPuzzle.Seed,
                    Score =
                        (int)
                        (1000 * ((int) CurrentPuzzle.BasePuzzle.Difficulty + 1) / PuzzleTimer.CurrenTimeSpan.TotalHours *
                         Math.Pow(0.9, mistakeCount)),
                    DateTimeAttempted = CurrentUser.CurrentUser.CurrentPuzzleStartTime,
                    AttemptNum =
                        Database.PuzzleAttempts.Count(
                            x =>
                                x.Username == CurrentUser.Username && x.PuzzleSeed == CurrentPuzzle.Seed &&
                                x.BasePuzzleID == CurrentPuzzle.BasePuzzle.BasePuzzleID),
                    //LINQ longhand version:
                    //AttemptNum = (from x in Database.PuzzleAttempts where x.Username == CurrentUser.Username && x.PuzzleSeed == CurrentPuzzle.Seed && x.BasePuzzleID == CurrentPuzzle.BasePuzzle.BasePuzzleID select x).Count(),
                    BasePuzzleID = CurrentPuzzle.BasePuzzle.BasePuzzleID
                };
            //Add this puzzle attempt to the database:
            Database.PuzzleAttempts.Add(PreviousAttempt);
            //Update the average puzzle difficulty for the user:
            CurrentUser.AveragePuzzleDifficulty = (CurrentUser.AveragePuzzleDifficulty * CurrentUser.NumPuzzlesSolved +
                                                   (double) CurrentPuzzle.BasePuzzle.Difficulty) /
                                                  (CurrentUser.NumPuzzlesSolved + 1);
            //Update the user's average score:
            CurrentUser.AverageScore = (CurrentUser.AverageScore * CurrentUser.NumPuzzlesSolved +
                                        PreviousAttempt.Score) /
                                       (CurrentUser.NumPuzzlesSolved + 1);
            //Update the user's average solving time:
            CurrentUser.AverageSolvingTime =
                TimeSpan.FromSeconds((CurrentUser.AverageSolvingTime.TotalSeconds * CurrentUser.NumPuzzlesSolved +
                                      PreviousAttempt.SolvingTime.TotalSeconds) / (CurrentUser.NumPuzzlesSolved + 1));
            //Update the user's total score:
            CurrentUser.TotalScore += PreviousAttempt.Score;
            //Add 1 to the user's total number of puzzles solved:
            CurrentUser.NumPuzzlesSolved++;
            //Save all changes:
            Database.SaveChanges();
            //Show the puzzle leaderboard if the user has elected to see it in the options:
            if (Options.IsLeaderboardVisible)
                ShowPuzzleLeaderboard();
        }

        /// <summary>
        ///     Changes the currently signed in user's password.
        /// </summary>
        private void ChangePassword()
        {
            if (ChangePasswordBox1 != ChangePasswordBox2)
            {
                ChangePasswordErrorMessage = "Sorry, these passwords don't match!\rPlease try again.";
                return;
            }

            if (!IsPasswordValid(ChangePasswordBox1))
            {
                ChangePasswordErrorMessage =
                    "The password must be between 6 and 42 characters,\rcontaining upper case letters, lower case letters and numbers.";
                return;
            }
            if (ChangePasswordBox1 == Encryption.Decrypt(CurrentUser.Password) || Database.OldPasswords.Any(x => x.Username == CurrentUser.Username && Encryption.Decrypt(x.OldPassword) == ChangePasswordBox1))
            {
                ChangePasswordErrorMessage = "You’ve already used that password:\rYou must choose a new password.";
                return;
            }
            Database.Add(new Old_Password {Username = CurrentUser.Username, OldPassword = CurrentUser.Password});
            CurrentUser.Password = Encryption.Encrypt(ChangePasswordBox1);
            ChangePasswordErrorMessage = "Password changed!";
            Database.SaveChanges();
        }

        /// <summary>
        ///     Saves the puzzle currently being solved to the currently signed in user's account.
        /// </summary>
        public void SavePuzzle()
        {
            //Since strings are immutable, it is inefficient to build a string iteratively, so I've used a StringBuilder (mutable), which is basically an array with some extra functionality.
            var stringBuilder = new StringBuilder(90);

            //Iterate through the puzzle grid, saving the contents of each cell.
            for (var i = 0; i < 9; i++)
            {
                for (var j = 0; j < 9; j++)
                    stringBuilder.Append(CurrentPuzzleData[i, j]);
                //Add the content of the cell to end of the stringbuilder.
                if (i < 8)
                    stringBuilder.Append(' ');
                //Put a space between each line to make it easier to load the puzzle back in.
            }
            CurrentUser.CurrentPuzzleSeed = CurrentPuzzle.Seed;
            CurrentUser.CurrentBasePuzzleID = CurrentPuzzle.BasePuzzle.BasePuzzleID;
            CurrentUser.CurrentPuzzleData = stringBuilder.ToString();
            //Creates a string out of the data in the StringBuilder to store in the database.
            CurrentUser.CurrentSolvingTime = PuzzleTimer.CurrenTimeSpan;
            Database.SaveChanges();
            OnPropertyChanged(nameof(IsContinuePuzzleButtonEnabled));
            //Notify the UI that the continue button should now be clickable.
        }

        /// <summary>
        ///     The Universal Windows Platform uses a 'Suspend API', meaning the app is suspended when minimized, or before
        ///     closing. This method is called whenever that happens.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">
        ///     Contains state relevent to the suspension and allows the event handler to request that the suspension
        ///     be deferred for a short period.
        /// </param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            //NOTE: This method is called whenever the app is suspended (minimised) or closed. Since we don't know whether the app will be closed or restored subsequently, we must save all relevant data now!
            var deferral = e.SuspendingOperation.GetDeferral(); //Requests that the app suspension operation be delayed.
            if (CurrentNavState.CurrentView == NavigationState.View.Solving)
                SavePuzzle(); //Saves puzzle if user is currently solving one.
            Database.SaveChanges();
            Database.Database.CloseConnection(); //Closes the connection to the database.
            deferral.Complete();
        }

        /// <summary>
        ///     This method is called when the app is restored after being suspended.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">An extra object to pass in arguments pertaining to the circumstances of the event. Not usually used.</param>
        private void OnResuming(object sender, object e)
        {
            //Re-opens the connection to the database.
            Database.Database.OpenConnection();
        }

        /// <summary>
        ///     Signs the user out.
        /// </summary>
        private void SignOut()
        {
            IsSignedIn = false;
            Database.SaveChangesAsync();
            //Signs in the 'Default User'. No data persists between sessions when the default user is signed in, since data is not held in the database.
            CurrentUser = new UserViewModel(DefaultUser);
            Options = new OptionsViewModel(DefaultUser);
            //Lets the UI know that the default user might not have a puzzle on-the-go, so the Continue button's state (enable/disabled) should be re-evaluated.
            OnPropertyChanged(nameof(IsContinuePuzzleButtonEnabled));
        }

        /// <summary>
        ///     Adds a new User to the database and signs them in.
        /// </summary>
        private async void SignUp()
        {
            if (string.IsNullOrEmpty(EnteredUsername) || !Regex.IsMatch(EnteredUsername, @"^.{3,20}$"))
            {
                LoginErrorMessage = "The username must be between 3 and 20 characters.";
            }
            else if (
                Database.Users.Any(
                    x => string.Equals(x.Username, EnteredUsername, StringComparison.CurrentCultureIgnoreCase)))
            {
                LoginErrorMessage = "Sorry, that username is already taken. Please try something else.";
            }

            #region Non-Regex Password Validator
            /*else if (string.IsNullOrEmpty(EnteredPassword) || (EnteredPassword.Length < 6) || (EnteredPassword.Length > 42) ||
                         !EnteredPassword.Any(char.IsLower) || !EnteredPassword.Any(char.IsUpper) ||
                         (!EnteredPassword.Any(char.IsDigit) && EnteredPassword.Any(char.IsSymbol)))*/
            #endregion

            else if (!IsPasswordValid(EnteredPassword))
            {
                LoginErrorMessage =
                    "The password must be between 6 and 42 characters, containing upper case letters, lower case letters and numbers or symbols.";
            }
            else
            {
                var user = new User
                {
                    Username = EnteredUsername,
                    Password = Encryption.Encrypt(EnteredPassword)
                };
                Database.Users.Add(user);
                await Database.SaveChangesAsync();
                SignIn();
            }
        }

        private bool IsPasswordValid(string password)
        {
            return (!string.IsNullOrEmpty(password) &&
                   Regex.IsMatch(password, @"^(?=.*(\d|\W))(?=.*[a-z])(?=.*[A-Z])(?!.*\s).{6,40}$"));
        }

        private void SignIn()
        {
            var user =
                Database.Users
                    .SingleOrDefault(
                        x => string.Equals(x.Username, EnteredUsername, StringComparison.CurrentCultureIgnoreCase));
            if (string.IsNullOrEmpty(EnteredUsername))
            {
                LoginErrorMessage = "The username field cannot be left blank.";
            }
            else if (user == null || Encryption.Decrypt(user.Password) != EnteredPassword)
            {
                LoginErrorMessage = "Sorry, this username/password combination is not recognised, please try again.";
            }
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

        /// <summary>
        ///     This is called from the UI Code behind. It changes the selected cell when the user presses the arrow keys.
        /// </summary>
        /// <param name="direction">The direction of the arrow key (Left is 0, Up is 1, Right is 2, Down is 3).</param>
        public void MoveSelection(int direction)
        {
            if (_selectedRow == -1 || _selectedColumn == -1)
                return;
            var originalRow = _selectedRow;
            var originalColumn = _selectedColumn;
            Cells[_selectedRow][_selectedColumn].IsSelected = false;
            var breakOut = false;
            do
            {
                switch (direction)
                {
                    case 0:
                        if (_selectedColumn > 0)
                            _selectedColumn--;
                        else breakOut = true;
                        break;
                    case 1:
                        if (_selectedRow > 0)
                            _selectedRow--;
                        else breakOut = true;
                        break;
                    case 2:
                        if (_selectedColumn < 8)
                            _selectedColumn++;
                        else breakOut = true;
                        break;
                    case 3:
                        if (_selectedRow < 8)
                            _selectedRow++;
                        else breakOut = true;
                        break;
                }
                if (breakOut)
                {
                    _selectedRow = originalRow;
                    _selectedColumn = originalColumn;
                    break;
                }
            } while
                (Cells[_selectedRow][_selectedColumn].IsReadOnly);

            Cells[_selectedRow][_selectedColumn].IsSelected = true;
        }

        internal sealed class NavigationState : INotifyPropertyChanged
        {
            public enum View
            {
                MainMenu,
                PuzzleDifficulty,
                Solving,
                Options,
                PuzzleLeaderboard,
                UserLeaderboard
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
                    OnPropertyChanged(nameof(IsPuzzleLeaderboardVisible));
                    OnPropertyChanged(nameof(IsUserLeaderboardVisible));
                }
            }

            public bool IsMainMenuVisible => CurrentView == View.MainMenu;
            public bool IsPuzzleDifficultyVisible => CurrentView == View.PuzzleDifficulty;
            public bool IsSolvingVisible => CurrentView == View.Solving;
            public bool IsOptionsVisible => CurrentView == View.Options;
            public bool IsPuzzleLeaderboardVisible => CurrentView == View.PuzzleLeaderboard;
            public bool IsUserLeaderboardVisible => CurrentView == View.UserLeaderboard;

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

            /// <summary>
            ///     Called by property setters to notify the UI that the property's value has changed.
            /// </summary>
            /// <param name="propertyName">The name of the property whose value changed.</param>
            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                //Invokes the 'PropertyChanged' event (if it is not null, hence the '?' before 'Invoke') to notify Users 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    internal static class Encryption
    {
        private const int Seed = 592;

        public static string Encrypt(string s)
        {
            var random = new Random(Seed);
            return string.Join("", from c in s select (char)(c + random.Next(-30,30)));
        }

        public static string Decrypt(string s)
        {
            var random = new Random(Seed);
            return string.Join("", from c in s select (char)(c - random.Next(-30,30)));
        }
    }
}