using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Model
{
    public class Database : DbContext
    {
        public Database(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Shortener> Shortener { get; set; }
    }
}
