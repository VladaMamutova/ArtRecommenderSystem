using System.ComponentModel;
using System.Runtime.CompilerServices;
using ArtRecommenderSystem.Models;

namespace ArtRecommenderSystem.ViewModels
{
    public class ArtCard : INotifyPropertyChanged
    {
        private bool _isLiked;
        public bool IsLiked
        {
            get => _isLiked;
            set
            {
                _isLiked = value;
                OnPropertyChanged(nameof(IsLiked));
            }
        }

        private bool _isDisliked;
        public bool IsDisliked
        {
            get => _isDisliked;
            set
            {
                _isDisliked = value;
                OnPropertyChanged(nameof(IsDisliked));
            }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Parents { get; set; }
        public int Date { get; set; }
        public int MuseumNumber { get; set; }
        public bool AreMasterClassesHeld { get; set; }
        public PopularityEnum Popularity { get; set; }
        public Genres[] Genres { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
