using System.ComponentModel.DataAnnotations.Schema;


namespace Fileshard.Service.Entities
{
    [Table("tag_namespaces")]
    public class EntityFileshardTagNamespace
    {
        public Guid Id { get; set; }

        public String Name { get; set; }

        public ICollection<EntityFileshardTag> Tags { get; set; } = new List<EntityFileshardTag>();
    }
}
