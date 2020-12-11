using System;

namespace ArtRecommenderSystem.Logic
{
    public static class ArtHelper
    {
        private class Period
        {
            public string Name { get; }
            public int Start { get; }
            public int End { get; }

            public Period(string name, int start, int end)
            {
                Name = name;
                Start = start;
                End = end;
            }
        }

        private static readonly Period[] Periods;
        private static readonly double[,] GenresSimilarity;

        public const int MAX_PERIOD_DIFFERENCE = 6;
        public const int MAX_MUSEUM_NUMBER_DIFFERENCE = 10;
        public const int MAX_GENRE_DIFFERENCE = 1;
        public const int MAX_MASTER_CLASSES_DIFFERENCE = 1;
        public const int MAX_POPULARITY_DIFFERENCE = 3;

        static ArtHelper()
        {
            Periods = new[]
            {
                new Period("Первобытность", -250000000, -5001),
                new Period("Древний мир", -5000, -801),
                new Period("Античность", -800, 450),
                new Period("Средневековье", 451, 1449),
                new Period("Ренессанс", 1450, 1599),
                new Period("Новое время", 1600, 1899),
                new Period("Новейшее время", 1900, int.MaxValue)
            };

            GenresSimilarity = new[,]
            {
                {1, 0.8, 0, 0.4, 0.4, 0.4, 0.6, 0.2, 0, 0.2, 0.6, 0.2, 0}, // Household
                {0.8, 1, 0.2, 0.8, 0.8, 0.8, 0.8, 0, 0, 0, 0, 0, 0}, // Portrait
                {0, 0.2, 1, 0.2, 0.2, 0.2, 0.2, 0, 0.8, 0, 0.2, 0.6, 0.2}, // Landscape
                {0.4, 0.8, 0.2, 1, 0.8, 0.8, 0.8, 0, 0, 0, 0.2, 0.4, 0}, // Historical
                {0.4, 0.8, 0.2, 0.8, 1, 0.8, 0.6, 0, 0, 0.4, 0, 0, 0}, // Mythological
                {0.4, 0.8, 0.2, 0.8, 0.8, 1, 0.6, 0, 0, 0, 0, 0, 0}, // Religious
                {0.6, 0.8, 0.2, 0.8, 0.6, 0.6, 1, 0, 0, 0.4, 0, 0, 0}, // Battle
                {0.2, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0.2, 0, 0.2}, // StillLife
                {0, 0, 0.8, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0}, // Marina
                {0.2, 0, 0, 0, 0.4, 0, 0.4, 0, 0, 1, 0.4, 0, 0}, // Animalistic
                {0.6, 0, 0.2, 0.2, 0, 0, 0, 0.2, 0, 0.4, 1, 0.6, 0.2}, // Interior
                {0.2, 0, 0.6, 0.4, 0, 0, 0, 0, 0, 0, 0.6, 1, 0.2}, // Architecture
                {0, 0, 0.2, 0, 0, 0, 0, 0.2, 0, 0, 0.2, 0.2, 1}, // Decorative
            };
        }

        public static int GetPeriodNumber(int date)
        {
            var period = -1;
            for (var i = 0; i < Periods.Length && period == -1; i++)
            {
                if (date < Periods[i].End)
                {
                    period = i + 1;
                }
            }

            return period;
        }

        public static string ConvertDateToString(this int date)
        {
            if (date < 0)
            {
                return Math.Abs(date).ToString("N0") + " г. до н. э.";
            }

            return date.ToString("N0") + " г.";
        }

        public static int ConvertStringToDate(this string date)
        {
            string result = "";
            if (date.Contains("до н. э."))
            {
                result += "-";
            }

            foreach (var symbol in date)
            {
                if (symbol <= '9' && symbol >= '0')
                {
                    result += symbol;
                }
            }

            return int.Parse(result);
        }

        //public static double GetGenresSimilarity(Genres[] list1, Genres[] list2)
        //{
        //    double result = 0;

        //    var genreMatrix = new double[list1.Length, list2.Length];
        //    var commonGenres = new List<int[]>();
        //    for (var i = 0; i < list1.Length; i++)
        //    {
        //        for (var j = 0; j < list2.Length; j++)
        //        {
        //            genreMatrix[i, j] =
        //                GenresSimilarity[(int)list1[i], (int)list2[j]];
        //            result += genreMatrix[i, j];

        //            if (Math.Abs(genreMatrix[i, j] - 1) < 0.01)
        //            {
        //                commonGenres.Add(new[] { i, j });
        //            }
        //        }
        //    }

        //    for (var i = 1; i < commonGenres.Count; i++)
        //    {
        //        result -= genreMatrix[commonGenres[i - 1][0],
        //            commonGenres[i][1]];
        //        result -= genreMatrix[commonGenres[i][0],
        //            commonGenres[i - 1][1]];
        //    }

        //    result /= list1.Length + list2.Length - commonGenres.Count;

        //    return result;
        //}
    }
}
