using AutoMapper;
using Fileshard.Service.Entities;
using Fileshard.Service.Repository;
using Fileshard.Service.Structs;
using Mapster;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using Nelibur.ObjectMapper;
using System;
using System.Linq;

namespace Fileshard.Service.Database
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly FileshardDbContext _dbContext = new FileshardDbContext();

        private readonly IMapper _mapper;

        public CollectionRepository()
        {
            TinyMapper.Bind<EntityFileshardObject, FileshardObject>();

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<EntityFileshardCollection, FileshardCollection>(MemberList.Source);
                cfg.CreateMap<EntityFileshardObject, FileshardObject>(MemberList.Source);
                cfg.CreateMap<EntityFileshardFile, FileshardFile>(MemberList.Source);
                cfg.CreateMap<EntityFileshardFileMeta, FileshardFileMeta>(MemberList.Source);
                cfg.CreateMap<FileshardObject, EntityFileshardObject>(MemberList.Source);
                cfg.CreateMap<FileshardFile, EntityFileshardFile>(MemberList.Source);
                cfg.CreateMap<FileshardObjectTag, FileshardObjectTag>(MemberList.Source);
                cfg.CreateMap<FileshardTag, EntityFileshardTag>(MemberList.Source);
                cfg.CreateMap<FileshardTagNamespace, EntityFileshardTagNamespace>(MemberList.Source);

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
                await _dbContext.SaveChangesAsync();
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

        public async Task<FileshardObject?> GetObject(Guid collectionId, Guid objectId)
        {
            var obj = _dbContext.Objects
                .Where(o => o.CollectionId == collectionId)
                .Include(o => o.Files)
                .ThenInclude(f => f.Metas)
                .Include(o => o.Tags)
                .ThenInclude(t => t.Tag)
                .ThenInclude(t => t.Namespace)
                .FirstOrDefault(o => o.Id == objectId);

            if (obj == null)
            {
                return null;
            }

            return TinyMapper.Map<EntityFileshardObject, FileshardObject>(obj);
        }

        public async Task<bool> ObjectHasNamespaceAlready(Guid objectId, String namespaceName) {
            return await _dbContext.ObjectTags
                .Where(t => t.ObjectId == objectId)
                .Where(t => t.Tag.Namespace.Name == namespaceName)
                .AnyAsync();
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
                // .Include(o => o.Tags)
                /*.OrderBy(o => o.Files.First().InternalPath)*/
                .ToListAsync();

            foreach (var obj in objects)
            {
                foreach (var file in obj.Files)
                {
                    file.Metas = file.Metas
                        .GroupBy(m => new { m.FileId, m.Key })
                        .Select(g => g.First())
                        .OrderBy(m => m.Key)
                        .ToList();
                }
            }

            return objects.Select(e => TinyMapper.Map<EntityFileshardObject, FileshardObject>(e)).ToList();
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

        public async Task UpsertMeta(string key, long value, Guid fileId)
        {
            var existingMeta = await _dbContext.FileMetas
                .FirstOrDefaultAsync(m => m.Key == key && m.FileId == fileId);

            if (existingMeta != null)
            {
                existingMeta.LongValue = value;
                _dbContext.FileMetas.Update(existingMeta);
                await _dbContext.SaveChangesAsync();
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
                await _dbContext.SaveChangesAsync();
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

        public async Task UpsertObjectTag(string tagNamespace, string tagName, float? weight, Guid objectId)
        {
              var existingTag = _dbContext.ObjectTags
                .FirstOrDefault(t => t.ObjectId == objectId && t.Tag.Namespace.Name == tagNamespace && t.Tag.Name == tagName);

            if (existingTag != null)
            {
                existingTag.Weight = weight ?? existingTag.Weight;
                _dbContext.ObjectTags.Update(existingTag);
            }
            else
            {
                var tag = _dbContext.Tags
                    .FirstOrDefault(t => t.Namespace.Name == tagNamespace && t.Name == tagName);

                if (tag == null)
                {
                    var tagNamespaceEntity = _dbContext.TagNamespaces
                        .FirstOrDefault(n => n.Name == tagNamespace);

                    if (tagNamespaceEntity == null)
                    {
                        tagNamespaceEntity = new EntityFileshardTagNamespace
                        {
                            Id = Guid.NewGuid(),
                            Name = tagNamespace
                        };

                        _dbContext.TagNamespaces.Add(tagNamespaceEntity);
                        await _dbContext.SaveChangesAsync();
                    }

                    tag = new EntityFileshardTag
                    {
                        Id = Guid.NewGuid(),
                        Name = tagName,
                        NamespaceId = _dbContext.TagNamespaces.First(n => n.Name == tagNamespace).Id
                    };

                    _dbContext.Tags.Add(tag);
                }

                var objectTag = new EntityFileshardObjectTag
                {
                    Id = Guid.NewGuid(),
                    ObjectId = objectId,
                    TagId = tag.Id,
                    Weight = weight
                };

                _dbContext.ObjectTags.Add(objectTag);

            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
