using System.Threading.Tasks;
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
            DataContext = new ArtCardsViewModel(false, null);
            ((ArtCardsViewModel) DataContext).SnackBarMessageDisplayRequested +=
                (sender, e) =>
                {
                    var messageQueue = Snackbar.MessageQueue;
                    Task.Factory.StartNew(() =>
                        messageQueue.Enqueue(e.Message));
                };
        }

        public ArtCardsPage(bool recommendation, bool? areFavorites)
        {
            InitializeComponent();
            DataContext = new ArtCardsViewModel(recommendation, areFavorites);
            ((ArtCardsViewModel)DataContext).SnackBarMessageDisplayRequested +=
                (sender, e) =>
                {
                    var messageQueue = Snackbar.MessageQueue;
                    Task.Factory.StartNew(() =>
                        messageQueue.Enqueue(e.Message));
                };
        }

        public void Activate()
        {
            ((ArtCardsViewModel)DataContext).UpdateArtCards();
        }
    }
}
