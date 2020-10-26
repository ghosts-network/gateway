namespace GhostNetwork.Gateway.Facade
{
    public class NewsFeedPublication
    {
        public NewsFeedPublication(string id, string content, CommentsShort comments, ReactionShort reactions)
        {
            Id = id;
            Content = content;
            Comments = comments;
            Reactions = reactions;
        }

        public string Id { get; }

        public string Content { get; }

        public CommentsShort Comments { get; }

        public ReactionShort Reactions { get; }
    }
}