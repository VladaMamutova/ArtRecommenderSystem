using System.Collections.Generic;
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
        private List<ArtCard> _artCards;

        public List<ArtCard> ArtCards
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
                           if (!(o is ArtCard artCard)) return;
                           if (artCard.IsLiked)
                           {
                               artCard.IsDisliked = false;
                               ApplicationContext.GetInstance()
                                   .UpdatePreference(artCard.Id,
                                       artCard.IsLiked);
                           }
                           else
                           {
                               ApplicationContext.GetInstance()
                                   .RemovePreference(artCard.Id);
                           }
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
                           if (!(o is ArtCard artCard)) return;
                           if (artCard.IsDisliked)
                           {
                               artCard.IsLiked = false;
                               ApplicationContext.GetInstance()
                                   .UpdatePreference(artCard.Id,
                                       artCard.IsLiked);
                           }
                           else
                           {
                               ApplicationContext.GetInstance()
                                   .RemovePreference(artCard.Id);
                           }
                       }));
            }
        }

        public ArtCardsViewModel()
        {
            ArtCards = new List<ArtCard>(ApplicationContext
                .GetInstance().ArtLeaves.Select(BuildArtRecord));
            SetUserPreferences();
        }

        public ArtCardsViewModel(bool isFavorite)
        {
            var artLeaves = ApplicationContext.GetInstance().ArtLeaves;
            var preferences = ApplicationContext.GetInstance()
                .GetUserPreferences(isFavorite);

            ArtCards = new List<ArtCard>();
            foreach (var preference in preferences)
            {
                var artLeaf = artLeaves.Find(art => art.Id == preference.ArtId);
                var artCard = BuildArtRecord(artLeaf);
                artCard.IsLiked = isFavorite;
                artCard.IsDisliked = !isFavorite;
                ArtCards.Add(artCard);
            }
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


        private void SetUserPreferences()
        {
            var preferences = ApplicationContext.GetInstance()
                .GetUserPreferences();
            foreach (var preference in preferences)
            {
                var artCard = _artCards.Find(art => art.Id == preference.ArtId);
                artCard.IsLiked = preference.Like == 1;
                artCard.IsDisliked = preference.Like == 0;
            }
        }
    }
}
