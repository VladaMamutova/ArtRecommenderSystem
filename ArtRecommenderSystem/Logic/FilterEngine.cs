using System;
using System.Collections.Generic;
using System.Linq;
using ArtRecommenderSystem.Database;
using ArtRecommenderSystem.Models;
using static ArtRecommenderSystem.Logic.ArtHelper;

namespace ArtRecommenderSystem.Logic
{
    static class FilterEngine
    {
        struct FilterIteration
        {
            public int FilterId;
            public int IterationNumber;

            public FilterIteration(int filterId, int iterationNumber)
            {
                FilterId = filterId;
                IterationNumber = iterationNumber;
            }
        }

        public struct FilterSettings
        {
            public int MinYear;
            public int MaxYear;
            public int MinMuseumNumber;
            public int MaxMuseumNumber;
            public bool MasterClassesAreHeld;
            public bool MasterClassesAreNotHeld;
            public List<PopularityEnum> PopularityList;
            public List<Genres> GenreList;
        }

        public struct FilterResult
        {
            public List<ArtLeaf> Arts;
            public FilterSettings Settings;
            public int IterationNumber;
        }

        private static int ExpandMinYear(int minYear)
        {
            // Сдвигаем нижнюю границу диапазона дат поиска.

            int index = GetPeriodNumber(minYear) - 1; // номер = индекс + 1
            int half = Periods[index].GetDuration() / 2;

            int newYear = minYear;
            if (minYear == Periods[index].Start)
            {
                if (index > 0)
                {
                    // Если текущий год - начало периода,
                    // то следующий год поиска - середина предыдущего периода.
                    half = Periods[index - 1].GetDuration() / 2;
                    newYear = Periods[index - 1].Start + half;
                }
            }
            else if (minYear <= Periods[index].Start + half)
            {
                // Если текущий год - в первой половине текущего периода,
                // то следующий год - начало периода.
                newYear = Periods[index].Start;
            }
            else
            {
                // Если текущий год - во второй половине периода,
                // то следующий год - середина периода.
                newYear = Periods[index].Start + half;
            }

            return newYear;
        }

        private static int ExpandMaxYear(int maxYear)
        {
            // Сдвигаем верхнюю границу диапазона дат поиска.

            int index = GetPeriodNumber(maxYear) - 1; // номер = индекс + 1
            int half = Periods[index].GetDuration() / 2;

            int newYear = maxYear;
            if (maxYear == Periods[index].End)
            {
                if (index < Periods.Length - 1)
                {
                    // Если текущий год - конец периода,
                    // то следующий год поиска - половина следующего периода.
                    half = Periods[index + 1].GetDuration() / 2;
                    newYear = Periods[index + 1].Start + half;
                }
            }
            else if (maxYear >= Periods[index].Start + half)
            {
                // Если текущий год - во второй половине текущего периода,
                // то следующий год - конец периода.
                newYear = Periods[index].End;
            }
            else
            {
                // Если текущий год - в первой половине периода,
                // то следующий год - середина периода.
                newYear = Periods[index].Start + half;
            }

            return newYear;
        }

        private static int GetDateIterations(int minYear, int maxYear)
        {
            // На каждой итерации добавляем половину текущего периода.
            // Так как всего 7 периодов, то максимальное число итераций - 14.

            int minPeriodIndex = GetPeriodNumber(minYear) - 1; // номер = индекс + 1
            int maxPeriodIndex = GetPeriodNumber(maxYear) - 1;

            int iterationsToMin = minPeriodIndex * 2;
            if (minYear > Periods[minPeriodIndex].Start +
                Periods[minPeriodIndex].GetDuration() / 2)
            {
                // Необходимо ещё 2 итерации: половина периода + начало периода.
                iterationsToMin += 2;
            }
            else if (minYear != Periods[minPeriodIndex].Start)
            {
                // Необходима ещё 1 итерация: начало периода.
                iterationsToMin += 1;
            }

            int iterationsToMax = (Periods.Length - 1 - maxPeriodIndex) * 2;
            if (maxYear < Periods[maxPeriodIndex].Start +
                Periods[maxPeriodIndex].GetDuration() / 2)
            {
                // Необходимо ещё 2 итерации: половина периода + конец периода.
                iterationsToMax += 2;
            }
            else if (maxYear != Periods[maxPeriodIndex].End)
            {
                // Необходима ещё 1 итерация: начало периода.
                iterationsToMax += 1;
            }

            return Math.Max(iterationsToMin, iterationsToMax);
        }

        private static int GetMuseumIterations(int minMuseumNumber,
            int maxMuseumNumber)
        {
            return Math.Max(minMuseumNumber,
                MAX_MUSEUM_NUMBER_DIFFERENCE - maxMuseumNumber);
        }

        private static int[] DistributeIterations(List<FilterIteration> filters)
        {
            var iterationNumber = filters.Sum(filter => filter.IterationNumber);
            var iterations = new int[iterationNumber];
            for (var i = 0; i < iterations.Length; i++)
            {
                iterations[i] = -1;
            }

            // Получаем частоту итераций для каждого фильтра поиска.
            var frequency = filters.ToDictionary(filter => filter.FilterId,
                filter => iterationNumber / filter.IterationNumber);

            // Сначала будем распределять итерации между фильтрами,
            // значения которых реже всего будут меняться,
            // поэтому сортируем список по возрастанию частоты.
            var frequencyDesc =
                frequency.OrderByDescending(filter => filter.Value).ToList();

            // Распределяем итерации между всеми фильтрами,
            // кроме наиболее часто встречающегося (то есть последнего в списке).
            for (var i = 0; i < frequencyDesc.Count - 1; i++)
            {
                var filter = frequencyDesc[i];
                var filterIterationNumber = filters
                    .Find(f => f.FilterId == filter.Key)
                    .IterationNumber;

                // Сдвигаемся на частоту фильтра и присваиваем итерации его id.
                // Повторяем, пока не присвоили все назначенные итерации фильтру.
                int currentIteration = 0;
                int index = filter.Value - 1;
                while (currentIteration < filterIterationNumber)
                {
                    if (iterations[index] == -1) // итерация не распределена
                    {
                        iterations[index] = filter.Key;
                    }
                    else // коллизия: итерация была уже распределена
                    {
                        if (index + 1 < iterations.Length
                        ) // назначаем следующую итерацию
                        {
                            iterations[index + 1] = filter.Key;
                        }
                        else // назначаем предыдующую итерацию
                        {
                            iterations[index - 1] = filter.Key;
                        }
                    }

                    currentIteration++;
                    index += filter.Value;
                }
            }

            // Нераспредённым итерациям назначаем id наиболее частотному фильтру.
            for (var i = 0; i < iterations.Length; i++)
            {
                if (iterations[i] == -1)
                {
                    iterations[i] = frequencyDesc[frequencyDesc.Count - 1].Key;
                }
            }

            return iterations;
        }

        private static List<ArtLeaf> RankFilteredArts(List<ArtLeaf> arts, FilterSettings settings)
        {
            // Рассчитываем оценки отличия от значений исходных фильтров
            // (расширяемые фильтры: дата и количество музеев).
            Dictionary<ArtLeaf, double> artScores = new Dictionary<ArtLeaf, double>();
            foreach (var art in arts)
            {
                double dateDifferenceScore = CalcDifferenceScore(art.Date,
                    settings.MinYear, settings.MaxYear);

                double museumDifferenceScore = CalcDifferenceScore(
                    art.MuseumNumber, settings.MinMuseumNumber,
                    settings.MaxMuseumNumber);

                artScores.Add(art, dateDifferenceScore + museumDifferenceScore);
            }

            // Возвращаем отсортированный по возрастанию список искусств:
            // чем меньше выход за диапазоны фильтров, тем объект больше
            // подходит по критериям поиска.
            return artScores.OrderBy(artScore => artScore.Value)
                .Select(artScore => artScore.Key).ToList();
        }

        private static double CalcDifferenceScore(int value, int min, int max)
        {
            double difference = 0;

            // Получаем абсолютное значение выхода из диапазона.
            if (value < min)
            {
                difference = min - value;
            }
            else if (value > max)
            {
                difference = value - max;
            }

            var range = max - min; // величина диапазона
            if (range != 0)
            {
                return difference / range; // доля выхода за границы диапазона
            }

            return difference; // так как диапазон состоял из одного значения,
                               // то возвращаем абсолютное значение выхода из диапазона
        }

        public static FilterResult Filter(FilterSettings settings)
        {
            int minYear = settings.MinYear;
            int maxYear = settings.MaxYear;
            int minMuseumNumber = settings.MinMuseumNumber;
            int maxMuseumNumber = settings.MaxMuseumNumber;

            // На случай если значения фильтров придётся расширять (то есть после
            // первой итерации ничего не будет найдено), получаем список из
            // id фильтров и максимально возможного количества итераций,
            // необходимого для покрытия всего диапазона значений фильтра.
            int dateFilterId = 1;
            int museumFilterId = 2;
            var filterIterations = new List<FilterIteration>
            {
                new FilterIteration(dateFilterId,
                    GetDateIterations(minYear, maxYear)),
                new FilterIteration(museumFilterId,
                    GetMuseumIterations(minMuseumNumber, maxMuseumNumber))
            };
            filterIterations = filterIterations
                .Where(filter => filter.IterationNumber > 0)
                .ToList();

            // Расширять значения фильтров будем равномерно, для этого получаем 
            // массив, в котором i-й итерации присвоен id фильтра.
            int[] iterationId = DistributeIterations(filterIterations);

            int iterationCount = -1;
            List<ArtLeaf> filteredArts;
            do
            {
                iterationCount++;

                // Если после первой итерации ничего не найдено, на каждой
                // следующей расширяем значения того фильтра, id которого
                // записано в массив.
                if (iterationCount > 0)
                {
                    if (iterationId[iterationCount - 1] == dateFilterId)
                    {
                        minYear = ExpandMinYear(minYear);
                        maxYear = ExpandMaxYear(maxYear);
                    }
                    else if (iterationId[iterationCount - 1] == museumFilterId)
                    {
                        minMuseumNumber = Math.Max(minMuseumNumber - 1, 0);
                        maxMuseumNumber = Math.Min(maxMuseumNumber + 1,
                            MAX_MUSEUM_NUMBER_DIFFERENCE);
                    }
                }

                filteredArts = ApplicationContext.GetInstance().Arts
                    .FilterByDate(minYear, maxYear)
                    .FilterByMuseumNumber(minMuseumNumber, maxMuseumNumber)
                    .FilterByMasterClasses(settings.MasterClassesAreHeld,
                        settings.MasterClassesAreNotHeld)
                    .FilterByPopularity(settings.PopularityList)
                    .FilterByGenres(settings.GenreList);

            } while (filteredArts.Count == 0 &&
                     iterationCount < iterationId.Length);

            // Ранжируем выдачу, если при поиске с исходными фильтрами
            // ничего не нашлось и значения фильтров были расширены.
            if (iterationCount > 0 && filteredArts.Count > 1)
            {
                filteredArts = RankFilteredArts(filteredArts, settings);
            }

            settings.MinYear = minYear;
            settings.MaxYear = maxYear;
            settings.MinMuseumNumber = minMuseumNumber;
            settings.MaxMuseumNumber = maxMuseumNumber;

            return new FilterResult
            {
                Arts = filteredArts,
                Settings = settings,
                IterationNumber = iterationCount
            };
        }

        public static List<ArtLeaf> FilterByDate(
            this List<ArtLeaf> arts, int min, int max)
        {
            return arts.Where(art => art.Date >= min && art.Date <= max)
                .ToList();
        }

        public static List<ArtLeaf> FilterByMuseumNumber(
            this List<ArtLeaf> arts, int min, int max)
        {
            return arts.Where(art =>
                art.MuseumNumber >= min && art.MuseumNumber <= max).ToList();
        }

        public static List<ArtLeaf> FilterByMasterClasses(
            this List<ArtLeaf> arts, bool areHeld, bool areNotHeld)
        {
            if (areHeld == areNotHeld) return arts;

            return arts.Where(art =>
                art.AreMasterClassesHeld == areHeld).ToList();
        }

        public static List<ArtLeaf> FilterByPopularity(
            this List<ArtLeaf> arts, List<PopularityEnum> popularityList)
        {
            return popularityList.Count == 0
                ? arts
                : arts.Where(art => popularityList.Contains(art.Popularity))
                    .ToList();
        }

        public static List<ArtLeaf> FilterByGenres(
            this List<ArtLeaf> arts, List<Genres> genreList)
        {
            return genreList.Count == 0
                ? arts
                : arts.Where(art =>
                        genreList.All(genre => art.Genres.Contains(genre)))
                    .ToList();
        }
    }
}
