
namespace Fileshard.Service.Entities
{
    public class EntityFileshardTag
    {
        public Guid Id { get; set; }

        public Guid NamespaceId { get; set; }

        public String Name { get; set; }

        public EntityFileshardTagNamespace Namespace { get; set; } = null!;
    }
}
