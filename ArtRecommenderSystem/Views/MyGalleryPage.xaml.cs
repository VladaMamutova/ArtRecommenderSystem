using System.Windows;
using System.Windows.Controls;

namespace ArtRecommenderSystem.Views
{
    /// <summary>
    /// Логика взаимодействия для MyGalleryPage.xaml
    /// </summary>
    public partial class MyGalleryPage : Page
    {
        private readonly ArtCardsPage _favoritePage;
        private readonly ArtCardsPage _blacklistPage;
        
        public MyGalleryPage()
        {
            InitializeComponent();
            _favoritePage = new ArtCardsPage(true);
            _blacklistPage = new ArtCardsPage(false);

            PreferencesToggleButton.IsChecked = true;
        }

        private void PreferencesToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(_favoritePage);
        }

        private void BlacklistToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(_blacklistPage);
        }
    }
}
