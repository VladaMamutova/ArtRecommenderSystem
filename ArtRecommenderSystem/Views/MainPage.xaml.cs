using System.Windows.Controls;
using ArtRecommenderSystem.ViewModels;

namespace ArtRecommenderSystem.Views
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainPageViewModel();
        }

        public MainPage(bool isFavorite)
        {
            InitializeComponent();
            DataContext = new MainPageViewModel(isFavorite);
        }
    }
}
