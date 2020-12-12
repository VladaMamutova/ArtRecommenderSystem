using System.Collections.Generic;

namespace ArtRecommenderSystem.Models
{
    public class ArtLeaf: ArtItem
    {
        public int Id { get; set; }
        public int Date { get; set; }
        public int MuseumNumber { get; set; }
        public bool AreMasterClassesHeld { get; set; }
        public PopularityEnum Popularity { get; set; }
        public List<Genres> Genres { get; set; }
    }
}
