using System.ComponentModel;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using SC.SimpleSudoku.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SC.SimpleSudoku.Views
{
    public sealed partial class MainPage : Page
    {
        private readonly SystemNavigationManager _sysNavManager;
        private readonly MainViewModel _dataContext;


        private void SetBackVisibility(object sender, PropertyChangedEventArgs e)
        {
            _sysNavManager.AppViewBackButtonVisibility = _dataContext.CurrentNavState.PreviousNavState == null
                ? AppViewBackButtonVisibility.Collapsed
                : AppViewBackButtonVisibility.Visible;
        }

        public MainPage()
        {
            InitializeComponent();
            _sysNavManager = SystemNavigationManager.GetForCurrentView();
            _dataContext = (MainViewModel) DataContext;
            _sysNavManager.BackRequested += SysNavManager_BackRequested;
            _dataContext.CurrentNavState.PropertyChanged += SetBackVisibility;
        }

        private void SysNavManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (_dataContext.CurrentNavState.IsSolvingVisible)
                _dataContext.SavePuzzle();
            if (_dataContext.CurrentNavState.GoBack())
                e.Handled = true;
        }

        private void MainPage_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (_dataContext.CurrentNavState.IsSolvingVisible)
            {
                if ((int) e.Key > 48 && (int) e.Key < 58)
                    _dataContext.SetValue((byte) ((int) e.Key - 48));
                else if (e.Key >= VirtualKey.Left && e.Key <= VirtualKey.Down)
                {
                    _dataContext.MoveSelection((int) e.Key - (int) VirtualKey.Left);
                }
            }
        }
    }
}