using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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

            LikeCommand = new RelayCommand(o =>
            {
                if (o is ArtCard artCard)
                {
                    if (artCard.IsLiked) artCard.IsDisliked = false;
                }
            });

            DislikeCommand = new RelayCommand(o =>
            {
                if (o is ArtCard artCard)
                {
                    if (artCard.IsDisliked) artCard.IsLiked = false;
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
                Name = leaf.Name,
                Parents = leaf.Parents,
                Date = leaf.Date,
                MuseumNumber = leaf.MuseumNumber,
                Popularity = leaf.Popularity,
                AreMasterClassesHeld = leaf.AreMasterClassesHeld,
                Genres = leaf.Genres.ToArray()
            };
        }
    }
}
