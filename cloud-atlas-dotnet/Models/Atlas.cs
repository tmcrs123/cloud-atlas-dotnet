namespace cloud_atlas_dotnet.Models
{
    public class Atlas
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }

        public int MarkersCount { get; set; }

        public Guid Owner { get; set; }
    }
}
