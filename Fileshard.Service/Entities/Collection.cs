using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fileshard.Service.Entities
{
    [Table("collections")]
    public class Collection
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }
    }
}
