using System;
using System.Globalization;
using System.Windows.Data;
using ArtRecommenderSystem.Models;

namespace ArtRecommenderSystem.Utilities
{
    class GenreToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (value == null || !(value is Genres genre))
                return null;

            return genre.GetAttrValueByEnumValue();
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var genreString = value as string;

            if (string.IsNullOrEmpty(genreString))
                return null;

            return genreString.GetEnumMemberByAttrValue<Genres>();
        }
    }
}
