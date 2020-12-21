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
        private int _lowerPeriodNumber;
        private int _upperPeriodNumber;
        private int _lowerYear;
        private int _upperYear;
        private int _lowerMuseumNumber;
        private int _upperMuseumNumber;
        private bool _masterClassesAreHeld;
        private bool _masterClassesAreNotHeld;

        private RelayCommand _filterCommand;
        private RelayCommand _resetFiltersCommand;

        public string LowerPeriod
        {
            get
            {
                var period = ArtHelper.Periods[_lowerPeriodNumber - 1];
                return $"{period.Start.ConvertDateToString()} ({period.Name})";
            }
        }

        public string UpperPeriod
        {
            get
            {
                var period = ArtHelper.Periods[_upperPeriodNumber - 1];
                return $"{period.End.ConvertDateToString()} ({period.Name})";
            }
        }

        public int LowerPeriodNumber
        {
            get => _lowerPeriodNumber;
            set
            {
                _lowerPeriodNumber = value;
                OnPropertyChanged(nameof(LowerPeriodNumber));
                OnPropertyChanged(nameof(LowerPeriod));
            }
        }

        public int UpperPeriodNumber
        {
            get => _upperPeriodNumber;
            set
            {
                _upperPeriodNumber = value;
                OnPropertyChanged(nameof(UpperPeriodNumber));
                OnPropertyChanged(nameof(UpperPeriod));
            }
        }

        public int LowerYear
        {
            get => _lowerYear;
            set
            {
                _lowerYear = value;
                OnPropertyChanged(nameof(LowerYear));
            }
        }

        public int UpperYear
        {
            get => _upperYear;
            set
            {
                _upperYear = value;
                OnPropertyChanged(nameof(UpperYear));
            }
        }

        public int LowerMuseumNumber
        {
            get => _lowerMuseumNumber;
            set
            {
                _lowerMuseumNumber = value;
                OnPropertyChanged(nameof(LowerMuseumNumber));
            }
        }

        public int UpperMuseumNumber
        {
            get => _upperMuseumNumber;
            set
            {
                _upperMuseumNumber = value;
                OnPropertyChanged(nameof(UpperMuseumNumber));
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

        public int MinPeriodNumber => 1;
        public int MaxPeriodNumber => ArtHelper.Periods.Length;
        public int MinYear { get; }
        public int MaxYear { get; }
        public int MinMuseumNumber { get; }
        public int MaxMuseumNumber { get; }

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

            LowerPeriodNumber = MinPeriodNumber;
            UpperPeriodNumber = MaxPeriodNumber;
            LowerYear = MinYear = ArtHelper.Periods[0].Start;
            UpperYear = MaxYear = DateTime.Now.Year;
            LowerMuseumNumber = MinMuseumNumber = 0;
            UpperMuseumNumber = MaxMuseumNumber = 10;

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

        public void Filter()
        {
            var settings = new FilterEngine.FilterSettings
            {
                MinYear = LowerYear,
                MaxYear = UpperYear,
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

            ArtCards.Clear();
            foreach (var art in filterResult.Arts)
            {
                ArtCards.Add(BuildArtRecord(art));
            }

            UpdateArtCards();
        }

        public void ResetFilters()
        {
            LowerYear = MinYear;
            UpperYear = MaxYear;
            LowerMuseumNumber = MinMuseumNumber;
            UpperMuseumNumber = MaxMuseumNumber;
            MasterClassesAreHeld = MasterClassesAreNotHeld = false;
            PopularityItems.ForEach(item => item.IsChecked = false);
            GenreItems.ForEach(item => item.IsChecked = false);

            ArtCards.Clear();
            var arts = ApplicationContext.GetInstance().Arts;
            foreach (var art in arts)
            {
                ArtCards.Add(BuildArtRecord(art));
            }

            UpdateArtCards();
        }
    }
}
