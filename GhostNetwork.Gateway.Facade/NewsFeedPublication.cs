namespace GhostNetwork.Gateway.Facade
{
    public class NewsFeedPublication
    {
        public NewsFeedPublication(string id, string content, int commentsCount, ReactionShort reactions)
        {
            Id = id;
            Content = content;
            CommentsCount = commentsCount;
            Reactions = reactions;
        }

        public string Id { get; }

        public string Content { get; }

        public int CommentsCount { get; }

        public ReactionShort Reactions { get; }
    }
}