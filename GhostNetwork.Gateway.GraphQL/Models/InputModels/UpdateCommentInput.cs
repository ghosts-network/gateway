namespace GhostNetwork.Gateway.GraphQL.Models.InputModels
{
    public class UpdateCommentInput
    {
        public string CommentId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
