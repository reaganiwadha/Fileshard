namespace Fileshard.Service.Repository
{
    public interface ICollectionRepository
    {
        Task<bool> IsEmpty();
    }
}
