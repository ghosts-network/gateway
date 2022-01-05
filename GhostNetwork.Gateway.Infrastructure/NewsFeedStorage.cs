using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Content.Api;
using GhostNetwork.Content.Client;
using GhostNetwork.Content.Model;
using GhostNetwork.Gateway.NewsFeed;

namespace GhostNetwork.Gateway.Infrastructure
{
    public class NewsFeedStorage : INewsFeedStorage
    {
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IPublicationsApi publicationsApi;
        private readonly ICommentsApi commentsApi;
        private readonly IReactionsApi reactionsApi;

        public NewsFeedStorage(IPublicationsApi publicationsApi, ICommentsApi commentsApi, IReactionsApi reactionsApi, ICurrentUserProvider currentUserProvider)
        {
            this.publicationsApi = publicationsApi;
            this.commentsApi = commentsApi;
            this.reactionsApi = reactionsApi;
            this.currentUserProvider = currentUserProvider;

            Reactions = new NewsFeedReactionsStorage(reactionsApi);
            Comments = new NewsFeedCommentsStorage(commentsApi);
        }

        public INewsFeedReactionsStorage Reactions { get; }

        public INewsFeedCommentsStorage Comments { get; }

        public async Task<NewsFeedPublication> GetByIdAsync(string id)
        {
            try
            {
                var publication = await publicationsApi.GetByIdAsync(id);
                var featuredComments = await LoadCommentsAsync(new[] { id });
                var reactions = await LoadReactionsAsync(new[] { id });
                var userReactions = publication.Author?.Id == null
                    ? new Dictionary<string, UserReaction>()
                    : await LoadUserReactionsAsync(publication.Author?.Id.ToString(), new[] { id });

                return new NewsFeedPublication(
                    publication.Id,
                    publication.Content,
                    publication.CreatedOn,
                    publication.UpdatedOn,
                    featuredComments[publication.Id],
                    new ReactionShort(reactions[publication.Id], userReactions[publication.Id]),
                    ToUser(publication.Author));
            }
            catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<(IEnumerable<NewsFeedPublication>, long)> GetUserFeedAsync(string userId, int skip, int take)
        {
            var publicationsResponse = await publicationsApi.SearchWithHttpInfoAsync(skip, take, order: Ordering.Desc);
            var publications = publicationsResponse.Data;
            var totalCount = GetTotalCountHeader(publicationsResponse);

            var featuredComments = await LoadCommentsAsync(publications.Select(p => p.Id));
            var reactions = await LoadReactionsAsync(publications.Select(p => p.Id));
            var userReactions = userId == null
                ? new Dictionary<string, UserReaction>()
                : await LoadUserReactionsAsync(userId, publications.Select(p => p.Id));

            var news = publications
                .Select(publication => new NewsFeedPublication(
                    publication.Id,
                    publication.Content,
                    publication.CreatedOn,
                    publication.UpdatedOn,
                    featuredComments[publication.Id],
                    new ReactionShort(reactions[publication.Id], userReactions[publication.Id]),
                    ToUser(publication.Author)))
                .ToList();

            return (news, totalCount);
        }

        public async Task<(IEnumerable<NewsFeedPublication>, long)> GetUserPublicationsAsync(Guid userId, int skip, int take)
        {
            var publicationsResponse = await publicationsApi.SearchByAuthorWithHttpInfoAsync(userId, skip, take, order: Ordering.Desc);
            var publications = publicationsResponse.Data;
            var totalCount = GetTotalCountHeader(publicationsResponse);

            var featuredComments = await LoadCommentsAsync(publications.Select(p => p.Id));
            var reactions = await LoadReactionsAsync(publications.Select(p => p.Id));
            var userReactions = currentUserProvider.UserId == null
                ? new Dictionary<string, UserReaction>()
                : await LoadUserReactionsAsync(currentUserProvider.UserId, publications.Select(p => p.Id));

            var news = publications
                .Select(publication => new NewsFeedPublication(
                    publication.Id,
                    publication.Content,
                    publication.CreatedOn,
                    publication.UpdatedOn,
                    featuredComments[publication.Id],
                    new ReactionShort(reactions[publication.Id], userReactions[publication.Id]),
                    ToUser(publication.Author)))
                .ToList();

            return (news, totalCount);
        }

        public async Task<NewsFeedPublication> PublishAsync(string content, string userId)
        {
            var model = new CreatePublicationModel(content, userId);
            var entity = await publicationsApi.CreateAsync(model);

            return new NewsFeedPublication(
                entity.Id,
                entity.Content,
                entity.CreatedOn,
                entity.UpdatedOn,
                new CommentsShort(Enumerable.Empty<PublicationComment>(), 0),
                new ReactionShort(new Dictionary<ReactionType, int>(), null),
                ToUser(entity.Author));
        }

        public async Task UpdateAsync(string publicationId, string content)
        {
            await publicationsApi.UpdateAsync(publicationId, new UpdatePublicationModel(content));
        }

        public async Task DeleteAsync(string publicationId)
        {
            await Reactions.RemoveManyAsync(KeysBuilder.PublicationReactionsKey(publicationId));
            await Comments.DeleteManyAsync(publicationId);
            await publicationsApi.DeleteAsync(publicationId);
        }

        private static PublicationComment ToDomain(Comment entity)
        {
            return new PublicationComment(
                entity.Id,
                entity.Content,
                KeysBuilder.PublicationCommentKey(entity.Key),
                ToUser(entity.Author),
                entity.CreatedOn);
        }

        private static UserInfo ToUser(Content.Model.UserInfo userInfo)
        {
            return new UserInfo(userInfo.Id, userInfo.FullName, userInfo.AvatarUrl);
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

        private async Task<Dictionary<string, CommentsShort>> LoadCommentsAsync(IEnumerable<string> publicationIds)
        {
            var keys = publicationIds.Select(KeysBuilder.PublicationCommentKey).ToList();
            var query = new FeaturedQuery(keys);

            var featuredComments = await commentsApi.SearchFeaturedAsync(query);

            return publicationIds
                .ToDictionary(publicationId => publicationId, publicationId =>
                {
                    var comment = featuredComments.GetValueOrDefault(KeysBuilder.PublicationCommentKey(publicationId));

                    return new CommentsShort(
                        comment?.Comments.Select(c => ToDomain(c)) ?? Enumerable.Empty<PublicationComment>(),
                        comment?.TotalCount ?? 0);
                });
        }

        private async Task<Dictionary<string, Dictionary<ReactionType, int>>> LoadReactionsAsync(IEnumerable<string> publicationIds)
        {
            var keys = publicationIds.Select(KeysBuilder.PublicationReactionsKey).ToList();
            var query = new ReactionsQuery(keys);
            var reactions = await reactionsApi.GetGroupedReactionsAsync(query);

            return publicationIds
                .ToDictionary(id => id, id =>
                {
                    var r = reactions.ContainsKey(KeysBuilder.PublicationReactionsKey(id))
                        ? reactions[KeysBuilder.PublicationReactionsKey(id)]
                        : new Dictionary<string, int>();
                    return r.Keys
                        .Select(k => (Enum.Parse<ReactionType>(k), r[k]))
                        .ToDictionary(o => o.Item1, o => o.Item2);
                });
        }

        private async Task<Dictionary<string, UserReaction>> LoadUserReactionsAsync(string userId, IEnumerable<string> publicationIds)
        {
            var query = new ReactionsQuery
            {
                Keys = publicationIds.Select(KeysBuilder.PublicationReactionsKey).ToList()
            };
            var userReactionsResponse = await reactionsApi.SearchAsync(userId, query);
            var userReactions = userId == null
                ? new Dictionary<string, Reaction>()
                : userReactionsResponse
                    .GroupBy(r => r.Key, r => r)
                    .ToDictionary(k => k.Key, k => k.First());

            return publicationIds
                .ToDictionary(id => id, id => userReactions.ContainsKey(KeysBuilder.PublicationReactionsKey(id))
                    ? new UserReaction(Enum.Parse<ReactionType>(userReactions[KeysBuilder.PublicationReactionsKey(id)].Type))
                    : null);
        }
    }
}
