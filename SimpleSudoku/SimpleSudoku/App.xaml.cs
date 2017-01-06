using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.EntityFrameworkCore;
using SC.SimpleSudoku.Model;
using SC.SimpleSudoku.Views;

namespace SC.SimpleSudoku
{
    /// <summary>
    ///     Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        ///     Initializes the singleton application object.  This is the first line of authored code
        ///     executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            if (Database == null)
                Database = new SudokuDataContext();
            try
            {
                Database.Database.Migrate();
            }
#if DEBUG
            catch (Exception e)
            {
                //Handling invalid database when debugging.
                //Next 6 lines only run if there is an Exception accessing the database when debugging.
                Debug.WriteLine($"Database Migration error: \"{e}\"");
                Debug.WriteLine("Deleting the database...");
                Database.Database.EnsureDeleted();
                Debug.WriteLine("Database deleted. Retrying migration...");
                Database.Database.Migrate();
                Debug.WriteLine("Migration successful.");
            }
#else
            //These lines run if there is an Exception accessing the database in the final release. They show the error message and close the app when the user clicks 'ok'.
            catch(IOException e)
            {
                var messageDialog = new MessageDialog($"There has been an error accessing the database: {e.Message}\n\n\r The app will now close.",
                    "Database Error");
                messageDialog.Commands.Add(new UICommand("Ok", command => Current.Exit()));
            }
#endif

#if DEBUG
            if (!Database.Users.Any(x => x.Username == "TestUser"))
                Database.Users.Add(new User
                {
                    Username = "TestUser",
                    Password = "1234"
                });
#endif
            if (!Database.Users.Any(x => x.Username == "Sign in"))
                Database.Users.Add(new User {Username = "Sign in", Password = null});
            if (!Database.BasePuzzles.Any())
            {
                Database.BasePuzzles.Add(new BasePuzzle
                {
                    Difficulty = PuzzleDifficulty.Easy,
                    PuzzleProblemData =
                        "209003718 000700000 000006059 700400000 540000096 000002007 810200000 000001000 356900801",
                    PuzzleSolutionData =
                        "269543718 135798642 478126359 782469135 543817296 691352487 817235964 924681573 356974821"
                });
                Database.BasePuzzles.Add(new BasePuzzle
                {
                    Difficulty = PuzzleDifficulty.Medium,
                    PuzzleProblemData =
                        "100009060 230074010 090000000 020030000 801090603 000010020 000000030 060180092 070600004",
                    PuzzleSolutionData =
                        "184529367 236874915 795361248 629738451 841295673 357416829 518942736 463187592 972653184"
                });
                Database.BasePuzzles.Add(new BasePuzzle
                {
                    Difficulty = PuzzleDifficulty.Hard,
                    PuzzleProblemData =
                        "030000400 100700603 400050097 000007100 000402000 009800000 360070001 504003009 008000070",
                    PuzzleSolutionData =
                        "837269415 195748623 426351897 253697148 781432956 649815732 362974581 574183269 918526374"
                });
                Database.BasePuzzles.Add(new BasePuzzle
                {
                    Difficulty = PuzzleDifficulty.Insane,
                    PuzzleProblemData =
                        "000700080 705000000 090600705 000042610 000561000 013980000 201009030 000000902 040005000",
                    PuzzleSolutionData =
                        "126754389 735298461 894613725 958342617 472561893 613987254 261479538 587136942 349825176"
                });
            }

            Database.SaveChanges();
            Database.Dispose();
        }

        private static SudokuDataContext Database { get; set; }

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
                DebugSettings.EnableFrameRateCounter = true;
#endif
            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                //We don't need to do this here, since we do it in the MainViewModel constructor:
                //if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                //{
                //    //TO DO: Load state from previously suspended application
                //}

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        ///     Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        ///     Invoked when application execution is being suspended.  Application state is saved
        ///     without knowing whether the application will be terminated or resumed with the contents
        ///     of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //Save application state and stop any background activity. This is actually done in the OnSuspending method in the MainViewModel class.
            deferral.Complete();
        }
    }
}