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
                new Database(Guid.NewGuid(), "Database1"),
                new Database(Guid.NewGuid(), "Database2"),
                new Database(Guid.NewGuid(), "Database3"),
                new Database(Guid.NewGuid(), "Database4"),
                new Database(Guid.NewGuid(), "Database5"),
            };
        }
    }
}
