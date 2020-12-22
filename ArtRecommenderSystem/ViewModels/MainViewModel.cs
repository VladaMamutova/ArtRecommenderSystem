using System;
using System.Collections.Generic;
using System.Linq;
using ArtRecommenderSystem.Database;
using ArtRecommenderSystem.Logic;
using ArtRecommenderSystem.Models;
using ArtRecommenderSystem.Utilities;

namespace ArtRecommenderSystem.ViewModels
{
    class MainViewModel : ArtCardsViewModel
    {
        private int _lowerPeriod;
        private int _upperPeriod;
        private int _lowerYear;
        private int _upperYear;
        private int _lowerEra; // 0 - до н. э., 1 - н. э.
        private int _upperEra; // 0 - до н. э., 1 - н. э.
        private int _lowerMuseumNumber;
        private int _upperMuseumNumber;
        private bool _masterClassesAreHeld;
        private bool _masterClassesAreNotHeld;
        private string _filterMessage;

        private RelayCommand _filterCommand;
        private RelayCommand _resetFiltersCommand;

        public int LowerPeriod
        {
            get => _lowerPeriod;
            set
            {
                _lowerPeriod = value;
                _lowerYear = ArtHelper.Periods[_lowerPeriod - 1].Start;
                LowerEra = _lowerYear < 0 ? 0 : 1;

                OnPropertyChanged(nameof(LowerPeriod));
                OnPropertyChanged(nameof(LowerYear));
                OnPropertyChanged(nameof(LowerEra));
                OnPropertyChanged(nameof(LowerDateText));
            }
        }

        public int UpperPeriod
        {
            get => _upperPeriod;
            set
            {
                _upperPeriod = value;
                _upperYear = ArtHelper.Periods[_upperPeriod - 1].End;
                UpperEra = _upperYear < 0 ? 0 : 1;

                OnPropertyChanged(nameof(UpperPeriod));
                OnPropertyChanged(nameof(UpperYear));
                OnPropertyChanged(nameof(UpperDateText));
            }
        }

        public int LowerYear
        {
            get => _lowerEra == 0 ? _lowerYear * -1 : _lowerYear;
            set
            {
                var newYear = value * (_lowerEra == 0 ? -1 : 1);
                if (value >= 0 && newYear >= MinYear && newYear <= MaxYear)
                {
                    _lowerYear = newYear;
                    _lowerPeriod = ArtHelper.GetPeriodNumber(_lowerYear);

                    OnPropertyChanged(nameof(LowerYear));
                    OnPropertyChanged(nameof(LowerPeriod));
                    OnPropertyChanged(nameof(LowerDateText));
                }
            }
        }

        public int UpperYear
        {
            get => _upperEra == 0 ? _upperYear * -1 : _upperYear;
            set
            {
                var newYear = value * (_upperEra == 0 ? -1 : 1);
                if (value >= 0 && newYear >= MinYear && newYear <= MaxYear)
                {
                    _upperYear = newYear;
                    _upperPeriod = ArtHelper.GetPeriodNumber(_upperYear);

                    OnPropertyChanged(nameof(UpperYear));
                    OnPropertyChanged(nameof(UpperPeriod));
                    OnPropertyChanged(nameof(UpperDateText));
                }
            }
        }

        public int LowerEra
        {
            get => _lowerEra;
            set
            {
                _lowerEra = value;
                if (_lowerEra == 0 && _lowerYear >= 0 ||
                    _lowerEra == 1 && _lowerYear < 0)
                {
                    _lowerYear *= -1;
                }

                if (_lowerYear < MinYear)
                {
                    _lowerYear = MinYear;
                    _lowerPeriod = MinPeriod;
                }
                else if (_lowerYear > MaxYear)
                {
                    _lowerYear = MaxYear;
                    _lowerPeriod = MaxPeriod;
                }
                else
                {
                    _lowerPeriod = ArtHelper.GetPeriodNumber(_lowerYear);
                }

                OnPropertyChanged(nameof(LowerEra));
                OnPropertyChanged(nameof(LowerYear));
                OnPropertyChanged(nameof(LowerPeriod));
                OnPropertyChanged(nameof(LowerDateText));
            }
        }

        public int UpperEra
        {
            get => _upperEra;
            set
            {
                _upperEra = value;
                if (_upperEra == 0 && _upperYear >= 0 ||
                    _upperEra == 1 && _upperYear < 0)
                {
                    _upperYear *= -1;
                }

                if (_upperYear < MinYear)
                {
                    _upperYear = MinYear;
                    _upperPeriod = MinPeriod;
                }
                else if (_upperYear > MaxYear)
                {
                    _upperYear = MaxYear;
                    _upperPeriod = MaxPeriod;
                }
                else
                {
                    _upperPeriod = ArtHelper.GetPeriodNumber(_upperYear);
                }

                OnPropertyChanged(nameof(UpperEra));
                OnPropertyChanged(nameof(UpperYear));
                OnPropertyChanged(nameof(UpperPeriod));
                OnPropertyChanged(nameof(UpperDateText));
            }
        }

        public int LowerMuseumNumber
        {
            get => _lowerMuseumNumber;
            set
            {
                if (value >= MinMuseumNumber && value <= MaxMuseumNumber)
                {
                    _lowerMuseumNumber = value;
                    OnPropertyChanged(nameof(LowerMuseumNumber));
                }
            }
        }

        public int UpperMuseumNumber
        {
            get => _upperMuseumNumber;
            set
            {
                if (value >= MinMuseumNumber && value <= MaxMuseumNumber)
                {
                    _upperMuseumNumber = value;
                    OnPropertyChanged(nameof(UpperMuseumNumber));
                }
            }
        }

        public bool MasterClassesAreHeld
        {
            get => _masterClassesAreHeld;
            set
            {
                _masterClassesAreHeld = value;
                OnPropertyChanged(nameof(MasterClassesAreHeld));
            }
        }

        public bool MasterClassesAreNotHeld
        {
            get => _masterClassesAreNotHeld;
            set
            {
                _masterClassesAreNotHeld = value;
                OnPropertyChanged(nameof(MasterClassesAreNotHeld));
            }
        }

        public string FilterMessage
        {
            get => _filterMessage;
            set
            {
                _filterMessage = value;
                OnPropertyChanged(nameof(FilterMessage));
            }
        }

        public int MinPeriod { get; }
        public int MaxPeriod { get; }
        public int MinYear { get; }
        public int MaxYear { get; }
        public int MinMuseumNumber { get; }
        public int MaxMuseumNumber { get; }

        public string LowerDateText => GetExtendedPeriodName(_lowerYear);
        public string UpperDateText => GetExtendedPeriodName(_upperYear);

        public List<GenreItem> GenreItems { get; }
        public List<PopularityItem> PopularityItems { get; }

        public RelayCommand FilterCommand
        {
            get
            {
                return _filterCommand ??
                       (_filterCommand = new RelayCommand(o => Filter()));
            }
        }

        public RelayCommand ResetFiltersCommand
        {
            get
            {
                return _resetFiltersCommand ??
                       (_resetFiltersCommand = new RelayCommand(o => ResetFilters()));
            }
        }
        
        public MainViewModel()
        {
            var artCards = ApplicationContext.GetInstance().Arts
                .Select(BuildArtRecord);

            foreach (var artCard in artCards)
            {
                ArtCards.Add(artCard);
            }

            EmptyMessage = "По вашему запросу ничего не найдено. " +
                           "Попробуйте снять ограничения в фильтре " +
                           "мастер-классов, популярности или жанров";

            _lowerPeriod = MinPeriod = 1;
            _upperPeriod = MaxPeriod = ArtHelper.Periods.Length;
            _lowerYear = MinYear = ArtHelper.Periods[MinPeriod - 1].Start;
            _upperYear = MaxYear = ArtHelper.Periods[MaxPeriod - 1].End;
            _lowerEra = _lowerYear < 0 ? 0 : 1;
            _upperEra = _upperYear < 0 ? 0 : 1;

            _lowerMuseumNumber = MinMuseumNumber = 0;
            _upperMuseumNumber = MaxMuseumNumber = 10;

            _filterMessage = "";

            GenreItems = new List<GenreItem>
            {
                new GenreItem(Genres.Household),
                new GenreItem(Genres.Portrait),
                new GenreItem(Genres.Landscape),
                new GenreItem(Genres.Historical),
                new GenreItem(Genres.Mythological),
                new GenreItem(Genres.Religious),
                new GenreItem(Genres.Battle),
                new GenreItem(Genres.StillLife),
                new GenreItem(Genres.Marina),
                new GenreItem(Genres.Animalistic),
                new GenreItem(Genres.Interior),
                new GenreItem(Genres.Decorative)
            };

            PopularityItems = new List<PopularityItem>
            {
                new PopularityItem(PopularityEnum.None),
                new PopularityItem(PopularityEnum.Seldom),
                new PopularityItem(PopularityEnum.Common),
                new PopularityItem(PopularityEnum.Widely)
            };
        }

        public override void Like(ArtCard artCard)
        {
            if (artCard.IsLiked)
            {
                artCard.IsDisliked = false;
                ApplicationContext.GetInstance()
                    .UpdatePreference(artCard.Id, artCard.IsLiked);
                LastChangedTime = DateTime.Now;

                ShowLikeMessage(artCard.Name);
            }
            else
            {
                ApplicationContext.GetInstance().RemovePreference(artCard.Id);
                LastChangedTime = DateTime.Now;

                ShowRemoveLikeMessage(artCard.Name);
            }
        }

        public override void Dislike(ArtCard artCard)
        {
            if (artCard.IsDisliked)
            {
                artCard.IsLiked = false;
                ApplicationContext.GetInstance()
                    .UpdatePreference(artCard.Id, artCard.IsLiked);
                LastChangedTime = DateTime.Now;

                ShowDislikeMessage(artCard.Name);
            }
            else
            {
                ApplicationContext.GetInstance().RemovePreference(artCard.Id);
                LastChangedTime = DateTime.Now;

                ShowRemoveDislikeMessage(artCard.Name);
            }
        }

        public override bool IsUpToDate()
        {
            return ApplicationContext.GetInstance().FavoritesChangedTime <
                   LastChangedTime &&
                   ApplicationContext.GetInstance().BlacklistChangedTime <
                   LastChangedTime;
        }

        public override void UpdateArtCards()
        {
            if (IsUpToDate()) return;

            var preferences = ApplicationContext.GetInstance().GetPreferences();

            foreach (var artCard in ArtCards)
            {
                artCard.IsLiked = false;
                artCard.IsDisliked = false;
            }

            foreach (var preference in preferences)
            {
                var artCard = ArtCards.FirstOrDefault(art => art.Id == preference.ArtId);
                if (artCard != null) // вид искусства был отфильтрован
                {
                    artCard.IsLiked = preference.Like == 1;
                    artCard.IsDisliked = preference.Like == 0;
                }
            }

            LastChangedTime = DateTime.Now;
        }

        private ArtCard BuildArtRecord(ArtLeaf leaf)
        {
            return new ArtCard
            {
                Id = leaf.Id,
                Name = leaf.Name,
                Parents = leaf.Parents,
                Date = leaf.Date,
                MuseumNumber = leaf.MuseumNumber,
                Popularity = leaf.Popularity,
                AreMasterClassesHeld = leaf.AreMasterClassesHeld,
                Genres = leaf.Genres.ToArray()
            };
        }

        private void Filter()
        {
            var settings = new FilterEngine.FilterSettings
            {
                MinYear = _lowerYear,
                MaxYear = _upperYear,
                MinMuseumNumber = LowerMuseumNumber,
                MaxMuseumNumber = UpperMuseumNumber,
                MasterClassesAreHeld = MasterClassesAreHeld,
                MasterClassesAreNotHeld = MasterClassesAreNotHeld,
                PopularityList = PopularityItems
                    .Where(item => item.IsChecked)
                    .Select(item => item.Popularity).ToList(),
                GenreList = GenreItems
                    .Where(item => item.IsChecked)
                    .Select(item => item.Genre).ToList()
            };

            var filterResult = FilterEngine.Filter(settings);
            FilterMessage = filterResult.IterationNumber > 0
                ? BuildFilterMessage(filterResult.Settings)
                : "";

            ArtCards.Clear();
            foreach (var art in filterResult.Arts)
            {
                ArtCards.Add(BuildArtRecord(art));
            }

            UpdateArtCards();
        }

        private void ResetFilters()
        {
            LowerPeriod = MinPeriod;
            UpperPeriod = MaxPeriod;
            LowerMuseumNumber = MinMuseumNumber;
            UpperMuseumNumber = MaxMuseumNumber;
            MasterClassesAreHeld = MasterClassesAreNotHeld = false;
            PopularityItems.ForEach(item => item.IsChecked = false);
            GenreItems.ForEach(item => item.IsChecked = false);
            FilterMessage = "";

            ArtCards.Clear();
            var arts = ApplicationContext.GetInstance().Arts;
            foreach (var art in arts)
            {
                ArtCards.Add(BuildArtRecord(art));
            }

            UpdateArtCards();
        }

        private string BuildFilterMessage(FilterEngine.FilterSettings settings)
        {
            var message = "Некоторые из фильтров поиска были расширены:";
            if (settings.MinYear != LowerYear || settings.MaxYear != UpperYear)
            {
                message += "\n\nПервое упоминание:";
                message += "\n   от " + GetExtendedPeriodName(settings.MinYear) +
                           "\n   до " + GetExtendedPeriodName(settings.MaxYear);
            }

            if (settings.MinMuseumNumber != LowerMuseumNumber ||
                settings.MaxMuseumNumber != UpperMuseumNumber)
            {
                message += "\n\nКоличество музеев: от " +
                           settings.MinMuseumNumber + " до " +
                           settings.MaxMuseumNumber;
            }

            return message;
        }

        private string GetExtendedPeriodName(int year)
        {
            int index = ArtHelper.GetPeriodNumber(year) - 1; // номер = индекс + 1
            int half = ArtHelper.Periods[index].GetDuration() / 2;

            string periodName = year.ConvertDateToString() + " (" +
                                ArtHelper.Periods[index].Name + ", ";
            if (year == ArtHelper.Periods[index].Start)
            {
                return periodName + "начало)";
            }

            if (year == ArtHelper.Periods[index].End)
            {
                return periodName + "конец)";
            }

            if (year <= ArtHelper.Periods[index].Start + half)
            {
                return periodName + "первая половина)";
            }

            return periodName + "вторая половина)";
        }
    }
}
