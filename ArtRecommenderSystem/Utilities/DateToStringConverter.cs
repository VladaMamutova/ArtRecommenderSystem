using System;
using System.Globalization;
using System.Windows.Data;
using ArtRecommenderSystem.Logic;

namespace ArtRecommenderSystem.Utilities
{
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is int date))
                return null;
            return date.ConvertDateToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is string dateAsString))
                return null;

            return dateAsString.ConvertStringToDate();
        }
    }
}
