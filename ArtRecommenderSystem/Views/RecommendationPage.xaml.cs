﻿using ArtRecommenderSystem.ViewModels;

namespace ArtRecommenderSystem.Views
{
    /// <summary>
    /// Логика взаимодействия для RecommendationPage.xaml
    /// </summary>
    public partial class RecommendationPage : ArtCardsPage
    {
        public RecommendationPage()
        {
            InitializeComponent();
        }

        protected override ArtCardsViewModel CreateViewModel()
        {
            return new RecommendationsViewModel();
        }
    }
}
