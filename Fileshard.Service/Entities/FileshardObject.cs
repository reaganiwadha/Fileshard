using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fileshard.Service.Entities
{
    [Table("objects")]
    public class FileshardObject
    {
        public Guid Id { get; set; }

        public Guid CollectionId { get; set; }

        public String Name { get; set; }

        public ICollection<FileshardFile> Files { get; set; } = new List<FileshardFile>();

        public Boolean IsImport { get; set; }

        [ConcurrencyCheck]
        public Guid Version { get; set; }
    }
}
