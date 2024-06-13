using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fileshard.Service.Structs
{
    public class FileshardTagNamespace
    {
        public Guid Id { get; set; }

        public String Name { get; set; }

        public ICollection<FileshardTag> Tags { get; set; } = new List<FileshardTag>();
    }
}
