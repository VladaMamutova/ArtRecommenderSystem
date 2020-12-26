using System;
using System.Collections.ObjectModel;
using System.Threading;
using ArtRecommenderSystem.Logic;
using ArtRecommenderSystem.Utilities;

namespace ArtRecommenderSystem.ViewModels
{
    public class SmartAssistantViewModel : ViewModelBase
    {
        private ObservableCollection<Message> _messages;
        private string _question;
        private RelayCommand _askCommand;
        
        public ObservableCollection<Message> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public string Question
        {
            get => _question;
            set
            {
                _question = value;
                OnPropertyChanged(nameof(Question));
            }
        }

        public RelayCommand AskCommand
        {
            get
            {
                return _askCommand ??
                       (_askCommand = new RelayCommand(o => Ask()));
            }
        }

        public event EventHandler MessageSent;

        public SmartAssistantViewModel()
        {
            Messages = new ObservableCollection<Message>();
            Messages.Add(new Message("Сколько музеев, в которых можно посмотреть витраж?", true));
            Messages.Add(new Message("Не очень много, всего 6 музеев."));
            Messages.Add(new Message("Это не популярный вид искусства?", true));
            Messages.Add(new Message("Да, он мало распространён сейчас."));
            Messages.Add(new Message("А мастер-классы проводятся?", true));
            Messages.Add(new Message("Да, мастер-классы проводятся."));

            // Сразу же в отдельном потоке создаём объект SmartAssistant,
            // так как объекту морфологического анализатора, который
            // инициализируется в ассистенте, необходимо дополнительное
            // время для первой загрузки.
            new Thread(o =>
            {
                var instance = SmartAssistant.Instance;
            }).Start();
        }

        private void Ask()
        {
            if (string.IsNullOrWhiteSpace(Question)) return;

            Messages.Add(new Message(Question, true));
            MessageSent?.Invoke(this, EventArgs.Empty);

            string answer = SmartAssistant.Instance.Answer(Question);
            Messages.Add(new Message(answer));
            Question = "";
        }
    }
}
