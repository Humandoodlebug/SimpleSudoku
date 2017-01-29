using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SC.SimpleSudoku.Converters
{
    /// <summary>
    /// Converts between a boolean and a visibility enum.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        //Converts 'value' from a boolean to a visibility enum value.
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool) value ? Visibility.Visible : Visibility.Collapsed;
        }

        //Converts 'value' from a visibility enum value to a boolean.
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (Visibility) value == Visibility.Visible;
        }
    }
}