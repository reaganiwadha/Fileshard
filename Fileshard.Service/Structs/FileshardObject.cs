using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fileshard.Service.Structs
{
    public class FileshardObject
    {
        public Guid Id { get; set; }

        public Guid CollectionId { get; set; }

        public FileshardCollection FileshardCollection { get; set; } = null!;

        public String Name { get; set; }

        public ICollection<FileshardFile> Files { get; set; } = new List<FileshardFile>();
    }
}
