using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml.Data;

namespace SC.SimpleSudoku.Converters
{
    //public class EnumDescriptionConverter : IValueConverter
    //{

    //    private static string GetEnumDescription(Enum enumObj)
    //    {
    //        var fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

    //        var attribArray = fieldInfo.GetCustomAttributes(false).ToArray();

    //        if (attribArray.Length == 0)
    //        {
    //            return enumObj.ToString();
    //        }
    //        var attrib = attribArray.OfType<DescriptionAttribute>().FirstOrDefault();
    //        return attrib?.ToString() ?? enumObj.ToString();
    //    }

    //    public object Convert(object value, Type targetType, object parameter, string language)
    //    {
    //        return GetEnumDescription((Enum)value);
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, string language)
    //    {
    //        return null;
    //    }
    //}
}