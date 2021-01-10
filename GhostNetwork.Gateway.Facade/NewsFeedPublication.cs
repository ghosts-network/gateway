namespace GhostNetwork.Gateway.Facade
{
    public class NewsFeedPublication
    {
        public NewsFeedPublication(string id, string content, CommentsShort comments, ReactionShort reactions, UserInfo author)
        {
            Id = id;
            Content = content;
            Comments = comments;
            Reactions = reactions;
            Author = author;
        }

        public string Id { get; }

        public string Content { get; }

        public CommentsShort Comments { get; }

        public ReactionShort Reactions { get; }
        
        public UserInfo Author { get; }
    }
}