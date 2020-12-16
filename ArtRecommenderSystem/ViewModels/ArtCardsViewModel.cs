using System;
using System.Collections.ObjectModel;
using ArtRecommenderSystem.Utilities;
using MaterialDesignThemes.Wpf;

namespace ArtRecommenderSystem.ViewModels
{
    public abstract class ArtCardsViewModel: ViewModelBase, IArtCardsViewModel
    {
        private ObservableCollection<ArtCard> _artCards;
        private RelayCommand _likeCommand;
        private RelayCommand _dislikeCommand;

        protected DateTime LastChangedTime;

        public ObservableCollection<ArtCard> ArtCards
        {
            get => _artCards;
            set
            {
                _artCards = value;
                OnPropertyChanged(nameof(ArtCards));
            }
        }

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

        protected ArtCardsViewModel()
        {
            LastChangedTime = DateTime.MinValue;
            ArtCards = new ObservableCollection<ArtCard>();
            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2500));
        }

        public abstract void Like(ArtCard artCard);
        public abstract void Dislike(ArtCard artCard);
        public abstract bool IsUpToDate();
        public abstract void UpdateArtCards();

        public void ShowLikeMessage(string artCardName)
        {
            OnSnackBarMessageDisplayRequest(
                $"Вид искусства \"{artCardName}\" добавлен " +
                "в раздел \"Понравившиеся\" Моей галереи");
        }

        public void ShowRemoveLikeMessage(string artCardName)
        {
            OnSnackBarMessageDisplayRequest(
                $"Вид искусства \"{artCardName}\" удалён из раздела " +
                "\"Понравившиеся\" Моей галереи");
        }

        public void ShowDislikeMessage(string artCardName)
        {
            OnSnackBarMessageDisplayRequest(
                $"Вид искусства \"{artCardName}\" добавлен в \"Чёрный список\" и больше не будет " +
                "появляться в ваших рекомендациях");
        }

        public void ShowRemoveDislikeMessage(string artCardName)
        {
            OnSnackBarMessageDisplayRequest(
                $"Вид искусства \"{artCardName}\" удалён из \"Чёрного списка\" и теперь " +
                "сможет появляться в ваших рекомендациях");
        }
    }
}
