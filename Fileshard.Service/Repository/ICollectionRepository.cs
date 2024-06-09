using Fileshard.Service.Entities;

namespace Fileshard.Service.Repository
{
    public interface ICollectionRepository
    {
        Task<Guid> Create(string title);

        Task<List<FileshardCollection>> GetAll();

        Task Ingest(Guid collectionId, List<FileshardObject> fileshardObjects);

        Task<List<FileshardObject>> GetObjects(Guid collectionId);

        Task<bool> IsEmpty();
    }
}
