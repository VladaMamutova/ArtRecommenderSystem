namespace ArtRecommenderSystem.Models
{
    internal abstract class ArtItem
    {
        public string Name { get; set; }
        public string[] Parents { get; set; }

        protected ArtItem()
        {
            Name = "";
            Parents = new string[0];
        }

        public override string ToString() => Name;
    }
}
