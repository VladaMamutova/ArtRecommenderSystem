using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ArtRecommenderSystem.Utilities
{
    public class SentByMeToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool) value
                ? Application.Current.FindResource("MidBackgroundBrush")
                : Application.Current.FindResource("LightBackgroundBrush");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
