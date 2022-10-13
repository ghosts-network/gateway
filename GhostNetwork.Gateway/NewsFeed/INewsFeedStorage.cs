using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.NewsFeed
{
    public interface INewsFeedStorage
    {
        INewsFeedReactionsStorage Reactions { get; }

        INewsFeedCommentsStorage Comments { get; }

        INewsFeedMediaStorage Media { get; }

        Task<NewsFeedPublication> GetByIdAsync(string id);

        Task<(IEnumerable<NewsFeedPublication>, string)> GetUserFeedAsync(string userId, int take, string cursor);

        Task<(IEnumerable<NewsFeedPublication>, string)> GetUserPublicationsAsync(Guid userId, int take, string cursor);

        Task<NewsFeedPublication> PublishAsync(string content, UserInfo author, IEnumerable<MediaStream> mediaStreams);

        Task UpdateAsync(string publicationId, string content);

        Task DeleteAsync(string publicationId);
    }
}