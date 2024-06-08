namespace Fileshard.Shared.Structs
{
    public class Database
    {
        public Database(Guid guid, string name)
        {
            Guid = guid;
            Name = name;
        }

        public Guid Guid { get; }
        public string Name { get; }
    }
}
