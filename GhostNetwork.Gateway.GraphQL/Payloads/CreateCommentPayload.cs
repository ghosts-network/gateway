using GhostNetwork.Gateway.GraphQL.Models;

namespace GhostNetwork.Gateway.GraphQL.Payloads
{
    public class CreateCommentPayload
    {
        public int Code { get; set; }
        public CommentEntity? Comment { get; set; }
    }
}
