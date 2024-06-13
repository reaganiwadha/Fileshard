using System.ComponentModel.DataAnnotations.Schema;

namespace Fileshard.Service.Entities
{
    [Table("object_tags")]
    public class EntityFileshardObjectTag
    {
        public Guid Id { get; set; }

        public Guid ObjectId { get; set; }

        public EntityFileshardObject FileshardObject { get; set; } = null!;

        public Guid TagId { get; set; }

        public EntityFileshardTag Tag { get; set; } = null!;

        public float? Weight { get; set; }
    }
}
