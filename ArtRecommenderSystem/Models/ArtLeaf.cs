using System.Collections.Generic;

namespace ArtRecommenderSystem.Models
{
    internal class ArtLeaf: ArtItem
    {
        public int Date { get; set; }
        public int MuseumNumber { get; set; }
        public bool AreMasterClassesHeld { get; set; }
        public PopularityEnum Popularity { get; set; }
        public List<Genres> Genres { get; set; }
        public List<string> Materials { get; set; } //TODO
    }
}
