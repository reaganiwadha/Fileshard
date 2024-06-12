using AutoMapper;
using Fileshard.Service.Entities;
using Fileshard.Service.Repository;
using Fileshard.Service.Structs;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace Fileshard.Service.Database
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly FileshardDbContext _dbContext = new FileshardDbContext();

        private readonly IMapper _mapper;

        public CollectionRepository()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<EntityFileshardCollection, FileshardCollection>();
                cfg.CreateMap<EntityFileshardObject, FileshardObject>();
                cfg.CreateMap<EntityFileshardFile, FileshardFile>();
                cfg.CreateMap<EntityFileshardFileMeta, FileshardFileMeta>();
                cfg.CreateMap<FileshardObject, EntityFileshardObject>();
                cfg.CreateMap<FileshardFile, EntityFileshardFile>();
            });
            _mapper = config.CreateMapper();
        }

        public Task<Guid> Create(string title)
        {
            var collection = new EntityFileshardCollection
            {
                Title = title
            };

            _dbContext.Collections.Add(collection);
            _dbContext.SaveChanges();

            return Task.FromResult(collection.Id);
        }

        public async Task<List<FileshardCollection>> GetAll()
        {
            var collections = _dbContext.Collections
                .ToList();

            return _mapper.Map<List<FileshardCollection>>(collections);
        }

        public async Task UpsertMeta(String key, String value, Guid fileId)
        {
            var existingMeta = await _dbContext.FileMetas
                .FirstOrDefaultAsync(m => m.Key == key && m.FileId == fileId);

            if (existingMeta != null)
            {
                existingMeta.Value = value;
                _dbContext.FileMetas.Update(existingMeta);
            }
            else
            {
                var meta = new EntityFileshardFileMeta
                {
                    Id = Guid.NewGuid(),
                    Key = key,
                    Value = value,
                    FileId = fileId,
                };

                _dbContext.FileMetas.Add(meta);
                await _dbContext.SaveChangesAsync();
            }
        }

        public Task<Boolean> FileHasMeta(String key, Guid fileId)
        { 
            return Task.FromResult(_dbContext.FileMetas.Any(m => m.Key == key && m.FileId == fileId));
        }

        public Task<Boolean> FileHasMetas(Guid fileId, params string[] keys)
        {
            return Task.FromResult(keys.All(key => _dbContext.FileMetas.Any(m => m.Key == key && m.FileId == fileId)));
        }

        public Task<FileshardObject?> GetObject(Guid collectionId, Guid objectId)
        {
            var obj = _dbContext.Objects
                .Where(o => o.CollectionId == collectionId)
                .Include(o => o.Files)
                .ThenInclude(f => f.Metas)
                .FirstOrDefault(o => o.Id == objectId);

            if (obj == null)
            {
                return Task.FromResult<FileshardObject?>(null);
            }

            return Task.FromResult<FileshardObject?>(_mapper.Map<FileshardObject>(obj));
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
                /*.OrderBy(o => o.Files.First().InternalPath)*/
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

            return _mapper.Map<List<FileshardObject>>(objects.Shuffle());
        }

        public Task Ingest(Guid collectionId, List<FileshardObject> fileshardObjects)
        {
            var entities = _mapper.Map<List<EntityFileshardObject>>(fileshardObjects);

            _dbContext.Objects.AddRange(
                entities
                    .Select(obj => { obj.CollectionId = collectionId; return obj; })
            );

            return _dbContext.SaveChangesAsync();
        }

        public Task<bool> IsEmpty()
        {
            return Task.FromResult(!_dbContext.Collections.Any());
        }

        public async Task UpsertMeta(string key, ulong value, Guid fileId)
        {
            var existingMeta = await _dbContext.FileMetas
                .FirstOrDefaultAsync(m => m.Key == key && m.FileId == fileId);

            if (existingMeta != null)
            {
                existingMeta.LongValue = value;
                _dbContext.FileMetas.Update(existingMeta);
            }
            else
            {
                var meta = new EntityFileshardFileMeta
                {
                    Id = Guid.NewGuid(),
                    Key = key,
                    LongValue = value,
                    FileId = fileId,
                };

                _dbContext.FileMetas.Add(meta);
                await _dbContext.SaveChangesAsync();
            }
        }   

        public async Task UpsertMeta(string key, DateTime value, Guid fileId)
        {
            var existingMeta = await _dbContext.FileMetas
           .FirstOrDefaultAsync(m => m.Key == key && m.FileId == fileId);

            if (existingMeta != null)
            {
                existingMeta.TimeValue = value;
                _dbContext.FileMetas.Update(existingMeta);
            }
            else
            {
                var meta = new EntityFileshardFileMeta
                {
                    Id = Guid.NewGuid(),
                    Key = key,
                    TimeValue = value,
                    FileId = fileId,
                };

                _dbContext.FileMetas.Add(meta);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
