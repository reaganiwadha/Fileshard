using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fileshard.Service.Chores
{
    internal interface IFileshardChore<T> 
    {
        Task<ProgressIterator<T>> GetTask();
    }
}
