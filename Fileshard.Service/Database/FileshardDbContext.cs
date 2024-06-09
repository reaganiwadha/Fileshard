using Fileshard.Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fileshard.Service.Database
{
    public class FileshardDbContext : DbContext
    {
        public DbSet<Collection> Collections => Set<Collection>();

        public FileshardDbContext()
        {
            // Bad practice but ok for now
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(
          DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(
                "Data Source=fileshard.db");
        }
    }
}
