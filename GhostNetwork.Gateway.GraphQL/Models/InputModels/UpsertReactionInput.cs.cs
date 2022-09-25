using GhostNetwork.Gateway.NewsFeed;

namespace GhostNetwork.Gateway.GraphQL.Models.InputModels
{
    public class UpsertReactionInput
    {
        public string PublicationId { get; set; } = string.Empty;
        public ReactionType ReactionType { get; set; }
    }
}
