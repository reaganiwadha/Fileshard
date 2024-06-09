using Fileshard.Service.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fileshard.Service.Database
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly FileshardDbContext _dbContext = new FileshardDbContext();

        public CollectionRepository()
        {
        }

        public Task<bool> IsEmpty()
        {
            return Task.FromResult(!_dbContext.Collections.Any());
        }
    }
}
