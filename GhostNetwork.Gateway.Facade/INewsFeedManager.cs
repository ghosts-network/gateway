using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Facade
{
    public interface INewsFeedManager
    {
        Task<IEnumerable<NewsFeedPublication>> FindManyAsync();

        Task CreateAsync(string content, string author);

        Task UpdateAsync(string id, string content);

        Task DeleteAsync(string id);

        Task AddCommentAsync(string publicationId, string author, string content);

        Task<PublicationComment> GetCommentByIdAsync(string id);

        Task<(IEnumerable<PublicationComment>, long)> SearchAsync(string publicationId, int skip, int take);

        Task DeleteCommentAsync(string id);

        Task AddReactionAsync(string publicationId, string author, ReactionType reaction);

        Task RemoveReactionAsync(string publicationId, string author);
    }
}
