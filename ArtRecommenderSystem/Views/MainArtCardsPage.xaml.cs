 using ArtRecommenderSystem.ViewModels;
 
 namespace ArtRecommenderSystem.Views
{
    /// <summary>
    /// Логика взаимодействия для MainArtCardsPage.xaml
    /// </summary>
    public partial class MainArtCardsPage : ArtCardsPage
    {
        public MainArtCardsPage()
        {
            InitializeComponent();
        }

        protected override ArtCardsViewModel CreateViewModel()
        {
            return new MainViewModel();
        }
    }
}
