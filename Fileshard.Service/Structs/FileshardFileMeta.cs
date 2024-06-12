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

        public FileshardFile FileshardFile { get; set; } = null!;

        public String? Key { get; set; }

        public DateTime? TimeValue { get; set; }

        public ulong? LongValue { get; set; }

        public String Value { get; set; }
    }
}
