namespace GhostNetwork.Gateway.NewsFeed
{
    public class UserReaction
    {
        public UserReaction(ReactionType type)
        {
            Type = type;
        }

        public ReactionType Type { get; }
    }
}
