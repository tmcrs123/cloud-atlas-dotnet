namespace Cloud_Atlas_Dotnet.Domain.Entities
{
    public class Account
    {
        public Guid AccountId { get; set; }
        public Guid UserId { get; set; }
        public bool IsVerified { get; set; }
    }

    public class VerifyAccountRequest
    {
        public bool IsVerified { get; set; }
        public Guid UserId { get; set; }
    }
}
