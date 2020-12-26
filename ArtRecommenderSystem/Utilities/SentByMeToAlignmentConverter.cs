using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ArtRecommenderSystem.Utilities
{
    public class SentByMeToAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            return value != null && (bool) value
                ? HorizontalAlignment.Right
                : HorizontalAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value != null && value is HorizontalAlignment alignment &&
                   alignment == HorizontalAlignment.Right;
        }
    }
}
