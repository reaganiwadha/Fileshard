using Dorssel.EntityFrameworkCore;
using Fileshard.Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fileshard.Service.Database
{
    public class FileshardDbContext : DbContext
    {
        public DbSet<FileshardCollection> Collections => Set<FileshardCollection>();

        public DbSet<FileshardObject> Objects => Set<FileshardObject>();

        public DbSet<FileshardFile> Files => Set<FileshardFile>();

        public DbSet<FileshardFileMeta> FileMetas => Set<FileshardFileMeta>();

        public FileshardDbContext()
        {
            // Bad practice but ok for now
            Database.EnsureCreated();
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*base.OnModelCreating(modelBuilder);*/

            modelBuilder.Entity<FileshardCollection>().ToTable("collections");

            modelBuilder.Entity<FileshardObject>()
                .HasMany(o => o.Files)
                .WithOne(f => f.FileshardObject)
                .HasForeignKey(f => f.ObjectId);

            modelBuilder.Entity<FileshardFile>()
                .HasMany(f => f.Metas)
                .WithOne(m => m.FileshardFile)
                .HasForeignKey(m => m.FileId);

            modelBuilder.Entity<FileshardFile>()
                .HasOne(f => f.FileshardObject)
                .WithMany(o => o.Files)
                .HasForeignKey(f => f.ObjectId);
        }

        protected override void OnConfiguring(
          DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(
                "Data Source=fileshard.db")
                .UseSqliteTimestamp();
        }
    }
}
