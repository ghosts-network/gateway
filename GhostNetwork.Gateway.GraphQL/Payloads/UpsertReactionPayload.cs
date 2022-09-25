using GhostNetwork.Gateway.NewsFeed;

namespace GhostNetwork.Gateway.GraphQL.Payloads
{
    public class UpsertReactionPayload
    {
        public int Code { get; set; }
        public ReactionType? ReactionType { get; set; }
    }
}
