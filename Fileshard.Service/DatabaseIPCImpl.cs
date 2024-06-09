using Fileshard.Shared.IPC;
using Fileshard.Shared.Structs;

namespace Fileshard.Service
{
    internal class DatabaseIPCImpl : DatabaseIPC
    {
        List<Database> DatabaseIPC.GetDatabases()
        {
            return new List<Database>
            {
            };
        }
    }
}
