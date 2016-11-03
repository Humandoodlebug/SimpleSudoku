using System;
using System.Collections.Generic;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SC.SimpleSudoku.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SystemNavigationManager SysNavManager;

        public void SetBackVisibility()
        {
            //TODO: Implement INotifyPropertyChanged in the MainViewModel and create a property to store the visibility of the back button, subscribing this method to it and setting the button's visibility from here through sysNavManager.AppViewBackButtonVisibility.
        }
        public MainPage()
        {
            InitializeComponent();
            SysNavManager = SystemNavigationManager.GetForCurrentView();
            //Temporary testing code
            SysNavManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SysNavManager.BackRequested += SysNavManager_BackRequested;
        }

        private void SysNavManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
