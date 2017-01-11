using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Microsoft.Xaml.Interactivity;

namespace SC.SimpleSudoku.Behaviors
{

    /// <summary>
    /// This is a behavior. It can be attatched to UI elements to make them more interactive. 
    /// This particular behavior listens for the key up event and invokes a command if the enter key has been pressed. 
    /// It is used on the login flyout, so that the user can press the enter key instead of clicking the sign in button.
    /// </summary>
    internal class ReturnKeyCommandBehavior : DependencyObject, IBehavior
    {
        private FrameworkElement _element;

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("Command", typeof(object),
            typeof(ReturnKeyCommandBehavior), null);

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ReturnKeyCommandBehavior), null);

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public ICommand Command
        {
            get { return GetValue(CommandProperty) as ICommand; }
            set { SetValue(CommandProperty, value); }
        }

        public DependencyObject AssociatedObject => _element;

        /// <param name="associatedObject"></param>
        public void Attach(DependencyObject associatedObject)
        {
            if (associatedObject == _element || DesignMode.DesignModeEnabled)
                return;
            _element = associatedObject as FrameworkElement;
            if (_element == null) return;

            _element.KeyUp += ElementKeyUp;
        }

        public void Detach()
        {
            _element.KeyUp -= ElementKeyUp;
        }

        /// <summary>
        /// Here's the event handler that runs when a key is pressed.
        /// It ensures that the key pressed is the Enter key, then
        /// executes the command set in the XAML.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElementKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (Command != null && e.Key == VirtualKey.Enter)
                Command.Execute(CommandParameter);
        }
    }
}