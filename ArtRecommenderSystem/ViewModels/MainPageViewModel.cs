using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ArtRecommenderSystem.Database;
using ArtRecommenderSystem.Models;
using ArtRecommenderSystem.Utilities;
using Newtonsoft.Json;

namespace ArtRecommenderSystem.ViewModels
{
    class MainPageViewModel : INotifyPropertyChanged
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

        public RelayCommand LikeCommand { get; }
        public RelayCommand DislikeCommand { get; }

        public MainPageViewModel()
        {
            var tree = File.ReadAllText("art.json");
            var reader = new JsonTextReader(new StringReader(tree));
            var root = JsonSerializer.CreateDefault()
                .Deserialize<ArtNode>(reader);
            //root.InitParents(new[] { root.Name });
            root.InitParents(new string[0]);

            ArtCards = new List<ArtCard>(root.GetAllLeaves()
                .Select(BuildArtRecord));

            SetUserPreferences();

            LikeCommand = new RelayCommand(o =>
            {
                if (o is ArtCard artCard)
                {
                    if (artCard.IsLiked)
                    {
                        artCard.IsDisliked = false;
                        ApplicationContext.GetApplicationContext().UpdatePreference(artCard.Id, artCard.IsLiked);
                    }
                    else
                    {
                        ApplicationContext.GetApplicationContext()
                            .RemovePreference(artCard.Id);
                    }
                }
            });

            DislikeCommand = new RelayCommand(o =>
            {
                if (o is ArtCard artCard)
                {
                    if (artCard.IsDisliked)
                    {
                        artCard.IsLiked = false;
                        ApplicationContext.GetApplicationContext().UpdatePreference(artCard.Id, artCard.IsLiked);
                    }
                    else
                    {
                        ApplicationContext.GetApplicationContext()
                            .RemovePreference(artCard.Id);
                    }
                }
            });
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
            var preferences = ApplicationContext.GetApplicationContext()
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
