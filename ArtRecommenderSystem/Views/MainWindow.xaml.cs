using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ArtRecommenderSystem.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainPage mainPage;
        private RecommendationPage recommendationPage;
        private MyGalleryPage myGalleryPage;

        public MainWindow()
        {
            InitializeComponent();
            mainPage = new MainPage();
            recommendationPage = new RecommendationPage();
            myGalleryPage = new MyGalleryPage();
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
            ContentFrame.Navigate(mainPage);
        }

        private void RecommendationPage_OnChecked(object sender, RoutedEventArgs e)
        {
            MyGalleryRadioButton.IsChecked = false;
            ContentFrame.Navigate(recommendationPage);
        }

        private void MyGalleryPage_OnChecked(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(myGalleryPage);
        }
    }
}
