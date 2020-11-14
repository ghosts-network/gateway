using System;

namespace GhostNetwork.Gateway.Facade
{
    public class PublicationComment
    {
        public PublicationComment(string id, string content, string publicationId,
            string authorId, DateTimeOffset createdOn)
        {
            Id = id;
            Content = content;
            PublicationId = publicationId;
            AuthorId = authorId;
            CreatedOn = createdOn;
        }

        public string Id { get; }
        public string Content { get; }

        public string PublicationId { get; }

        public string AuthorId { get; }

        public DateTimeOffset CreatedOn { get; }
    }
}
