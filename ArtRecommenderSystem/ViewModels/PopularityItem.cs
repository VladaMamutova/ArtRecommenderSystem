using System.ComponentModel;
using System.Runtime.CompilerServices;
using ArtRecommenderSystem.Models;

namespace ArtRecommenderSystem.ViewModels
{
    class PopularityItem : INotifyPropertyChanged
    {
        private PopularityEnum _popularity;
        private bool _isChecked;

        public PopularityEnum Popularity
        {
            get => _popularity;
            set
            {
                _popularity = value;
                OnPropertyChanged(nameof(Popularity));
            }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public PopularityItem(PopularityEnum popularity)
        {
            _popularity = popularity;
            IsChecked = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
