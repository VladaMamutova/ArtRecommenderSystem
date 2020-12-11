namespace ArtRecommenderSystem.Models
{
    public class ArtWork
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public int Date { get; set; }
        public int MuseumNumber { get; set; }
        public int PopularityId { get; set; }
        public int MasterClasses { get; set; }
    }
}
