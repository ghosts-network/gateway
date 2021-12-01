using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace GhostNetwork.Gateway.NewsFeed
{
    public interface INewsFeedCommentsStorage
    {
        Task<PublicationComment> GetByIdAsync(string id);

        Task<(IEnumerable<PublicationComment>, long)> GetAsync(string publicationId, int skip, int take);

        Task<PublicationComment> PublishAsync(string content, string publicationId, string userId);

        Task<DomainResult> UpdateAsync(string commentId, string content);

        Task DeleteAsync(string id);

        Task DeleteManyAsync(string publicationId);
    }
}