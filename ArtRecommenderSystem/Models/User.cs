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

        public override string ToString()
        {
            return $"Id={Id}, Nickname=\"{Nickname}\", " +
                   $"Interests={{{string.Join(", ", Interests)}}})";
        }
    }
}
