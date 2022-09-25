using GhostNetwork.Gateway.GraphQL.Models;

namespace GhostNetwork.Gateway.GraphQL.Payloads
{
    public class CreatePublicationPayload
    {
        public PublicationEntity? Publication { get; set; }
    }
}