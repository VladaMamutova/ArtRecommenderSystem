using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ArtRecommenderSystem.Database;

namespace ArtRecommenderSystem.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ArtCardsPage _mainPage;
        private readonly ArtCardsPage _recommendationPage;
        private readonly MyGalleryPage _myGalleryPage;

        public MainWindow()
        {
            InitializeComponent();

            _mainPage = new ArtCardsPage();
            _recommendationPage = new ArtCardsPage(true, null);
            _myGalleryPage = new MyGalleryPage();
            MainRadioButton.IsChecked = true;
        }

        private void Move(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonExpand_OnClick(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                ((Button)sender).ToolTip = "Развернуть";
            }
            else
            {
                WindowState = WindowState.Maximized;
                ((Button)sender).ToolTip = "Свернуть";
            }
        }

        private void MainPage_OnChecked(object sender, RoutedEventArgs e)
        {
            MyGalleryRadioButton.IsChecked = false;
            ContentFrame.Navigate(_mainPage);
            _mainPage.Activate();
        }

        private void RecommendationPage_OnChecked(object sender, RoutedEventArgs e)
        {
            MyGalleryRadioButton.IsChecked = false;
            ContentFrame.Navigate(_recommendationPage);
            _recommendationPage.Activate();
        }

        private void MyGalleryPage_OnChecked(object sender, RoutedEventArgs e)
        {
            MainRadioButton.IsChecked = false;
            RecommendationRadioButton.IsChecked = false;
            ContentFrame.Navigate(_myGalleryPage);
            _myGalleryPage.Activate();
        }

        private void SetUser(string login)
        {
            if (ApplicationContext.GetInstance().SetUser(login) !=
                true)
            {
                MessageBox.Show("Пользователь с логином \"" + login +
                                "\" не найден.");
            }
        }
    }
}
