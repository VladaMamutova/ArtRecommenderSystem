using System.Data.Entity;
using ArtRecommenderSystem.Models;

namespace ArtRecommenderSystem.Database
{
    public class ApplicationContext : DbContext
    {
        public DbSet<ArtWork> ArtWorks { get; set; }
        public DbSet<ArtType> ArtTypes { get; set; }
        public DbSet<PopularityType> PopularityTypes { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<WorkGenre> WorkGenres { get; set; }

        public ApplicationContext() : base("DefaultConnection") { }
    }
}
