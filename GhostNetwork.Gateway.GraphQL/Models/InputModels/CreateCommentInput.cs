namespace GhostNetwork.Gateway.GraphQL.Models.InputModels
{
    public class CreateCommentInput
    {
        public string PublicationId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? CommentReplyId { get; set; }

    }
}
