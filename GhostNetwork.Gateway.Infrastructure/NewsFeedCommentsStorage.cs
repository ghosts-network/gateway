using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
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

        public async Task<(IEnumerable<PublicationComment>, long, string)> GetAsync(string publicationId, int skip, int take, string cursor)
        {
            var response = await commentsApi
                .SearchByKeyWithHttpInfoAsync(KeysBuilder.PublicationCommentKey(publicationId), skip, cursor, take, Ordering.Desc);

            return (response.Data.Select(c => ToDomain(c, publicationId)).ToList(),
                GetTotalCountHeader(response),
                GetCursorHeader(response));
        }

        public async Task<PublicationComment> PublishAsync(string content, string publicationId, UserInfo author)
        {
            var authorContent = new UserInfoModel(author.Id, author.FullName, author.AvatarUrl);
            var comment = await commentsApi.CreateAsync(new CreateCommentModel(KeysBuilder.PublicationCommentKey(publicationId), content, author: authorContent));

            return ToDomain(comment, publicationId);
        }

        public async Task<DomainResult> UpdateAsync(string commentId, string content)
        {
            try
            {
                await commentsApi.UpdateAsync(commentId, new UpdateCommentModel(content));
            }
            catch (ApiException)
            {
                return DomainResult.Error("API error!");
            }

            return DomainResult.Success();
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

        private static string GetCursorHeader(IApiResponse response)
        {
            return !response.Headers.TryGetValue("X-Cursor", out var headers)
                ? default
                : headers.FirstOrDefault();
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