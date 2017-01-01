using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
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
    }
}