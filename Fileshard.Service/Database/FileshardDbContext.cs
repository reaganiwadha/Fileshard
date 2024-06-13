using Dorssel.EntityFrameworkCore;
using Fileshard.Service.Entities;
using Fileshard.Service.Migrations;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fileshard.Service.Database
{
    internal class FileshardDbContext : DbContext
    {
        public DbSet<EntityFileshardCollection> Collections => Set<EntityFileshardCollection>();

        public DbSet<EntityFileshardObject> Objects => Set<EntityFileshardObject>();

        public DbSet<EntityFileshardFile> Files => Set<EntityFileshardFile>();

        public DbSet<EntityFileshardFileMeta> FileMetas => Set<EntityFileshardFileMeta>();

        public DbSet<EntityFileshardTag> Tags => Set<EntityFileshardTag>();

        public DbSet<EntityFileshardObjectTag> ObjectTags => Set<EntityFileshardObjectTag>();

        public DbSet<EntityFileshardTagNamespace> TagNamespaces => Set<EntityFileshardTagNamespace>();

        public FileshardDbContext()
        {
            Migrate();
        }

        private void Migrate()
        {
            var serviceProvider = new ServiceCollection();
            serviceProvider.AddFluentMigratorCore()
                             .ConfigureRunner(rb => rb
                                    .AddSQLite()
                                    .WithGlobalConnectionString($"Data Source={GetDatabasePath()}")
                                    .ScanIn(typeof(CreateFileshardCollectionTable).Assembly).For.Migrations())
                             .AddLogging(lb => lb.AddFluentMigratorConsole());

            var sp = serviceProvider.BuildServiceProvider();
            var runner = sp.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*base.OnModelCreating(modelBuilder);*/

            modelBuilder.Entity<EntityFileshardCollection>().ToTable("collections");

            modelBuilder.Entity<EntityFileshardObject>()
                .HasMany(o => o.Files)
                .WithOne(f => f.FileshardObject)
                .HasForeignKey(f => f.ObjectId);

            modelBuilder.Entity<EntityFileshardObject>()
                .HasMany(o => o.Tags)
                .WithOne(t => t.FileshardObject)
                .HasForeignKey(t => t.ObjectId);

            modelBuilder.Entity<EntityFileshardFile>()
                .HasMany(f => f.Metas)
                .WithOne(m => m.FileshardFile)
                .HasForeignKey(m => m.FileId);

            modelBuilder.Entity<EntityFileshardCollection>()
                .HasMany(c => c.Objects)
                .WithOne(o => o.Collection)
                .HasForeignKey(o => o.CollectionId);

            modelBuilder.Entity<EntityFileshardFile>()
                .HasOne(f => f.FileshardObject)
                .WithMany(o => o.Files)
                .HasForeignKey(f => f.ObjectId);

            modelBuilder.Entity<EntityFileshardTagNamespace>()
                .HasMany(tn => tn.Tags)
                .WithOne(t => t.Namespace)
                .HasForeignKey(t => t.NamespaceId);
        }

        

        private string GetDatabasePath()
        {
            string tempPath = Path.GetTempPath();
            string dbPath = Path.Combine(tempPath, "fileshard.db");

            return dbPath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={GetDatabasePath()}")
                .UseSqliteTimestamp();
        }
    }
}
