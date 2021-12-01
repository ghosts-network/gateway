using System.Threading.Tasks;

namespace GhostNetwork.Gateway.NewsFeed
{
    public interface INewsFeedReactionsStorage
    {
        Task<ReactionShort> AddOrUpdateAsync(string publicationId, ReactionType reactionType, string userId);

        Task<ReactionShort> RemoveAsync(string publicationId, string userId);

        Task RemoveManyAsync(string key);
    }
}