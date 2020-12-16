using System;
using System.Collections.ObjectModel;
using System.Linq;
using ArtRecommenderSystem.Database;
using ArtRecommenderSystem.Logic;
using ArtRecommenderSystem.Models;

namespace ArtRecommenderSystem.ViewModels
{
    class RecommendationsViewModel: ArtCardsViewModel
    {
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
            if (IsUpToDate()) return;
            {
                var arts = ApplicationContext.GetInstance().Arts;
                var recommendedArtIds =
                    RecommendationEngine.CollaborativeFiltering();
                var blackList = ApplicationContext.GetInstance().GetBlacklist();

                ArtCards = new ObservableCollection<ArtCard>();
                foreach (var id in recommendedArtIds)
                {
                    var artLeaf = arts.Find(art => art.Id == id);
                    if (blackList.All(art => art.ArtId != artLeaf.Id))
                    {
                        ArtCards.Add(BuildArtRecord(artLeaf));
                    }
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
