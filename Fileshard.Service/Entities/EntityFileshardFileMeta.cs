using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fileshard.Service.Entities
{
    [Table("file_metas")]
    internal class EntityFileshardFileMeta
    {
        public Guid Id { get; set; }

        public Guid FileId { get; set; }

        public EntityFileshardFile FileshardFile { get; set; } = null!;

        public String Key { get; set; }

        public String Value { get; set; }


        [ConcurrencyCheck]
        public Guid Version { get; set; }
    }
}
