namespace ArtRecommenderSystem.ViewModels
{
    public interface IArtCardsViewModel
    {
        void Like(ArtCard artCard);
        void Dislike(ArtCard artCard);
        bool IsUpToDate();
        void UpdateArtCards();

        void ShowLikeMessage(string artCardName);
        void ShowRemoveLikeMessage(string artCardName);
        void ShowDislikeMessage(string artCardName);
        void ShowRemoveDislikeMessage(string artCardName);
    }
}
