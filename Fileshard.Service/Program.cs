using ServiceWire.NamedPipes;
using ServiceWire;
using Fileshard.Shared.IPC;

namespace Fileshard.Service
{
    class Service
    {
        static void Main(string[] args)
        {
            var logger = new Logger(logLevel: LogLevel.Debug);

            var nphost = new NpHost("fileshard", logger);
            nphost.AddService<DatabaseIPC>(new DatabaseIPCImpl());

            nphost.Open();

            Console.ReadLine();
        }
    }
}