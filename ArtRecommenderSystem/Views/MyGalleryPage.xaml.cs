using System.Windows;
using System.Windows.Controls;

namespace ArtRecommenderSystem.Views
{
    /// <summary>
    /// Логика взаимодействия для MyGalleryPage.xaml
    /// </summary>
    public partial class MyGalleryPage : Page
    {
        private readonly MainPage _favoritePage;
        private readonly MainPage _blacklistPage;
        
        public MyGalleryPage()
        {
            InitializeComponent();
            _favoritePage = new MainPage(true);
            _blacklistPage = new MainPage(false);

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
