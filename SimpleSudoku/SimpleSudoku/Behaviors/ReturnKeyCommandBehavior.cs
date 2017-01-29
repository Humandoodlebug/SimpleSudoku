using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Microsoft.Xaml.Interactivity;

namespace SC.SimpleSudoku.Behaviors
{
    /// <summary>
    ///     This is a behavior. It can be attatched to UI elements to make them more interactive.
    ///     This particular behavior listens for the key up event and invokes a command if the enter key has been pressed.
    ///     It is used on the login flyout, so that the user can press the enter key instead of clicking the sign in button.
    /// </summary>
    internal class ReturnKeyCommandBehavior : DependencyObject, IBehavior
    {
        //Dependency properties to hold values set in the XAML.
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("Command",
            typeof(object),
            typeof(ReturnKeyCommandBehavior), null);

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ReturnKeyCommandBehavior), null);

        private FrameworkElement _element;

        /// <summary>
        ///     Exposes an optional parameter to pass to the command when it is invoked.
        /// </summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        ///     Exposes the command that should be invoked when the return key is pressed.
        /// </summary>
        public ICommand Command
        {
            get { return GetValue(CommandProperty) as ICommand; }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        ///     Exposes the framework element the behaviour is attached to.
        /// </summary>
        public DependencyObject AssociatedObject => _element;

        /// <summary>
        ///     Called when the behaviour is attached to a UI Framework element.
        /// </summary>
        /// <param name="associatedObject">The framework element the behaviour has been attached to.</param>
        public void Attach(DependencyObject associatedObject)
        {
            //If associatedObject has already been attached to or design mode is enabled, do nothing.
            if (associatedObject == _element || DesignMode.DesignModeEnabled)
                return;
            //Store the associatedObject that the behaviour has been attached to.
            _element = associatedObject as FrameworkElement;
            //If the object that has been attached to is not null, 
            //subscribe the ElementKeyUp event handler to the key up event on the framework element.
            if (_element == null) return;

            _element.KeyUp += ElementKeyUp;
        }

        /// <summary>
        /// Called when the UI Framework element is disposed of.
        /// Unsubscribes the ElementKeyUp event handler from the KeyUp event on the framework element.
        /// </summary>
        public void Detach()
        {
            _element.KeyUp -= ElementKeyUp;
        }

        /// <summary>
        ///     Here's the event handler that runs when a key is pressed.
        ///     It ensures that the key pressed is the Enter key, then
        ///     executes the command set in the XAML.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Details about the circumstances that triggered the event.</param>
        private void ElementKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (Command != null && e.Key == VirtualKey.Enter)
                Command.Execute(CommandParameter);
        }
    }
}