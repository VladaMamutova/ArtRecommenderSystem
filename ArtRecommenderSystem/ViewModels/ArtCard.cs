using System.ComponentModel;
using System.Runtime.CompilerServices;
using ArtRecommenderSystem.Models;

namespace ArtRecommenderSystem.ViewModels
{
    public class ArtCard : INotifyPropertyChanged
    {
        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

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
