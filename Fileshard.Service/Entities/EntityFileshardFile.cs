using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fileshard.Service.Entities
{
    [Table("files")]
    public class EntityFileshardFile
    {
        public Guid Id { get; set; }

        public Guid ObjectId { get; set; }

        public EntityFileshardObject FileshardObject { get; set; } = null!;

        public String InternalPath { get; set; }

        public ICollection<EntityFileshardFileMeta> Metas { get; set; } = new List<EntityFileshardFileMeta>();


        [ConcurrencyCheck]
        public Guid Version { get; set; }
    }
}
