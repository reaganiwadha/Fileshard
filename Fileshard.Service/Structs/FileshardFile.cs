using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fileshard.Service.Structs
{
    public class FileshardFile
    {
        public Guid Id { get; set; }

        public Guid ObjectId { get; set; }

        public FileshardObject FileshardObject { get; set; } = null!;

        public String InternalPath { get; set; }

        public ICollection<FileshardFileMeta> Metas { get; set; } = new List<FileshardFileMeta>();
    }
}
