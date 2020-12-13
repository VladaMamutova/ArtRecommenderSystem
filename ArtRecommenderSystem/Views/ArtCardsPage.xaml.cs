using System.Windows.Controls;
using ArtRecommenderSystem.ViewModels;

namespace ArtRecommenderSystem.Views
{
    /// <summary>
    /// Логика взаимодействия для ArtCardsPage.xaml
    /// </summary>
    public partial class ArtCardsPage : Page
    {
        public ArtCardsPage()
        {
            InitializeComponent();
            DataContext = new ArtCardsViewModel();
        }

        public ArtCardsPage(bool isFavorite)
        {
            InitializeComponent();
            DataContext = new ArtCardsViewModel(isFavorite);
        }
    }
}
