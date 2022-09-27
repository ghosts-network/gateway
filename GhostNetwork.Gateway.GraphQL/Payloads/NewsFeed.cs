using GhostNetwork.Gateway.GraphQL.Models;

namespace GhostNetwork.Gateway.GraphQL.Payloads
{
    public class NewsFeedPayload
    {
        public IEnumerable<PublicationEntity> Publications { get; set; } = Enumerable.Empty<PublicationEntity>();

        public string? Cursor { get; set; }
    }
}
