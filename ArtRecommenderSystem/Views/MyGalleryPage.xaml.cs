using System.Windows;
using System.Windows.Controls;

namespace ArtRecommenderSystem.Views
{
    /// <summary>
    /// Логика взаимодействия для MyGalleryPage.xaml
    /// </summary>
    public partial class MyGalleryPage : Page
    {
        private readonly ArtCardsPage _favoritesPage;
        private readonly ArtCardsPage _blacklistPage;
        
        public MyGalleryPage()
        {
            InitializeComponent();
            _favoritesPage = new ArtCardsPage(true);
            _blacklistPage = new ArtCardsPage(false);

            FavoritesToggleButton.IsChecked = true;
        }

        private void FavoritesToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(_favoritesPage);
            _favoritesPage.Activate();
        }

        private void BlacklistToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(_blacklistPage);
            _blacklistPage.Activate();
        }

        public void Activate()
        {
            if (FavoritesToggleButton.IsChecked != null &&
                FavoritesToggleButton.IsChecked.Value)
            {
                _favoritesPage.Activate();
            }
            else
            {
                _blacklistPage.Activate();
            }
        }
    }
}
