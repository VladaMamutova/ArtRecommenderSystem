using ArtRecommenderSystem.ViewModels;

namespace ArtRecommenderSystem.Views
{
    /// <summary>
    /// Логика взаимодействия для FavoritesPage.xaml
    /// </summary>
    public partial class FavoritesPage : ArtCardsPage
    {
        public FavoritesPage()
        {
            InitializeComponent();
        }

        protected override ArtCardsViewModel CreateViewModel()
        {
            return new FavoritesViewModel();
        }
    }
}
