namespace GhostNetwork.Gateway.GraphQL.Models.InputModels
{
    public class UpdatePublicationInput
    {
        public string PublicationId { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
    }
}
