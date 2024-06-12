using Fileshard.Service.Entities;
using Fileshard.Service.Structs;

namespace Fileshard.Service.Repository
{
    public interface ICollectionRepository
    {
        Task<Guid> Create(string title);

        Task<List<FileshardCollection>> GetAll();

        Task Ingest(Guid collectionId, List<FileshardObject> fileshardObjects);

        Task<Boolean> FileHasMeta(String key, Guid fileId);

        Task<Boolean> FileHasMetas(Guid fileId, params string[] keys);

        Task UpsertMeta(String key, String value, Guid fileId);

        Task UpsertMeta(String key, DateTime value, Guid fileId);

        Task UpsertMeta(String key, long value, Guid fileId);

        Task<List<FileshardObject>> GetObjects(Guid collectionId);

        Task<FileshardObject?> GetObject(Guid collectionId, Guid objectId);

        Task<List<String>> FilterNonExistentFiles(List<String> files);

        Task<bool> IsEmpty();
    }
}
