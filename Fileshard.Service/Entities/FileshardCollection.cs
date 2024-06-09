using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fileshard.Service.Entities
{
    [Table("collections")]
    public class FileshardCollection
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        public ICollection<FileshardObject> Objects { get; set; } = new List<FileshardObject>();
    }
}
