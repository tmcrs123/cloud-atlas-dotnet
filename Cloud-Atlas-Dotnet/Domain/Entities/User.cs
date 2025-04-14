namespace Cloud_Atlas_Dotnet.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }

    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class DeleteUserRequest
    {
        public Guid Id { get; set; }
    }

    public class UpdateUserRequest
    {
        public string Password { get; set; }
        public Guid Id { get; set; }
    }

}
