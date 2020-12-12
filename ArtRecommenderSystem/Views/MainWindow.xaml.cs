using System.Data.Entity;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ArtRecommenderSystem.Database;
using ArtRecommenderSystem.Models;
namespace ArtRecommenderSystem.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // ApplicationContext db;

        private MainPage mainPage;
        private RecommendationPage recommendationPage;
        private MyGalleryPage myGalleryPage;

        public MainWindow()
        {
            InitializeComponent();

            /*db = new ApplicationContext();
            db.ArtWorks.Load();*/
            //this.DataContext = db.ArtWorks.Local.ToBindingList();

            //var tree = File.ReadAllText("art.json");
            //var reader = new JsonTextReader(new StringReader(tree));
            //var root = JsonSerializer.CreateDefault()
            //    .Deserialize<ArtNode>(reader);
            //root.InitParents(new[] { root.Name });

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
            MainRadioButton.IsChecked = false;
            RecommendationRadioButton.IsChecked = false;
            ContentFrame.Navigate(myGalleryPage);
        }
    }
}
