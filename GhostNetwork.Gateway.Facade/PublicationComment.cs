namespace GhostNetwork.Gateway.Facade
{
    public class PublicationComment
    {
        public PublicationComment(string id, string content)
        {
            Id = id;
            Content = content;
        }

        public string Id { get; }
        public string Content { get; }
    }
}
