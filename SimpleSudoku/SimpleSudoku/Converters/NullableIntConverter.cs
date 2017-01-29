using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using SC.SimpleSudoku.ViewModels;

namespace SC.SimpleSudoku.Converters
{
    /// <summary>
    /// Converts between a nullable integer and a string for binding purposes.
    /// </summary>
    public class NullableIntConverter : IValueConverter
    {
        //Converts 'value' from a nullable integer to a string.
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value == null ? "" : ((int?)value).ToString();
        }

        //Converts 'value' from a string to a nullable integer.
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                string str = (string) value;
                if (str == string.Empty)
                    return null;
                return int.Parse(str);
            }
            catch
            {
                return new ValidationResult("Parse Error :'(");
            }
        }
    }
}
