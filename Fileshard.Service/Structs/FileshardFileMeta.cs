using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fileshard.Service.Structs
{
    public class FileshardFileMeta
    {
        public Guid Id { get; set; }

        public Guid FileId { get; set; }

        public String? Key { get; set; }

        public DateTime? TimeValue { get; set; }

        public long? LongValue { get; set; }

        public String Value { get; set; }
    }
}
