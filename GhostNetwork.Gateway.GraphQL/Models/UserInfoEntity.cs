namespace GhostNetwork.Gateway.GraphQL.Models
{
    public class UserInfoEntity
    {
        public string Id { get; set; } = null!;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }
    }
}
