using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Api;
using GhostNetwork.Gateway.Facade;
using GhostNetwork.Publications.Api;
using GhostNetwork.Publications.Model;
using GhostNetwork.Reactions.Api;
using GhostNetwork.Reactions.Client;
using UserInfo = GhostNetwork.Gateway.Facade.UserInfo;

namespace GhostNetwork.Infrastructure.Repository
{
    public class NewsFeedStorage : INewsFeedManager
    {
        private readonly IPublicationsApi publicationsApi;
        private readonly ICommentsApi commentsApi;
        private readonly IReactionsApi reactionsApi;
        private readonly ICurrentUserProvider currentUserProvider;

        public NewsFeedStorage(IPublicationsApi publicationsApi, ICommentsApi commentsApi, IReactionsApi reactionsApi, ICurrentUserProvider currentUserProvider)
        {
            this.publicationsApi = publicationsApi;
            this.commentsApi = commentsApi;
            this.reactionsApi = reactionsApi;
            this.currentUserProvider = currentUserProvider;
        }

        public async Task<(IEnumerable<NewsFeedPublication>, long)> FindManyAsync(int skip, int take)
        {
            var publicationsResponse = await publicationsApi.SearchWithHttpInfoAsync(skip, take, order: Ordering.Desc);
            var publications = publicationsResponse.Data;
            var newsFeedPublications = new List<NewsFeedPublication>(publications.Count);

            foreach (var publication in publications)
            {
                var commentsResponse = await commentsApi.SearchWithHttpInfoAsync(publication.Id, 0, 3);

                var reactions = new Dictionary<ReactionType, int>();
                try
                {
                    var response = await reactionsApi.GetAsync($"publication_{publication.Id}");
                    reactions = response.Keys
                        .Select(k => (Enum.Parse<ReactionType>(k), response[k]))
                        .ToDictionary(o => o.Item1, o => o.Item2);
                }
                catch (ApiException)
                {
                    // ignored
                }

                UserReaction userReaction = null;

                if (currentUserProvider.UserId != null)
                {
                    try
                    {
                        var reactionByAuthor = await reactionsApi.GetReactionByAuthorAsync($"publication_{publication.Id}", currentUserProvider.UserId);

                        userReaction = new UserReaction(Enum.Parse<ReactionType>(reactionByAuthor.Type));
                    }
                    catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
                    {
                        // ignored
                    }
                }

                newsFeedPublications.Add(new NewsFeedPublication(
                    publication.Id,
                    publication.Content,
                    BuildCommentsShort(commentsResponse),
                    new ReactionShort(reactions, userReaction),
                    ToUser(publication.Author)));
            }

            return (newsFeedPublications, GetTotalCountHeader(publicationsResponse));
        }

        public async Task<NewsFeedPublication> CreateAsync(string content)
        {
            var model = new CreatePublicationModel(content, ToUserModel(await currentUserProvider.GetProfileAsync()));
            var entity = await publicationsApi.CreateAsync(model);

            return new NewsFeedPublication(entity.Id,
                entity.Content,
                new CommentsShort(Enumerable.Empty<PublicationComment>(), 0), 
                new ReactionShort(new Dictionary<ReactionType, int>(),
                    new UserReaction(new ReactionType())),
                ToUser(entity.Author));
        }

        public async Task<ReactionShort> GetReactionsAsync(string publicationId)
        {
            var reactions = new Dictionary<ReactionType, int>();
            try
            {
                var response = await reactionsApi.GetAsync($"publication_{publicationId}");
                reactions = response.Keys
                    .Select(k => (Enum.Parse<ReactionType>(k), response[k]))
                    .ToDictionary(o => o.Item1, o => o.Item2);
            }
            catch (ApiException)
            {
                // ignored
            }

            UserReaction userReaction = null;

            if (currentUserProvider.UserId != null)
            {
                try
                {
                    var reactionByAuthor = await reactionsApi.GetReactionByAuthorAsync($"publication_{publicationId}", currentUserProvider.UserId);

                    userReaction = new UserReaction(Enum.Parse<ReactionType>(reactionByAuthor.Type));
                }
                catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
                {
                    // ignored
                }
            }

            return new ReactionShort(reactions, userReaction);
        }

        public async Task AddReactionAsync(string publicationId, ReactionType reaction)
        {
            await reactionsApi.UpsertAsync($"publication_{publicationId}", reaction.ToString(), currentUserProvider.UserId);
        }

        public async Task RemoveReactionAsync(string publicationId)
        {
            await reactionsApi.DeleteByAuthorAsync($"publication_{publicationId}", currentUserProvider.UserId);
        }

        public async Task UpdateAsync(string id, string content)
        {
            var model = new UpdatePublicationModel(content);

            await publicationsApi.UpdateAsync(id, model);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var publication = await publicationsApi.GetByIdAsync(id);

            if (publication.Author.Id.ToString() == currentUserProvider.UserId)
            {
                await reactionsApi.DeleteAsync(id);
                await publicationsApi.DeleteAsync(id);
                return true;
            }

            return false;
        }

        public async Task AddCommentAsync(string publicationId, string content)
        {
            await commentsApi.CreateAsync(new CreateCommentModel(publicationId, content, author: ToUserModel(await currentUserProvider.GetProfileAsync())));
        }

        public async Task<(IEnumerable<PublicationComment>, long)> SearchCommentsAsync(string publicationId, int skip, int take)
        {
            var comments = await commentsApi.SearchAsync(publicationId, skip, take);

            var totalCount = 0;

            return (comments.Select(ToDomain).ToList(), totalCount);
        }

        public async Task<PublicationComment> GetCommentByIdAsync(string id)
        {
            var comment = await commentsApi.GetByIdAsync(id);

            return comment == null ? null : ToDomain(comment);
        }

        public async Task DeleteCommentAsync(string id)
        {
            await commentsApi.DeleteAsync(id);
        }

        private static long GetTotalCountHeader<T>(Publications.Client.ApiResponse<T> response)
        {
            var totalCount = 0;
            if (response.Headers.TryGetValue("X-TotalCount", out var headers))
            {
                if (!int.TryParse(headers.FirstOrDefault(), out totalCount))
                {
                    totalCount = 0;
                }
            }

            return totalCount;
        }

        private static PublicationComment ToDomain(Comment entity)
        {
            return new PublicationComment(
                entity.Id,
                entity.Content,
                entity.PublicationId,
                ToUser(entity.Author),
                entity.CreatedOn
                );
        }

        private static CommentsShort BuildCommentsShort(Publications.Client.ApiResponse<List<Comment>> response)
        {
            return new CommentsShort(
                response.Data
                    .Select(c => new PublicationComment(c.Id, c.Content, c.PublicationId, ToUser(c.Author), c.CreatedOn))
                    .ToList(),
                GetTotalCountHeader(response));
        }

        private static UserInfo ToUser(Publications.Model.UserInfo userInfo)
        {
            return new UserInfo(userInfo.Id, userInfo.FullName, userInfo.AvatarUrl);
        }

        private static UserInfoModel ToUserModel(UserInfo userInfo)
        {
            return new UserInfoModel(userInfo.Id, userInfo.FullName, userInfo.AvatarUrl);
        }
    }
}
