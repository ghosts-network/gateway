using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.NewsFeed
{
    public interface INewsFeedStorage
    {
        INewsFeedReactionsStorage Reactions { get; }
        INewsFeedCommentsStorage Comments { get; }
        
        Task<NewsFeedPublication> GetByIdAsync(string id);
        Task<(IEnumerable<NewsFeedPublication>, long)> GetUserFeedAsync(string userId, int skip, int take);
        Task<NewsFeedPublication> PublishAsync(string content, string userId);
        Task UpdateAsync(string publicationId, string content);
        Task DeleteAsync(string publicationId);
    }
}