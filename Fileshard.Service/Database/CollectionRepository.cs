using Fileshard.Service.Entities;
using Fileshard.Service.Repository;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace Fileshard.Service.Database
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly FileshardDbContext _dbContext = new FileshardDbContext();

        public CollectionRepository()
        {
        }

        public Task<Guid> Create(string title)
        {
            var collection = new FileshardCollection
            {
                Title = title
            };

            _dbContext.Collections.Add(collection);
            _dbContext.SaveChanges();

            return Task.FromResult(collection.Id);
        }

        public Task<List<FileshardCollection>> GetAll()
        {
            return Task.FromResult(_dbContext.Collections.ToList());
        }

        public async Task InsertMeta(FileshardFileMeta meta)
        {
            var existingMeta = await _dbContext.FileMetas
                .FirstOrDefaultAsync(m => m.Key == meta.Key && m.FileId == meta.FileId);

            if (existingMeta != null)
            {
                existingMeta.Value = meta.Value;
                _dbContext.FileMetas.Update(existingMeta);
            }
            else
            {
                _dbContext.FileMetas.Add(meta);
            }

            await _dbContext.SaveChangesAsync();
        }

        public Task UpdateFile(FileshardFile file)
        {
            var existing = _dbContext.Files.FindAsync(file.Id).Result;
            _dbContext.Entry(existing).CurrentValues.SetValues(file);
            
            return _dbContext.SaveChangesAsync();
        }

        public Task<FileshardObject?> GetObject(Guid collectionId, Guid objectId)
        {
            return Task.FromResult(_dbContext.Objects
                        .Where(o => o.CollectionId == collectionId)
                        .Include(o => o.Files)
                        .FirstOrDefault(o => o.Id == objectId));
        }

        public Task<List<String>> FilterNonExistentFiles(List<String> files)
        {
               return Task.FromResult(files
                            .Where(f => !_dbContext.Files.Any(file => file.InternalPath == f))
                            .ToList());
        }

        public async Task<List<FileshardObject>> GetObjects(Guid collectionId)
        {
            var objects = await _dbContext.Objects
                .Where(o => o.CollectionId == collectionId)
                .Where(o => o.Files.Count != 0)
                .Include(o => o.Files)
                .ThenInclude(f => f.Metas)
                .OrderBy(o => o.Files.First().InternalPath)
                .ToListAsync();

            foreach (var obj in objects)
            {
                foreach (var file in obj.Files)
                {
                    file.Metas = file.Metas
                        .GroupBy(m => new { m.FileId, m.Key })
                        .Select(g => g.First())
                        .ToList();
                }
            }

            return objects;
        }

        public Task Ingest(Guid collectionId, List<FileshardObject> fileshardObjects)
        {
            _dbContext.Objects.AddRange(
                fileshardObjects
                    .Select(obj => { obj.CollectionId = collectionId; return obj; })
            );

            return _dbContext.SaveChangesAsync();
        }

        public Task<bool> IsEmpty()
        {
            return Task.FromResult(!_dbContext.Collections.Any());
        }
    }
}
