using Fileshard.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fileshard.Service.Structs
{
    public class FileshardTag
    {
        public Guid Id { get; set; }

        public Guid NamespaceId { get; set; }

        public String Name { get; set; }
    }
}
