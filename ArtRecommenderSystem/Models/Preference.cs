using System.ComponentModel.DataAnnotations.Schema;

namespace ArtRecommenderSystem.Models
{
    public class Preference
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("art_id")]
        public int ArtId { get; set; }
        public int Like { get; set; } // 0 - false, 1 - true

        public Preference() { }

        public Preference(int userId, int artId, int like)
        {
            UserId = userId;
            ArtId = artId;
            Like = like;
        }
    }
}
