using System;
using System.Collections.Generic;

namespace GhostNetwork.Gateway.NewsFeed
{
    public class NewsFeedPublication
    {
        public NewsFeedPublication(
            string id,
            string content,
            DateTimeOffset createdOn,
            DateTimeOffset updatedOn,
            CommentsShort comments,
            ReactionShort reactions,
            IEnumerable<Media> media,
            UserInfo author)
        {
            Id = id;
            Content = content;
            Comments = comments;
            Reactions = reactions;
            Media = media;
            Author = author;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
        }

        public string Id { get; }

        public string Content { get; }

        public CommentsShort Comments { get; }

        public ReactionShort Reactions { get; }

        public IEnumerable<Media> Media { get; }

        public UserInfo Author { get; }

        public DateTimeOffset CreatedOn { get; }

        public DateTimeOffset UpdatedOn { get;  }
    }
}