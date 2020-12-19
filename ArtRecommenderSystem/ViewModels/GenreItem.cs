using System.ComponentModel;
using System.Runtime.CompilerServices;
using ArtRecommenderSystem.Models;

namespace ArtRecommenderSystem.ViewModels
{
    class GenreItem : INotifyPropertyChanged
    {
        private Genres _genre;
        private bool _isChecked;

        public Genres Genre
        {
            get => _genre;
            set
            {
                _genre = value;
                OnPropertyChanged(nameof(Genre));
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

        public GenreItem(Genres genre)
        {
            _genre = genre;
            _isChecked = false;
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
