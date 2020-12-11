using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace ArtRecommenderSystem.Utilities
{
    public class StringArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (value == null || !(value is IEnumerable<string> list))
                return null;

            return string.Join(" / ", list);
            ;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var listAsString = value as string;

            if (string.IsNullOrEmpty(listAsString))
                return null;

            var list = listAsString.Split(new[] {" / "},
                StringSplitOptions.RemoveEmptyEntries);
            return list;
        }
    }
}
