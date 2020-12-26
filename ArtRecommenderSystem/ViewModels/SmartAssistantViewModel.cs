using System.Collections.ObjectModel;

namespace ArtRecommenderSystem.ViewModels
{
    public class SmartAssistantViewModel : ViewModelBase
    {
        private ObservableCollection<Message> _messages;

        public ObservableCollection<Message> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public SmartAssistantViewModel()
        {
            Messages = new ObservableCollection<Message>();
            Messages.Add(new Message("Сколько музеев, в которых можно посмотреть витраж?", true));
            Messages.Add(new Message("Не очень много, всего 6 музеев."));
            Messages.Add(new Message("Это не популярный вид искусства?", true));
            Messages.Add(new Message("Да, он мало распространён сейчас."));
            Messages.Add(new Message("А мастер-классы проводятся?", true));
            Messages.Add(new Message("Да, мастер-классы проводятся."));
        }
    }
}
