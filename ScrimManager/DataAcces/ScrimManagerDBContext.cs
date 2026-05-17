using Microsoft.EntityFrameworkCore;
using ScrimManager.Domain;

namespace ScrimManager.Data
{
    public class ScrimManagerDbContext : DbContext
    {
        public ScrimManagerDbContext(DbContextOptions<ScrimManagerDbContext> options)
            : base(options) { }

        public DbSet<Tournament> Tournaments { get; set; }
    }
}