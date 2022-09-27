namespace GhostNetwork.Gateway.GraphQL.Models
{
    public class CommentEntity
    {
        public string Id { get; set; } = null!;

        public string Content { get; set; } = string.Empty;

        [GraphQLIgnore]
        public string Key { get; set; } = null!;

        [GraphQLIgnore]
        public string AuthorId { get; set; } = null!;

        public string? CommentReplyId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}
