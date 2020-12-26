namespace ArtRecommenderSystem.ViewModels
{
    public class Message
    {
        public string Text { get; }
        public bool SentByMe { get; }

        public Message(string text, bool sentByMe = false)
        {
            Text = text;
            SentByMe = sentByMe;
        }
    }
}
