using System;
using System.Globalization;
using System.Windows.Data;
using ArtRecommenderSystem.Models;

namespace ArtRecommenderSystem.Utilities
{
    public class PopularityToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (value == null || !(value is PopularityEnum popularity))
                return null;

            return popularity.GetAttrValueByEnumValue();
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var popularityString = value as string;

            if (string.IsNullOrEmpty(popularityString))
                return null;

            return popularityString.GetEnumMemberByAttrValue<PopularityEnum>();
        }
    }
}
