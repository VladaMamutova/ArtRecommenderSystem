﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ArtRecommenderSystem.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ArtCardsPage _mainPage;
        private readonly ArtCardsPage _recommendationPage;
        private readonly SmartAssistantPage _smartAssistantPage;
        private readonly MyGalleryPage _myGalleryPage;

        public MainWindow()
        {
            InitializeComponent();

            _mainPage = new MainArtCardsPage();
            _recommendationPage = new RecommendationPage();
            _smartAssistantPage = new SmartAssistantPage();
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

        private void SmartAssistantPage_OnChecked(object sender, RoutedEventArgs e)
        {
            MyGalleryRadioButton.IsChecked = false;
            ContentFrame.Navigate(_smartAssistantPage);
            _smartAssistantPage.Activate();
        }

        private void MyGalleryPage_OnChecked(object sender, RoutedEventArgs e)
        {
            MainRadioButton.IsChecked = false;
            RecommendationRadioButton.IsChecked = false;
            SmartAssistantRadioButton.IsChecked = false;
            ContentFrame.Navigate(_myGalleryPage);
            _myGalleryPage.Activate();
        }
    }
}
