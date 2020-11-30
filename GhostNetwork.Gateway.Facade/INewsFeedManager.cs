using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Facade
{
    public interface INewsFeedManager
    {
        Task<IEnumerable<NewsFeedPublication>> FindManyAsync();

        Task<NewsFeedPublication> CreateAsync(string content, string author);

        Task UpdateAsync(string id, string content);

        Task DeleteAsync(string id);

        Task AddCommentAsync(string publicationId, string author, string content);

        Task<PublicationComment> GetCommentByIdAsync(string id);

        Task<(IEnumerable<PublicationComment>, long)> SearchCommentsAsync(string publicationId, int skip, int take);

        Task<IEnumerable<ReactionShort>> GetReactionsAsync(string publicationId);

        Task AddReactionAsync(string publicationId, string author, ReactionType type);

        Task DeleteCommentAsync(string id);

        Task RemoveReactionAsync(string publicationId, string author);
    }
}
