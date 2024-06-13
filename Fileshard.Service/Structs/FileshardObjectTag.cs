using Fileshard.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fileshard.Service.Structs
{
    public class FileshardObjectTag
    {
        public Guid Id { get; set; }

        public Guid ObjectId { get; set; }

        public Guid TagId { get; set; }

        public FileshardTag Tag { get; set; } = null!;

        public float? Weight { get; set; }
    }
}
