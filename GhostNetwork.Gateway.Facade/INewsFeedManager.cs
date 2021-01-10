using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Facade
{
    public interface INewsFeedManager
    {
        Task<(IEnumerable<NewsFeedPublication>, long)> FindManyAsync(int skip, int take);

        Task<NewsFeedPublication> CreateAsync(string content);

        Task UpdateAsync(string id, string content);

        Task<bool> DeleteAsync(string id);

        Task AddCommentAsync(string publicationId, string content);

        Task<PublicationComment> GetCommentByIdAsync(string id);

        Task<(IEnumerable<PublicationComment>, long)> SearchCommentsAsync(string publicationId, int skip, int take);

        Task<ReactionShort> GetReactionsAsync(string publicationId);

        Task AddReactionAsync(string publicationId, ReactionType type);

        Task DeleteCommentAsync(string id);

        Task RemoveReactionAsync(string publicationId);
    }
}
