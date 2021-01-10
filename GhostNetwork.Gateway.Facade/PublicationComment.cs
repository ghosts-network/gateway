using System;

namespace GhostNetwork.Gateway.Facade
{
    public class PublicationComment
    {
        public PublicationComment(string id, string content, string publicationId,
            UserInfo author, DateTimeOffset createdOn)
        {
            Id = id;
            Content = content;
            PublicationId = publicationId;
            Author = author;
            CreatedOn = createdOn;
        }

        public string Id { get; }
        public string Content { get; }

        public string PublicationId { get; }

        [Obsolete]
        public Guid AuthorId => Author.Id;
        
        public UserInfo Author { get; }

        public DateTimeOffset CreatedOn { get; }
    }
}
