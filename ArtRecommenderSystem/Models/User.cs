using System.ComponentModel.DataAnnotations.Schema;

namespace ArtRecommenderSystem.Models
{
    public class User
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("nickname")]
        public string Nickname { get; set; }
        [Column("interests")]
        public string Interests { get; set; }
    }
}
