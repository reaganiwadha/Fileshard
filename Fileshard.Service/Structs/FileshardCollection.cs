namespace Fileshard.Service.Structs
{
    public class FileshardCollection
    {
        public Guid Id { get; set; }

        public String Title { get; set; }

        public ICollection<FileshardObject> Objects { get; set; } = new List<FileshardObject>();
    }
}
