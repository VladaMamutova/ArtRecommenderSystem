using System.Threading.Tasks;
using System.Windows.Controls;
using ArtRecommenderSystem.ViewModels;

namespace ArtRecommenderSystem.Views
{
    // Базовый класс фабрики.
    // Бизнес-логика не зависит от конкретных классов продуктов,
    // так как ArtCardsViewModel — это абстрактный базовый класс
    // модели представления списка видов искусств.
    public abstract class ArtCardsPage : Page
    {
        protected ArtCardsPage()
        {
            InitializeDataContext();
        }

        // Фабричный метод.
        protected abstract ArtCardsViewModel CreateViewModel();

        public void InitializeDataContext()
        {
            var viewModel = CreateViewModel();
            viewModel.SnackBarMessageDisplayRequested += (sender, e) =>
            {
                Task.Factory.StartNew(() =>
                    viewModel.MessageQueue.Enqueue(e.Message));
            };

            DataContext = viewModel;
        }

        public void Activate()
        {
            ((ArtCardsViewModel)DataContext).MessageQueue.Clear();
            ((ArtCardsViewModel)DataContext).UpdateArtCards();
        }
    }
}
