using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace SC.SimpleSudoku.Converters
{
    /// <summary>
    /// Converts a Time Span to its string representation. Used in the MainPage Xaml file to display bound TimeSpan objects.
    /// </summary>
    public class TimeSpanToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var timeSpan = (TimeSpan) value;
            //Returns the TimeSpan converted to a string.
            return $"{Math.Truncate(timeSpan.TotalHours):00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}"; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException(); //Not implemented, since converting nack to a TimeSpan from a string is not necessary.
        }
    }
}
