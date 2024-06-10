using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fileshard.Service.Entities
{
    [Table("collections")]
    internal class EntityFileshardCollection
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        public ICollection<EntityFileshardObject> Objects { get; set; } = new List<EntityFileshardObject>();
    }
}
