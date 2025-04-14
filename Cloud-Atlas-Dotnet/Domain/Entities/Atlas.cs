namespace Cloud_Atlas_Dotnet.Domain.Entities
{
    public class Atlas
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Title { get; set; }
    }

    public class CreateAtlasRequest
    {
        public string Title { get; set; }
        public Guid UserId { get; set; }
    }

    public class UpdateAtlasRequest
    {
        public string Title { get; set; }
        public Guid AtlasId { get; set; }
    }

    public class DeleteAtlasRequest
    {
        public Guid AtlasId { get; set; }
    }
}
