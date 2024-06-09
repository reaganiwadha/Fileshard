using System.ComponentModel.DataAnnotations.Schema;

namespace Fileshard.Service.Entities
{
    [Table("files")]
    public class FileshardFile
    {
        public Guid Id { get; set; }

        public Guid ObjectId { get; set; }

        public FileshardObject FileshardObject { get; set; } = null!;

        public String InternalPath { get; set; }
    }
}
