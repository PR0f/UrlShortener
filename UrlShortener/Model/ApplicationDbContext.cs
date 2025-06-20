using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Model
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Shortener> Shortener { get; set; }
    }
}
