using Fileshard.Service.Entities;

namespace Fileshard.Service.Repository
{
    public interface ICollectionRepository
    {
        Task<Guid> Create(string title);

        Task<List<FileshardCollection>> GetAll();

        Task Ingest(Guid collectionId, List<FileshardObject> fileshardObjects);

        Task UpdateFile(FileshardFile file);

        Task InsertMeta(FileshardFileMeta meta);

        Task<List<FileshardObject>> GetObjects(Guid collectionId);

        Task<FileshardObject?> GetObject(Guid collectionId, Guid objectId);

        Task<List<String>> FilterNonExistentFiles(List<String> files);

        Task<bool> IsEmpty();
    }
}
