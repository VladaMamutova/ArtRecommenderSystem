using ArtRecommenderSystem.ViewModels;

namespace ArtRecommenderSystem.Views
{
    /// <summary>
    /// Логика взаимодействия для BlacklistPage.xaml
    /// </summary>
    public partial class BlacklistPage : ArtCardsPage
    {
        public BlacklistPage()
        {
            InitializeComponent();
        }

        protected override ArtCardsViewModel CreateViewModel()
        {
            return new BlacklistViewModel();
        }
    }
}
