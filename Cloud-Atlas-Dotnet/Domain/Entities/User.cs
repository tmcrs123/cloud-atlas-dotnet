using Cloud_Atlas_Dotnet.Domain.Attributes;

namespace Cloud_Atlas_Dotnet.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        
        [SensitiveData]
        public string Name { get; set; }
        
        public string Username { get; set; }
        
        public string Email { get; set; }
        
        [SensitiveData]
        public string Password { get; set; }
    }
}
