namespace GhostNetwork.Gateway.GraphQL.Models
{
    public class PublicationEntity
    {
        public string Id { get; set; } = null!;

        public string Content { get; set; } = string.Empty;

        [GraphQLIgnore]
        public string AuthorId { get; set; } = string.Empty;

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset? UpdatedOn { get; set; }
    }
}
