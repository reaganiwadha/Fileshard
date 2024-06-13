namespace Fileshard.Service.Structs
{
    public class FileshardObject
    {
        public Guid Id { get; set; }

        public Guid CollectionId { get; set; }

        public String Name { get; set; }

        public ICollection<FileshardFile> Files { get; set; } = new List<FileshardFile>();

        public ICollection<FileshardObjectTag> Tags { get; set; } = new List<FileshardObjectTag>();
    }
}
