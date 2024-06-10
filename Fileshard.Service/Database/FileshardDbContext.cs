using Dorssel.EntityFrameworkCore;
using Fileshard.Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fileshard.Service.Database
{
    internal class FileshardDbContext : DbContext
    {
        public DbSet<EntityFileshardCollection> Collections => Set<EntityFileshardCollection>();

        public DbSet<EntityFileshardObject> Objects => Set<EntityFileshardObject>();

        public DbSet<EntityFileshardFile> Files => Set<EntityFileshardFile>();

        public DbSet<EntityFileshardFileMeta> FileMetas => Set<EntityFileshardFileMeta>();

        public FileshardDbContext()
        {
            // Bad practice but ok for now
            Database.EnsureCreated();
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*base.OnModelCreating(modelBuilder);*/

            modelBuilder.Entity<EntityFileshardCollection>().ToTable("collections");

            modelBuilder.Entity<EntityFileshardObject>()
                .HasMany(o => o.Files)
                .WithOne(f => f.FileshardObject)
                .HasForeignKey(f => f.ObjectId);

            modelBuilder.Entity<EntityFileshardFile>()
                .HasMany(f => f.Metas)
                .WithOne(m => m.FileshardFile)
                .HasForeignKey(m => m.FileId);

            modelBuilder.Entity<EntityFileshardFile>()
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
