using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ArtRecommenderSystem.Database;
using ArtRecommenderSystem.Logic;
using ArtRecommenderSystem.Models;
using ArtRecommenderSystem.Utilities;

namespace ArtRecommenderSystem.ViewModels
{
    class ArtCardsViewModel : INotifyPropertyChanged
    {
        private bool? _areFavorites;
        private bool _areRecommendations;
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

        public ArtCardsViewModel(bool areRecommendations, bool? areFavorites)
        {
            _areFavorites = areFavorites;
            _lastChangedTime = DateTime.MinValue;
            _areRecommendations = areRecommendations;

            if (!areFavorites.HasValue)
            {
                ArtCards = new ObservableCollection<ArtCard>(ApplicationContext
                    .GetInstance().Arts.Select(BuildArtRecord));
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

        public class SnackBarMessageDisplayEventArgs : EventArgs
        {
            public string Message { get; set; }
        }

        public event EventHandler<SnackBarMessageDisplayEventArgs>
            SnackBarMessageDisplayRequested;

        protected void OnSnackBarMessageDisplayRequest(string message)
        {
            SnackBarMessageDisplayRequested?.Invoke(this,
                new SnackBarMessageDisplayEventArgs {Message = message});
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

                OnSnackBarMessageDisplayRequest(
                    "Вид искусства \"" + artCard.Name +
                    "\" добавлен в раздел \"Понравившиеся\" Моей галереи");
            }
            else
            {
                ApplicationContext.GetInstance().RemovePreference(artCard.Id);
                if (_areFavorites.HasValue && _areFavorites.Value)
                {
                    ArtCards.Remove(artCard);
                }

                _lastChangedTime = DateTime.Now;

                OnSnackBarMessageDisplayRequest(
                    "Вид искусства \"" + artCard.Name +
                    "\" удалён из раздела " +
                    "\"Понравившиеся\" Моей галереи");
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
                 if (_areRecommendations)
                {
                    ArtCards.Remove(artCard);
                }

                OnSnackBarMessageDisplayRequest(
                    "Вид искусства \"" + artCard.Name + "\" больше не будет " +
                    "появляться в ваших рекомендациях");

                _lastChangedTime = DateTime.Now;
            }
            else
            {
                ApplicationContext.GetInstance().RemovePreference(artCard.Id);
                if (_areFavorites.HasValue && !_areFavorites.Value)
                {
                    ArtCards.Remove(artCard);
                }

                OnSnackBarMessageDisplayRequest(
                    "Вид искусства \"" + artCard.Name + "\" теперь " +
                    "сможет появляться в ваших рекомендациях");

                _lastChangedTime = DateTime.Now;
            }
        }

        private void UpdateUserPreferences()
        {
            var preferences = ApplicationContext.GetInstance().GetPreferences();

            foreach (var artCard in _artCards)
            {
                artCard.IsLiked = false;
                artCard.IsDisliked = false;
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
            if (_areFavorites.HasValue && !_areRecommendations)
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
                if (_areRecommendations)
                {
                    ArtCards = new ObservableCollection<ArtCard>();
                    var arts = ApplicationContext.GetInstance().Arts;
                    var recommendedArtIds =
                        RecommendationEngine.CollaborativeFiltering();

                    var blackList = ApplicationContext.GetInstance()
                        .GetBlacklist();
                    foreach (var id in recommendedArtIds)
                    {
                        var artLeaf = arts.Find(art => art.Id == id);
                        if (blackList.All(art => art.ArtId != artLeaf.Id))
                        {
                            ArtCards.Add(BuildArtRecord(artLeaf));
                        }
                    }

                }
                else
                {
                    if (_areFavorites.HasValue)
                    {
                        var preferences = _areFavorites.Value
                            ? ApplicationContext.GetInstance().GetFavorites()
                            : ApplicationContext.GetInstance().GetBlacklist();
                        var artCards = new List<ArtCard>();

                        var arts = ApplicationContext.GetInstance().Arts;
                        foreach (var preference in preferences)
                        {
                            var artCard = BuildArtRecord(
                                arts.Find(art => art.Id == preference.ArtId));
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
                }

                _lastChangedTime = DateTime.Now;
            }
        }
    }
}
