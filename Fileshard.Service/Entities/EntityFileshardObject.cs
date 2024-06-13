using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fileshard.Service.Entities
{
    [Table("objects")]
    public class EntityFileshardObject
    {
        public Guid Id { get; set; }

        public Guid CollectionId { get; set; }

        public String Name { get; set; }

        public ICollection<EntityFileshardFile> Files { get; set; } = new List<EntityFileshardFile>();

        public EntityFileshardCollection Collection { get; set; } = null!;

        public ICollection<EntityFileshardObjectTag> Tags { get; set; } = new List<EntityFileshardObjectTag>();

        public Boolean IsImport { get; set; }

        [ConcurrencyCheck]
        public Guid Version { get; set; }
    }
}
