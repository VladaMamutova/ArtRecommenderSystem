using System.Windows.Controls;
using ArtRecommenderSystem.ViewModels;

namespace ArtRecommenderSystem.Views
{
    /// <summary>
    /// Логика взаимодействия для SmartAssistantPage.xaml
    /// </summary>
    public partial class SmartAssistantPage : Page
    {
        public SmartAssistantPage()
        {
            InitializeComponent();
            DataContext = new SmartAssistantViewModel();
        }
    }
}
