namespace Cloud_Atlas_Dotnet.Domain.Entities
{
    public class Atlas
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Title { get; set; }
        public Coordinates Coordinates { get; set; }
    }
}
