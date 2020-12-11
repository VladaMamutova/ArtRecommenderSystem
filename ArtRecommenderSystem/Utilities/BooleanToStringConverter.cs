using System;
using System.Globalization;
using System.Windows.Data;

namespace ArtRecommenderSystem.Utilities
{
    public class BooleanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool)value ? "Есть" : "Нет";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (string)value == "Есть";
        }
    }
}
