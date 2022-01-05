using System;

namespace GhostNetwork.Gateway.NewsFeed
{
    public class NewsFeedPublication
    {
        public NewsFeedPublication(string id, string content, DateTimeOffset createdeOn, DateTimeOffset? updatedOn, CommentsShort comments, ReactionShort reactions, UserInfo author)
        {
            Id = id;
            Content = content;
            Comments = comments;
            Reactions = reactions;
            Author = author;
            CreatedOn = createdeOn;
            UpdatedOn = updatedOn;
        }

        public string Id { get; }

        public string Content { get; }

        public CommentsShort Comments { get; }

        public ReactionShort Reactions { get; }

        public UserInfo Author { get; }

        public DateTimeOffset CreatedOn { get; }

        public DateTimeOffset? UpdatedOn { get;  }
    }
}