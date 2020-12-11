using System;
using System.Globalization;
using System.Windows.Data;
using ArtRecommenderSystem.Models;

namespace ArtRecommenderSystem.Utilities
{
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (value == null || !(value is PopularityEnum popularity))
                return null;

            return (int)popularity;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (value == null || !(value is int popularityValue))
                return PopularityEnum.None;

            return (PopularityEnum) popularityValue;
        }
    }
}