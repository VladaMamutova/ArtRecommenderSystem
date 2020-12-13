using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ArtRecommenderSystem.Database;
using ArtRecommenderSystem.Models;
using ArtRecommenderSystem.Utilities;

namespace ArtRecommenderSystem.ViewModels
{
    class ArtCardsViewModel : INotifyPropertyChanged
    {
        private bool? _areFavorites;
        private DateTime _lastChangedTime;

        private ObservableCollection<ArtCard> _artCards;

        public ObservableCollection<ArtCard> ArtCards
        {
            get => _artCards;
            set
            {
                _artCards = value;
                OnPropertyChanged(nameof(ArtCards));
            }
        }

        private RelayCommand _likeCommand;

        public RelayCommand LikeCommand
        {
            get
            {
                return _likeCommand ??
                       (_likeCommand = new RelayCommand(o =>
                       {
                           if (o is ArtCard artCard) Like(artCard);
                       }));
            }
        }

        private RelayCommand _dislikeCommand;

        public RelayCommand DislikeCommand
        {
            get
            {
                return _dislikeCommand ??
                       (_dislikeCommand = new RelayCommand(o =>
                       {
                           if (o is ArtCard artCard) Dislike(artCard);
                       }));
            }
        }

        public ArtCardsViewModel(bool? areFavorites = null)
        {
            _areFavorites = areFavorites;
            _lastChangedTime = DateTime.MinValue;

            if (!areFavorites.HasValue)
            {
                ArtCards = new ObservableCollection<ArtCard>(ApplicationContext
                    .GetInstance().ArtLeaves.Select(BuildArtRecord));
            }

            UpdateArtCards();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }

        private static ArtCard BuildArtRecord(ArtLeaf leaf)
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

        private void Like(ArtCard artCard)
        {
            if (artCard.IsLiked)
            {
                artCard.IsDisliked = false;
                ApplicationContext.GetInstance()
                    .UpdatePreference(artCard.Id, artCard.IsLiked);
                if (_areFavorites.HasValue && !_areFavorites.Value)
                {
                    ArtCards.Remove(artCard);
                }

                _lastChangedTime = DateTime.Now;
            }
            else
            {
                ApplicationContext.GetInstance().RemovePreference(artCard.Id);
                if (_areFavorites.HasValue && _areFavorites.Value)
                {
                    ArtCards.Remove(artCard);
                }

                _lastChangedTime = DateTime.Now;
            }
        }

        private void Dislike(ArtCard artCard)
        {
            if (artCard.IsDisliked)
            {
                artCard.IsLiked = false;
                ApplicationContext.GetInstance()
                    .UpdatePreference(artCard.Id, artCard.IsLiked);
                if (_areFavorites.HasValue && _areFavorites.Value)
                {
                    ArtCards.Remove(artCard);
                }

                _lastChangedTime = DateTime.Now;
            }
            else
            {
                ApplicationContext.GetInstance().RemovePreference(artCard.Id);
                if (_areFavorites.HasValue && !_areFavorites.Value)
                {
                    ArtCards.Remove(artCard);
                }

                _lastChangedTime = DateTime.Now;
            }
        }

        private void UpdateUserPreferences()
        {
            var preferences = ApplicationContext.GetInstance()
                .GetUserPreferences();

            foreach (var artCard in _artCards)
            {
                artCard.IsLiked = false;
                artCard.IsDisliked = false; ;
            }

            foreach (var preference in preferences)
            {
                var artCard =
                    _artCards.First(art => art.Id == preference.ArtId);
                artCard.IsLiked = preference.Like == 1;
                artCard.IsDisliked = preference.Like == 0;
            }
        }

        private bool NeedToUpdate()
        {
            if (_areFavorites.HasValue)
            {
                return _areFavorites.Value
                    ? ApplicationContext.GetInstance().FavoritesChangedTime >
                      _lastChangedTime
                    : ApplicationContext.GetInstance().BlacklistChangedTime >
                      _lastChangedTime;
            }

            return ApplicationContext.GetInstance().FavoritesChangedTime >
                   _lastChangedTime ||
                   ApplicationContext.GetInstance().BlacklistChangedTime >
                   _lastChangedTime;
        }

        public void UpdateArtCards()
        {
            if (NeedToUpdate())
            {
                if (_areFavorites.HasValue)
                {
                    var preferences = ApplicationContext.GetInstance()
                        .GetUserPreferences(_areFavorites.Value);
                    var artCards = new List<ArtCard>();

                    var artLeaves = ApplicationContext.GetInstance().ArtLeaves;
                    foreach (var preference in preferences)
                    {
                        var artLeaf =
                            artLeaves.Find(art => art.Id == preference.ArtId);
                        var artCard = BuildArtRecord(artLeaf);
                        artCard.IsLiked = _areFavorites.Value;
                        artCard.IsDisliked = !_areFavorites.Value;
                        artCards.Add(artCard);
                    }

                    ArtCards = new ObservableCollection<ArtCard>(artCards);
                }
                else
                {
                    UpdateUserPreferences();
                }

                _lastChangedTime = DateTime.Now;
            }
        }
    }
}
