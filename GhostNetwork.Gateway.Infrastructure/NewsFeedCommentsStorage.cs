using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Content.Api;
using GhostNetwork.Content.Client;
using GhostNetwork.Content.Model;
using GhostNetwork.Gateway.NewsFeed;

namespace GhostNetwork.Gateway.Infrastructure
{
    public class NewsFeedCommentsStorage : INewsFeedCommentsStorage
    {
        private readonly ICommentsApi commentsApi;

        public NewsFeedCommentsStorage(ICommentsApi commentsApi)
        {
            this.commentsApi = commentsApi;
        }

        public async Task<PublicationComment> GetByIdAsync(string id)
        {
            var comment = await commentsApi.GetByIdAsync(id);

            return ToDomain(comment, comment == null ? null : KeysBuilder.PublicationFromCommentKey(comment.Key));
        }

        public async Task<(IEnumerable<PublicationComment>, long)> GetAsync(string publicationId, int skip, int take)
        {
            var response = await commentsApi
                .SearchByKeyWithHttpInfoAsync(KeysBuilder.PublicationCommentKey(publicationId), skip, take);

            var comments = response.Data.Select(c => ToDomain(c, publicationId)).ToList();
            var totalCount = GetTotalCountHeader(response);

            return (comments, totalCount);
        }

        public async Task<PublicationComment> PublishAsync(string content, string publicationId, string userId)
        {
            var comment = await commentsApi.CreateAsync(new CreateCommentModel(KeysBuilder.PublicationCommentKey(publicationId), content, authorId: userId));

            return ToDomain(comment, publicationId);
        }

        public async Task DeleteAsync(string id)
        {
            await commentsApi.DeleteAsync(id);
        }

        public async Task DeleteManyAsync(string publicationId)
        {
            await commentsApi.DeleteByKeyAsync(KeysBuilder.PublicationCommentKey(publicationId));
        }

        private static long GetTotalCountHeader(IApiResponse response)
        {
            if (!response.Headers.TryGetValue("X-TotalCount", out var headers))
            {
                return 0;
            }

            return int.TryParse(headers.FirstOrDefault(), out var totalCount)
                ? totalCount
                : 0;
        }

        private static PublicationComment ToDomain(Comment entity, string publicationId)
        {
            return entity == null
                ? null
                : new PublicationComment(
                    entity.Id,
                    entity.Content,
                    publicationId,
                    ToUser(entity.Author),
                    entity.CreatedOn);
        }

        private static UserInfo ToUser(Content.Model.UserInfo userInfo)
        {
            return new(userInfo.Id, userInfo.FullName, userInfo.AvatarUrl);
        }
    }
}