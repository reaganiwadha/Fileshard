using Fileshard.Shared.Structs;

namespace Fileshard.Shared.IPC
{
    public interface DatabaseIPC
    {
        List<Database> GetDatabases();
    }
}
