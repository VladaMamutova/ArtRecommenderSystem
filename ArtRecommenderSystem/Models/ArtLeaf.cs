using System;
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

        protected bool Equals(ArtLeaf other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ArtLeaf) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
