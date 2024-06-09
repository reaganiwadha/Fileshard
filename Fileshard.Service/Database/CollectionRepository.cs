using Fileshard.Service.Entities;
using Fileshard.Service.Repository;
using Microsoft.EntityFrameworkCore;

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

        public Task<FileshardObject?> GetObject(Guid collectionId, Guid objectId)
        {
            return Task.FromResult(_dbContext.Objects
                        .Where(o => o.CollectionId == collectionId)
                        .Include(o => o.Files)
                        .FirstOrDefault(o => o.Id == objectId));
        }

        public Task<List<FileshardObject>> GetObjects(Guid collectionId)
        {
            return Task.FromResult(_dbContext.Objects
                .Where(o => o.CollectionId == collectionId)
                .Where(o => o.Files.Count != 0)
                .Include(o => o.Files)
                .ToList());
        }

        public Task Ingest(Guid collectionId, List<FileshardObject> fileshardObjects)
        {
            _dbContext.Objects.AddRange(
                fileshardObjects.Select(obj => { obj.CollectionId = collectionId; return obj; })
            );

            return _dbContext.SaveChangesAsync();
        }

        public Task<bool> IsEmpty()
        {
            return Task.FromResult(!_dbContext.Collections.Any());
        }
    }
}
