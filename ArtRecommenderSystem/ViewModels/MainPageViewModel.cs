using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ArtRecommenderSystem.Models;
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
                IsChecked = false,
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
