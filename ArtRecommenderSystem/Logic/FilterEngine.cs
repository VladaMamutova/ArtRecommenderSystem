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
            int index = GetPeriodNumber(minYear) - 1; // номер = индекс + 1
            int half = (Periods[index].End - Periods[index].Start) / 2;

            int newYear = minYear;
            if (minYear == Periods[index].Start)
            {
                if (index > 0)
                {
                    half = Periods[index - 1].GetDuration() / 2;
                    newYear = Periods[index - 1].Start + half;
                }
            }
            else if (minYear <= Periods[index].Start + half)
            {
                newYear = Periods[index].Start;
            }
            else
            {
                newYear = Periods[index].Start + half;
            }

            return newYear;
        }

        private static int ExpandMaxYear(int maxYear)
        {
            int index = GetPeriodNumber(maxYear) - 1; // номер = индекс + 1
            int half = (Periods[index].End - Periods[index].Start) / 2;

            int newYear = maxYear;
            if (maxYear == Periods[index].End)
            {
                if (index < Periods.Length - 1)
                {
                    half = Periods[index + 1].GetDuration() / 2;
                    newYear = Periods[index + 1].Start + half;
                }
            }
            else if (maxYear >= Periods[index].Start + half)
            {
                newYear = Periods[index].End;
            }
            else
            {
                newYear = Periods[index].Start + half;
            }

            return newYear;
        }

        private static int GetDateIterations(int minYear, int maxYear)
        {
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

            var frequency =
                filters.ToDictionary(filter => filter.FilterId,
                    filter => iterationNumber / filter.IterationNumber);

            var frequencyDesc =
                frequency.OrderByDescending(filter => filter.Value).ToList();
            for (var i = 0; i < frequencyDesc.Count - 1; i++)
            {
                var filter = frequencyDesc[i];
                var filterIterationNumber = filters
                    .Find(f => f.FilterId == filter.Key)
                    .IterationNumber;
                int currentIteration = 0;
                for (var j = filter.Value;
                    j < iterations.Length &&
                    currentIteration < filterIterationNumber;
                    j += filter.Value)
                {

                    if (iterations[j] == -1)
                    {
                        iterations[j] = filter.Key;
                    }
                    else
                    {
                        if (j + 1 < iterations.Length)
                        {
                            iterations[j + 1] = filter.Key;
                        }
                        else
                        {
                            iterations[j - 1] = filter.Key;
                        }
                    }

                    currentIteration++;
                }
            }

            for (var i = 0; i < iterations.Length; i++)
            {
                if (iterations[i] == -1)
                {
                    iterations[i] = frequencyDesc[frequencyDesc.Count - 1].Key;
                }
            }

            return iterations;
        }
        
        public static FilterResult Filter(FilterSettings settings)
        {
            int minYear = settings.MinYear;
            int maxYear = settings.MaxYear;
            int minMuseumNumber = settings.MinMuseumNumber;
            int maxMuseumNumber = settings.MaxMuseumNumber;

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

            int[] iterationId = DistributeIterations(filterIterations);
            int iterationCount = -1;
            List<ArtLeaf> filteredArts;
            do
            {
                iterationCount++;
                if (iterationCount > 0)
                {
                    if (iterationId[iterationCount] == dateFilterId)
                    {
                        minYear = ExpandMinYear(minYear);
                        maxYear = ExpandMaxYear(maxYear);
                    }
                    else if (iterationId[iterationCount] == museumFilterId)
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
                     iterationCount + 1 < iterationId.Length);

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
