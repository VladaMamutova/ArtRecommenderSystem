using System;
using System.Linq;
using ArtRecommenderSystem.Database;
using ArtRecommenderSystem.Models;

namespace ArtRecommenderSystem.ViewModels
{
    class MainViewModel : ArtCardsViewModel
    {
        public MainViewModel()
        {
            var artCards = ApplicationContext.GetInstance().Arts
                .Select(BuildArtRecord);
            foreach (var artCard in artCards)
            {
                ArtCards.Add(artCard);
            }
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

            var preferences = ApplicationContext.GetInstance().GetPreferences();

            foreach (var artCard in ArtCards)
            {
                artCard.IsLiked = false;
                artCard.IsDisliked = false;
            }

            foreach (var preference in preferences)
            {
                var artCard = ArtCards.First(art => art.Id == preference.ArtId);
                artCard.IsLiked = preference.Like == 1;
                artCard.IsDisliked = preference.Like == 0;
            }

            LastChangedTime = DateTime.Now;
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
