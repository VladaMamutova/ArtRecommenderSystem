using System;
using System.Collections.ObjectModel;
using System.Linq;
using ArtRecommenderSystem.Database;
using ArtRecommenderSystem.Logic;
using ArtRecommenderSystem.Models;

namespace ArtRecommenderSystem.ViewModels
{
    class RecommendationsViewModel : ArtCardsViewModel
    {
        private string _interests;

        // 0 - коллаборативная фильтрация, 1 - контент-ориентированная фильтрация
        private int _algorithmIndex;

        private bool _forceUpdate;

        public string Interests
        {
            get => _interests;
            set
            {
                _interests = value;
                OnPropertyChanged(nameof(Interests));
            }
        }

        public int AlgorithmIndex
        {
            get => _algorithmIndex;
            set
            {
                if (value != -1)
                {
                    _algorithmIndex = value;
                    OnPropertyChanged(nameof(AlgorithmIndex));
                    OnPropertyChanged(nameof(IsCollaborativeFilteringSelected));
                    _forceUpdate = true;
                    UpdateArtCards();
                    _forceUpdate = false;
                }
            }
        }

        public bool IsCollaborativeFilteringSelected => _algorithmIndex == 0;

        public RecommendationsViewModel()
        {
            AlgorithmIndex = 0;
        }

        public override void Like(ArtCard artCard)
        {
            if (artCard.IsLiked)
            {
                artCard.IsDisliked = false;
                ApplicationContext.GetInstance()
                    .UpdatePreference(artCard.Id, artCard.IsLiked);
                LastChangedTime = DateTime.Now;

                ShowLikeMessage(artCard.Name);
            }
            else
            {
                ApplicationContext.GetInstance().RemovePreference(artCard.Id);
                LastChangedTime = DateTime.Now;

                ShowRemoveLikeMessage(artCard.Name);
            }
        }

        public override void Dislike(ArtCard artCard)
        {
            if (artCard.IsDisliked)
            {
                artCard.IsLiked = false;
                ApplicationContext.GetInstance()
                    .UpdatePreference(artCard.Id, artCard.IsLiked);
                ArtCards.Remove(artCard);
                LastChangedTime = DateTime.Now;

                ShowDislikeMessage(artCard.Name);
            }
            else
            {
                ApplicationContext.GetInstance().RemovePreference(artCard.Id);
                LastChangedTime = DateTime.Now;

                ShowRemoveDislikeMessage(artCard.Name);
            }
        }

        public override bool IsUpToDate()
        {
            return ApplicationContext.GetInstance().FavoritesChangedTime <
                   LastChangedTime &&
                   ApplicationContext.GetInstance().BlacklistChangedTime <
                   LastChangedTime;
        }

        public override void UpdateArtCards()
        {
            if (IsUpToDate() && !_forceUpdate) return;
            {
                var arts = ApplicationContext.GetInstance().Arts;
                var recommendationEngine = IsCollaborativeFilteringSelected
                    ? (IRecommendationEngine) new CollaborativeFiltering()
                    : new ContentBasedFiltering();

                var recommendations = recommendationEngine.Recommend();
                var blackList = ApplicationContext.GetInstance().GetBlacklist();

                ArtCards = new ObservableCollection<ArtCard>();
                foreach (var id in recommendations)
                {
                    var artLeaf = arts.Find(art => art.Id == id);
                    if (blackList.All(art => art.ArtId != artLeaf.Id))
                    {
                        ArtCards.Add(BuildArtRecord(artLeaf));
                    }
                }

                if (IsCollaborativeFilteringSelected)
                {
                    Interests =
                        string.Join(", ",
                            ((CollaborativeFiltering) recommendationEngine)
                            .RetrieveLastInterests());
                }

                LastChangedTime = DateTime.Now;
            }
        }

        private ArtCard BuildArtRecord(ArtLeaf leaf)
        {
            return new ArtCard
            {
                Id = leaf.Id,
                Name = leaf.Name,
                Parents = leaf.Parents,
                Date = leaf.Date,
                MuseumNumber = leaf.MuseumNumber,
                Popularity = leaf.Popularity,
                AreMasterClassesHeld = leaf.AreMasterClassesHeld,
                Genres = leaf.Genres.ToArray()
            };
        }
    }
}
