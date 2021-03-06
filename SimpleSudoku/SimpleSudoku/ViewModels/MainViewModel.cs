﻿using System;
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
    /// <summary>
    ///     Contains properties to be bound to by the User Interface in the MainPage.xaml file.
    /// </summary>
    internal sealed class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        ///     The default user. Used when no user is signed in.
        /// </summary>
        private static readonly User DefaultUser = new User
        {
            Username = "Sign in",
            IsMistakeHighlightingOn = true,
            IsPuzzleTimerVisible = true,
            IsLeaderboardVisible = false
        };

        //Backing fields for properties:

        private ObservableCollection<ObservableCollection<CellViewModel>> _cells;
        private string _changePasswordBox1;
        private string _changePasswordBox2;
        private string _changePasswordErrorMessage;

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
        private string _seedErrorMessage;

        /// <summary>
        ///     The MainViewModel constructor. Runs when the app starts.
        /// </summary>
        public MainViewModel()
        {
            //Initialise the navigation state, so that the main menu is displayed.
            CurrentNavState = new NavigationState();

            //Add an event handler to handle when the navigation state changes.
            CurrentNavState.PropertyChanged += CurrentNavState_PropertyChanged;
#if DEBUG //Following if statement runs only in debug mode, to check if the designer is active:
            if (DesignMode.DesignModeEnabled) //If in design mode, stop here.
                return;
#endif
            //If not in design mode, carry on and do the database stuff:

            //Open a connection to the database.
            Database = new SudokuDataContext();

            //Add handlers to the Suspending and Resuming events.
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

        //Enable the continue button if the user has a saved puzzle in the database.
        public bool IsContinuePuzzleButtonEnabled => !string.IsNullOrEmpty(CurrentUser.CurrentPuzzleData);

        /// <summary>
        ///     A simple stopwatch. Records the time elapsed while solving the current puzzle.
        /// </summary>
        public PuzzleTimerViewModel PuzzleTimer
        {
            get { return _puzzleTimer; }
            private set
            {
                _puzzleTimer = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Holds data about the puzzle currently being solved.
        /// </summary>
        private Sudoku CurrentPuzzle { get; set; }

        /// <summary>
        ///     Gets the current puzzle as a simple byte array.
        /// </summary>
        private byte[,] CurrentPuzzleData
        {
            get
            {
                //Iterates through every cell and adds its contents to the correct position in the byte array.
                var data = new byte[Cells.Count, Cells[0].Count];
                for (var i = 0; i < data.GetLength(0); i++)
                for (var j = 0; j < data.GetLength(1); j++)
                    if (Cells[i][j].Content != null)
                        data[i, j] = Cells[i][j].Content.Value;
                    else data[i, j] = 0; //If the content is null, use 0 instead.
                return data;
            }
        }

        /// <summary>
        ///     The current puzzle data. Bound to by the puzzle solving User Interface.
        /// </summary>
        public ObservableCollection<ObservableCollection<CellViewModel>> Cells
        {
            get { return _cells; }
            private set
            {
                _cells = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Stores the current data context for the database.
        /// </summary>
        private SudokuDataContext Database { get; }

        /// <summary>
        ///     Bound to the contents of the username field on the login flyout.
        /// </summary>
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

        /// <summary>
        ///     Bound to the contents of the password field on the login flyout.
        /// </summary>
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

        /// <summary>
        ///     Exposes properties bound to by controls on the options page.
        /// </summary>
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

        /// <summary>
        ///     Exposes information about the current view to the UI, along with the navigation state history to make the back
        ///     button work.
        /// </summary>
        public NavigationState CurrentNavState
        {
            get { return _currentNavState; }
            private set
            {
                if (_currentNavState == value)
                    return;
                _currentNavState = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Exposes information about the currently signed in user to the UI.
        /// </summary>
        public UserViewModel CurrentUser
        {
            get { return _currentUser; }
            private set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Returns true if a user is signed in and false otherwise.
        /// </summary>
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


        //Some commands to trigger methods when events are raised in the UI.
        //See the methods they trigger for comments on their purposes.
        public ICommand NewPuzzleCommand => new DelegateCommand(obj => GotoNewPuzzle());
        public ICommand OptionsCommand => new DelegateCommand(obj => GotoOptions());
        public ICommand SignInCommand => new DelegateCommand(obj => SignIn());
        public ICommand SignUpCommand => new DelegateCommand(obj => SignUp());
        // ReSharper disable once UnusedMember.Global
        public ICommand SetValueCommand => new DelegateCommand(obj => SetValue(byte.Parse((string) obj)));
        public ICommand SignOutCommand => new DelegateCommand(obj => SignOut());
        public ICommand ChangePasswordCommand => new DelegateCommand(obj => ChangePassword());
        public ICommand ShowPuzzleCommand => new DelegateCommand(obj => GeneratePuzzle((string) obj));
        public ICommand GoHomeCommand => new DelegateCommand(obj => GoHome());
        public ICommand ContinuePuzzleCommand => new DelegateCommand(obj => ContinuePuzzle());
        public ICommand RevealCommand => new DelegateCommand(obj => Reveal());
        public ICommand ShowUserLeaderboardCommand => new DelegateCommand(obj => ShowUserLeaderboard());

        /// <summary>
        ///     Exposes the error message to be displayed on the Login flyout. Set to string.Empty or null if no error message is
        ///     displayed.
        /// </summary>
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

        /// <summary>
        ///     Holds the contents of the first text box in the Change Password flyout.
        /// </summary>
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

        /// <summary>
        ///     Holds the contents of the second text box in the Change Password flyout.
        /// </summary>
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

        /// <summary>
        ///     Bound to by the UI. Returns true if pencil mode is active, and false otherwise.
        ///     Toggled by pencil mode (note mode) app bar button.
        /// </summary>
        public bool IsInPencilMode
        {
            get { return _isInPencilMode; }
            set
            {
                _isInPencilMode = value;
                OnPropertyChanged();
            }
        }


        public string SeedErrorMessage
        {
            get { return _seedErrorMessage; }
            private set
            {
                if (value == _seedErrorMessage)
                    return;
                _seedErrorMessage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Bound to by the UI. Converts the seed entered by the user between an integer and a string. Also handles relevent validation errors.
        /// </summary>
        public string EnteredSeedString
        {
            get { return EnteredSeed?.ToString() ?? string.Empty; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    EnteredSeed = null;
                    SeedErrorMessage = string.Empty;
                }
                else
                {
                    int result;
                    SeedErrorMessage = int.TryParse(value, out result)
                        ? string.Empty
                        : "Sorry, the seed you've entered isn't a valid integer! Please try again...";
                    if (string.IsNullOrEmpty(SeedErrorMessage))
                        EnteredSeed = result;
                }
            }
        }

        /// <summary>
        ///      Holds the seed entered by the user when generating a puzzle.
        /// </summary>
        private int? EnteredSeed //The ? means 'nullable'.
        {
            get { return _enteredSeed; }
            set
            {
                if (_enteredSeed == value)
                    return;
                _enteredSeed = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EnteredSeedString));
            }
        }

        /// <summary>
        ///     Bound to by the UI. Stores an array of users to be displayed on the user leaderboard.
        /// </summary>
        public User[] UserLeaderboard
        {
            get { return _user; }
            private set
            {
                _user = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Bound to by the UI. Stores a list of attempts for the current puzzle.
        ///     Displayed on the puzzle leaderboard.
        /// </summary>
        public List<Puzzle_Attempt> PuzzleAttempts
        {
            get { return _puzzleAttempts; }
            private set
            {
                _puzzleAttempts = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Bound to by the UI. Holds the puzzle attempt that has just been completed (if any, null otherwise).
        /// </summary>
        public Puzzle_Attempt PreviousAttempt
        {
            get { return _previousAttempt; }
            private set
            {
                _previousAttempt = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Holds the error message, if any, to be displayed in the Change Password flyout.
        ///     Set to null or string.Empty if there is no error message to be displayed.
        /// </summary>
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

        /// <summary>
        ///     An event Subscribed to by event handlers in the UWP UI Framework.
        ///     Invoked when a bound property changes through the OnPropertyChanged() procedure.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Generates and displays the User Leaderboard when the 'Leaderboard' button is clicked.
        /// </summary>
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

        /// <summary>
        ///     Subscribed to the PropertyChanged event in the constructor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentNavState_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //If the property that has changed isn't the CurrentView property, do nothing.
            if (e.PropertyName != nameof(CurrentNavState.CurrentView)) return;
            //If the user has just started/continued solving a puzzle, start the puzzle timer.
            if (CurrentNavState.CurrentView == NavigationState.View.Solving)
                PuzzleTimer?.Start();
            else //Otherwise, stop the puzzle timer.
                PuzzleTimer?.Stop();
        }

        /// <summary>
        ///     Reveals the correct answer for the selected cell.
        ///     Triggered when the 'Reveal' button is pressed.
        /// </summary>
        private void Reveal()
        {
            //If no row is selected, do nothing.
            if (_selectedRow == -1 || _selectedColumn == -1)
                return;
            //Set IsWrong to true to ensure the cell is on the mistakes list.
            Cells[_selectedRow][_selectedColumn].IsWrong = true;
            //Set the selected cell's content to the byte in the cell's position in the solution data, revealig the correct answer.
            Cells[_selectedRow][_selectedColumn].Content = CurrentPuzzle.SolutionData[_selectedRow, _selectedColumn];
            //Set IsWrong to false to remove red outline.
            Cells[_selectedRow][_selectedColumn].IsWrong = false;
            //Make the cell read only, so it is obvious the correct answer has been revealed and cannot be changed.
            Cells[_selectedRow][_selectedColumn].IsReadOnly = true;
            //Deselect the cell.
            _selectedRow = -1;
            _selectedColumn = -1;
            //If all cells are filled in correctly, end the game.
            if (Cells.All(x => x.All(y => !y.IsWrong && y.Content != null)))
                CompletePuzzle();
        }

        /// <summary>
        ///     Displays the main menu when the Home app bar button is clicked.
        /// </summary>
        private void GoHome()
        {
            //Record the current navigation state, so it can be restored with the back button.
            CurrentNavState.RecordView();

            var previousView = CurrentNavState.CurrentView;
            //Display the Main Menu.
            CurrentNavState.CurrentView = NavigationState.View.MainMenu;

            //If the user was solving a puzzle then save it.
            if (previousView == NavigationState.View.Solving)
            {
                SavePuzzle();
            }
            //If the user was looking at a leaderboard then clear the current puzzle data, since they have just solved it.
            else if (previousView == NavigationState.View.PuzzleLeaderboard)
            {
                CurrentUser.CurrentPuzzleData = null;
                OnPropertyChanged(nameof(IsContinuePuzzleButtonEnabled));
                Database.SaveChanges();
            }
        }

        /// <summary>
        ///     Converts the current puzzle from a string stored in the database to a two dimensional array of bytes.
        /// </summary>
        /// <param name="puzzleString">The string to be converted.</param>
        /// <returns>A byte array representing the current puzzle.</returns>
        private byte[,] LoadPuzzleBytes(string puzzleString)
        {
            var puzzleStrings = puzzleString.Split(' ');
            var puzzleBytes = new byte[9, 9];
            for (var i = 0; i < 9; i++)
            for (var j = 0; j < 9; j++)
                puzzleBytes[i, j] = byte.Parse(puzzleStrings[i][j].ToString());
            return puzzleBytes;
        }

        /// <summary>
        ///     Opens the puzzle last saved by the user.
        /// </summary>
        private void ContinuePuzzle()
        {
            //Ensure no cell is selected.
            _selectedRow = -1;
            _selectedColumn = -1;
            //Generate the current puzzle sudoku object.
            CurrentPuzzle =
                new Sudoku(Database.BasePuzzles.Single(x => x.BasePuzzleID == CurrentUser.CurrentBasePuzzleID),
                    CurrentUser.CurrentPuzzleSeed);

            //Load the current puzzle data into the UI.
            var puzzleData = LoadPuzzleBytes(CurrentUser.CurrentPuzzleData);
            Cells = new ObservableCollection<ObservableCollection<CellViewModel>>();
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
            //Create the puzzle timer, setting its timespan to the time already elapsed from the database.
            PuzzleTimer = new PuzzleTimerViewModel(CurrentUser.CurrentUser)
            {
                CurrenTimeSpan = CurrentUser.CurrentSolvingTime
            };
            //Record the current navigation state (for back button functionality) and show the solving page.
            CurrentNavState.RecordView();
            CurrentNavState.CurrentView = NavigationState.View.Solving;
        }

        /// <summary>
        /// Generates a Sudoku puzzle when a difficulty is selected from the 'New Puzzle' menu.
        /// </summary>
        /// <param name="difficulty">The difficulty sudoku puzzle to generate.</param>
        private void GeneratePuzzle(string difficulty)
        {
            //If there is a validation error message still being shown, don't try to generate the puzzle.
            if (!string.IsNullOrEmpty(SeedErrorMessage))
                return;
            BasePuzzle[] basePuzzles;
            //Get a list of base puzzles of difficulty selected by the user.
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

            //If no seed is entered, generate a random seed.
            if (EnteredSeed == null)
                EnteredSeed = new Random().Next();

            //Select a base puzzle from the retrieved list of base puzzles from the database.
            var basePuzzle = basePuzzles[EnteredSeed.Value % basePuzzles.Length];
            //Set the current puzzle using the obtained parameters (seed, base puzzle).
            CurrentPuzzle = new Sudoku(basePuzzle, EnteredSeed);
            //Clear the database of mistakes made in any previous puzzle by the current user.
            Database.RemoveRange(Database.Mistakes.Where(x => x.Username == CurrentUser.Username));

            //If a puzzle with this seed isn't already in the database, add it.
            if (!Database.Puzzles.Any(x => x.PuzzleSeed == CurrentPuzzle.Seed))
                Database.Puzzles.Add(new Puzzle
                {
                    BasePuzzleId = basePuzzle.BasePuzzleID,
                    PuzzleSeed = CurrentPuzzle.Seed
                });
            //Create a puzzle timer to keep track of elapsed solving time.
            PuzzleTimer = new PuzzleTimerViewModel(CurrentUser.CurrentUser);
            //Display the puzzle solving UI.
            DisplayPuzzle();
            //Record the time the user started the puzzle in the database.
            CurrentUser.CurrentUser.CurrentPuzzleStartTime = DateTime.Now;
            Database.SaveChanges();
            //Start the puzzle timer.
            PuzzleTimer.Start();
        }

        /// <summary>
        /// Displays the puzzle on the solving page for the user to start solving.
        /// </summary>
        private void DisplayPuzzle()
        {
            //Clear the Cells collection for a new puzzle to be added.
            Cells = new ObservableCollection<ObservableCollection<CellViewModel>>();
            //Iterate through all 9 rows of the puzzle
            for (var i = 0; i < 9; i++)
            {
                //Create a new row.
                var row = new ObservableCollection<CellViewModel>();
                //Iterate through all 9 columns of the puzzle.
                for (var j = 0; j < 9; j++)
                {
                    //Create a cell based on the puzzle problem data.
                    var isReadOnly = true;
                    byte? content = CurrentPuzzle.ProblemData[i, j];
                    if (content == 0)
                    {
                        content = null;
                        isReadOnly = false;
                    }
                    //Add the cell to the row.
                    row.Add(new CellViewModel(Options, i, j, CellSelectedHandler, content, isReadOnly, Database.Mistakes,
                        false, CurrentUser.Username, Database));
                }
                //Add the row to the Cells collection.
                Cells.Add(row);
            }
            //Switch to the solving page.
            CurrentNavState.CurrentView = NavigationState.View.Solving;
        }

        /// <summary>
        /// Handles the cell selection event.
        /// </summary>
        /// <param name="sender">The cell triggering the cell selection event.</param>
        /// <param name="row">The row of the cell in the puzzle.</param>
        /// <param name="column">The column of the cell in the puzzle.</param>
        private void CellSelectedHandler(object sender, int row, int column)
        {
            //If the cell is read only, deselect it and skip the rest (read only cells cannot be selected).
            if (Cells[row][column].IsReadOnly)
            {
                Cells[row][column].IsSelected = false;
                return;
            }
            //If a cell is already selected then deselect it.
            if (_selectedRow != -1 && _selectedColumn != -1)
                Cells[_selectedRow][_selectedColumn].IsSelected = false;
            //Mark the selected cell as selected.
            _selectedRow = row;
            _selectedColumn = column;
            Cells[_selectedRow][_selectedColumn].IsSelected = true;
        }

        /// <summary>
        /// Generates and displays the puzzle leaderboard after a puzzle has been completed.
        /// </summary>
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
            //If the passwords don't match, display an error message.
            if (ChangePasswordBox1 != ChangePasswordBox2)
            {
                ChangePasswordErrorMessage = "Sorry, these passwords don't match!\rPlease try again.";
            }

            //if the password is not valid, display an error message.
             else if (!IsPasswordValid(ChangePasswordBox1))
            {
                ChangePasswordErrorMessage =
                    "The password must be between 6 and 42 characters,\rcontaining upper case letters, lower case letters and numbers.";
            }
            //If the password entered is the same as the current password or a password previously used by the current user, display an error message.
             else if (ChangePasswordBox1 == Encryption.Decrypt(CurrentUser.Password) ||
                      Database.OldPasswords.Any(
                          x =>
                              x.Username == CurrentUser.Username &&
                              Encryption.Decrypt(x.OldPassword) == ChangePasswordBox1))
            {
                ChangePasswordErrorMessage = "You’ve already used that password:\rYou must choose a new password.";
            }
            //If the password checks out (no error messages are displayed) then continue.
            else
            {
                //Add the current password to the old passwords list
                Database.Add(new Old_Password {Username = CurrentUser.Username, OldPassword = CurrentUser.Password ?? string.Empty});
                //Set the user's password in the database to the newly entered password (encrypted)
                CurrentUser.Password = Encryption.Encrypt(ChangePasswordBox1);
                //Display a helpful message instead of an error.
                ChangePasswordErrorMessage = "Password changed!";
                Database.SaveChanges();
            }
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
            //If the username isn't valid, display an error message.
            if (string.IsNullOrEmpty(EnteredUsername) || !Regex.IsMatch(EnteredUsername, @"^.{3,20}$"))
            {
                LoginErrorMessage = "The username must be between 3 and 20 characters.";
            }
            //If a user with this username already exists in the database, display an error message.
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

            //If the password isn't valid, display an error message.
            else if (string.IsNullOrEmpty(EnteredPassword) || !IsPasswordValid(EnteredPassword))
            {
                LoginErrorMessage =
                    "The password must be between 6 and 42 characters, containing upper case letters, lower case letters and numbers or symbols.";
            }
            //Create a new user account with the specified username and password, adding it to the database.
            else
            {
                var user = new User
                {
                    Username = EnteredUsername,
                    Password = Encryption.Encrypt(EnteredPassword)
                };
                Database.Users.Add(user);
                await Database.SaveChangesAsync();
                //Sign the user in.
                SignIn();
            }
        }

        /// <summary>
        /// Checks if a password is valid.
        /// </summary>
        /// <param name="password">The password to be tested.</param>
        /// <returns>Returns true if the password is valid, false otherwise.</returns>
        private bool IsPasswordValid(string password)
        {
            return !string.IsNullOrEmpty(password) &&
                   Regex.IsMatch(password, @"^(?=.*(\d|\W))(?=.*[a-z])(?=.*[A-Z])(?!.*\s).{6,42}$");
        }

        /// <summary>
        /// Signs a user in. Triggered when a user selects the 'sign in' button on the 'sign in' flyout.
        /// </summary>
        private void SignIn()
        {
            //Get the user with the entered username from the database (if they exist), ignoring case.
            var user =
                Database.Users
                    .SingleOrDefault(
                        x => string.Equals(x.Username, EnteredUsername, StringComparison.CurrentCultureIgnoreCase));
            //If the username field is blank, display and error message.
            if (string.IsNullOrEmpty(EnteredUsername))
            {
                LoginErrorMessage = "The username field cannot be left blank.";
            }
            //If no user with this username was found in the database or the password entered doesn't match, display an error message.
            else if (user == null || Encryption.Decrypt(user.Password) != EnteredPassword && !string.IsNullOrEmpty(user.Password))
            {
                LoginErrorMessage = "Sorry, this username/password combination is not recognised, please try again.";
            }
            //If the username and password match, sign the user in.
            else
            {
                CurrentUser = new UserViewModel(user);
                Options = new OptionsViewModel(user);
                IsSignedIn = true;
                //Clear the Username, password and error message boxes.
                EnteredUsername = string.Empty;
                EnteredPassword = string.Empty;
                LoginErrorMessage = string.Empty;
            }
            //Re-evaluate whether the current user has a saved puzzle in the database.
            OnPropertyChanged(nameof(IsContinuePuzzleButtonEnabled));
        }

        /// <summary>
        /// Displays the options page.
        /// </summary>
        private void GotoOptions()
        {
            CurrentNavState.RecordView();
            CurrentNavState.CurrentView = NavigationState.View.Options;
        }

        /// <summary>
        /// Displays the New Puzzle menu.
        /// </summary>
        private void GotoNewPuzzle()
        {
            CurrentNavState.RecordView();
            CurrentNavState.CurrentView = NavigationState.View.PuzzleDifficulty;
        }

        /// <summary>
        /// Called when the value of a property changes. Raises the PropertyChanged event to notify the UI that the value of a property it binds to has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property whose value has changed.</param>
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
            //If no cell is selected, do nothing.
            if (_selectedRow == -1 || _selectedColumn == -1)
                return;
            //Record the starting position of the selection.
            var originalRow = _selectedRow;
            var originalColumn = _selectedColumn;
            //Deselect the currently selected cell.
            Cells[_selectedRow][_selectedColumn].IsSelected = false;
            var breakOut = false;
            do
            {
                //Move the selection in the direction of the arrow key pressed.
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
                //If the selection has moved off the edge of the puzzle grid, move it back to where it started.
                if (breakOut)
                {
                    _selectedRow = originalRow;
                    _selectedColumn = originalColumn;
                    break;
                }
            //If the Cell selected is read only, move another cell in the same direction.
            } while
                (Cells[_selectedRow][_selectedColumn].IsReadOnly);
            //Mark the selected cell as selected.
            Cells[_selectedRow][_selectedColumn].IsSelected = true;
        }

        /// <summary>
        /// Holds the navigation state of the app (which page it is on) and exposes boolean visibilities for the UI to bind to.
        /// Also handles the back butto functionality.
        /// </summary>
        internal sealed class NavigationState : INotifyPropertyChanged
        {
            /// <summary>
            /// Allows for identification of each of the possible views/pages in the app.
            /// </summary>
            public enum View
            {
                MainMenu,
                PuzzleDifficulty,
                Solving,
                Options,
                PuzzleLeaderboard,
                UserLeaderboard
            }

            //Private backing fields.
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
                    //Also Mark all the properties this may affect as changed.
                    OnPropertyChanged(nameof(IsMainMenuVisible));
                    OnPropertyChanged(nameof(IsPuzzleDifficultyVisible));
                    OnPropertyChanged(nameof(IsSolvingVisible));
                    OnPropertyChanged(nameof(IsOptionsVisible));
                    OnPropertyChanged(nameof(IsPuzzleLeaderboardVisible));
                    OnPropertyChanged(nameof(IsUserLeaderboardVisible));
                }
            }
            //A group of properties indicating the visibility of each page in the app:
            public bool IsMainMenuVisible => CurrentView == View.MainMenu;
            public bool IsPuzzleDifficultyVisible => CurrentView == View.PuzzleDifficulty;
            public bool IsSolvingVisible => CurrentView == View.Solving;
            public bool IsOptionsVisible => CurrentView == View.Options;
            public bool IsPuzzleLeaderboardVisible => CurrentView == View.PuzzleLeaderboard;
            public bool IsUserLeaderboardVisible => CurrentView == View.UserLeaderboard;

            //Holds the previous navigation state, for back button functionality.
            public NavigationState PreviousNavState
            {
                get { return _previousNavState; }
                private set
                {
                    if (_previousNavState == value)
                        return;
                    _previousNavState = value;
                    OnPropertyChanged();
                }
            }
            /// <summary>
            /// Raised when a property changes in this class. Mainly for properties bound to the UI.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Records the current navigation state.
            /// Should be called before changing the current view/page for back button functionality.
            /// </summary>
            public void RecordView()
            {
                PreviousNavState = new NavigationState {PreviousNavState = PreviousNavState, CurrentView = CurrentView};
            }

            /// <summary>
            /// Reverts the navigation state to its previous state (goes back to the previous page).
            /// </summary>
            /// <returns>Returns true if the app was able to go back and false otherwise.</returns>
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
                //Invokes the 'PropertyChanged' event (if it is not null, hence the '?' before '.Invoke') to notify the UI that the property with name propertyName has changed.
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// A class to handle database password encryption.
    /// </summary>
    internal static class Encryption
    {
        private const int Seed = 592;

        /// <summary>
        /// Encrypts the password to be stored in the database.
        /// </summary>
        /// <param name="s">The passwor to encrypt.</param>
        /// <returns>The encrypted password.</returns>
        public static string Encrypt(string s)
        {
            var random = new Random(Seed);
            return string.Join("", from c in s select (char) (c + random.Next(-30, 30)));
        }

        /// <summary>
        /// Deecrypts the password to be read from the database.
        /// </summary>
        /// <param name="s">The password to decrypt.</param>
        /// <returns>The decrypted password.</returns>
        public static string Decrypt(string s)
        {
            if (s == null)
                return string.Empty;
            var random = new Random(Seed);
            return string.Join("", from c in s select (char) (c - random.Next(-30, 30)));
        }
    }
}