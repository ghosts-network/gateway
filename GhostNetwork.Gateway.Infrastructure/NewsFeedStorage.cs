using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Content.Api;
using GhostNetwork.Content.Client;
using GhostNetwork.Content.Model;
using GhostNetwork.Gateway.NewsFeed;
using Media = GhostNetwork.Gateway.NewsFeed.Media;

namespace GhostNetwork.Gateway.Infrastructure
{
    public class NewsFeedStorage : INewsFeedStorage
    {
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly NewsFeedApi newsFeedApi;
        private readonly IPublicationsApi publicationsApi;
        private readonly ICommentsApi commentsApi;
        private readonly IReactionsApi reactionsApi;

        public NewsFeedStorage(
            IPublicationsApi publicationsApi,
            ICommentsApi commentsApi,
            IReactionsApi reactionsApi,
            ICurrentUserProvider currentUserProvider,
            INewsFeedMediaStorage mediaStorage,
            NewsFeedApi newsFeedApi)
        {
            this.publicationsApi = publicationsApi;
            this.commentsApi = commentsApi;
            this.reactionsApi = reactionsApi;
            this.currentUserProvider = currentUserProvider;
            this.newsFeedApi = newsFeedApi;

            Reactions = new NewsFeedReactionsStorage(reactionsApi);
            Comments = new NewsFeedCommentsStorage(commentsApi);
            Media = mediaStorage;
        }

        public INewsFeedReactionsStorage Reactions { get; }

        public INewsFeedCommentsStorage Comments { get; }

        public INewsFeedMediaStorage Media { get; }

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
                    publication.Media is null ? Array.Empty<Media>() : publication.Media.Select(x => new Media(x.Link)),
                    ToUser(publication.Author));
            }
            catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<(IEnumerable<NewsFeedPublication>, string)> GetPersonalizedFeedAsync(string userId, int take, string cursor)
        {
            var (publications, crs) = await newsFeedApi.GetUserFeedAsync(userId, take, cursor);

            var featuredComments = await LoadCommentsAsync(publications.Select(p => p.Id).ToList());
            var reactions = await LoadReactionsAsync(publications.Select(p => p.Id).ToList());
            var userReactions = userId == null
                ? new Dictionary<string, UserReaction>()
                : await LoadUserReactionsAsync(userId, publications.Select(p => p.Id).ToList());

            var news = publications
                .Select(publication => new NewsFeedPublication(
                    publication.Id,
                    publication.Content,
                    publication.CreatedOn,
                    publication.UpdatedOn,
                    featuredComments[publication.Id],
                    new ReactionShort(reactions[publication.Id], userReactions[publication.Id]),
                    publication.Media is null ? Array.Empty<Media>() : publication.Media.Select(x => new Media(x.Link)),
                    ToUser(publication.Author)))
                .ToList();

            return (news, crs);
        }

        public async Task<(IEnumerable<NewsFeedPublication>, string)> GetUserFeedAsync(string userId, int take, string cursor)
        {
            var publicationsResponse = await publicationsApi.SearchWithHttpInfoAsync(cursor: cursor, take: take, order: Ordering.Desc);
            var publications = publicationsResponse.Data;
            var crs = GetCursorHeader(publicationsResponse);

            var featuredComments = await LoadCommentsAsync(publications.Select(p => p.Id).ToList());
            var reactions = await LoadReactionsAsync(publications.Select(p => p.Id).ToList());
            var userReactions = userId == null
                ? new Dictionary<string, UserReaction>()
                : await LoadUserReactionsAsync(userId, publications.Select(p => p.Id).ToList());

            var news = publications
                .Select(publication => new NewsFeedPublication(
                    publication.Id,
                    publication.Content,
                    publication.CreatedOn,
                    publication.UpdatedOn,
                    featuredComments[publication.Id],
                    new ReactionShort(reactions[publication.Id], userReactions[publication.Id]),
                    publication.Media is null ? Array.Empty<Media>() : publication.Media.Select(x => new Media(x.Link)),
                    ToUser(publication.Author)))
                .ToList();

            return (news, crs);
        }

        public async Task<(IEnumerable<NewsFeedPublication>, string)> GetUserPublicationsAsync(Guid userId, int take, string cursor)
        {
            var publicationsResponse = await publicationsApi.SearchByAuthorWithHttpInfoAsync(userId, cursor: cursor, take: take, order: Ordering.Desc);
            var publications = publicationsResponse.Data;
            var crs = GetCursorHeader(publicationsResponse);

            var featuredComments = await LoadCommentsAsync(publications.Select(p => p.Id).ToList());
            var reactions = await LoadReactionsAsync(publications.Select(p => p.Id).ToList());
            var userReactions = currentUserProvider.UserId == null
                ? new Dictionary<string, UserReaction>()
                : await LoadUserReactionsAsync(currentUserProvider.UserId, publications.Select(p => p.Id).ToList());

            var news = publications
                .Select(publication => new NewsFeedPublication(
                    publication.Id,
                    publication.Content,
                    publication.CreatedOn,
                    publication.UpdatedOn,
                    featuredComments[publication.Id],
                    new ReactionShort(reactions[publication.Id], userReactions[publication.Id]),
                    publication.Media is null ? Array.Empty<Media>() : publication.Media.Select(x => new Media(x.Link)),
                    ToUser(publication.Author)))
                .ToList();

            return (news, crs);
        }

        public async Task<NewsFeedPublication> PublishAsync(string content, UserInfo author, IEnumerable<MediaStream> mediaStreams)
        {
            var authorContent = new UserInfoModel(author.Id, author.FullName, author.AvatarUrl);
            var model = new CreatePublicationModel(content, author: authorContent);

            try
            {
                var media = await Media.UploadAsync(mediaStreams, author.Id.ToString());
                var entity = await publicationsApi.CreateAsync(model);

                return new NewsFeedPublication(
                    entity.Id,
                    entity.Content,
                    entity.CreatedOn,
                    entity.UpdatedOn,
                    new CommentsShort(Enumerable.Empty<PublicationComment>(), 0),
                    new ReactionShort(new Dictionary<ReactionType, int>(), null),
                    media,
                    ToUser(entity.Author));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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

        private static string GetCursorHeader(IApiResponse response)
        {
            return !response.Headers.TryGetValue("X-Cursor", out var headers)
                ? default
                : headers.FirstOrDefault();
        }

        private async Task<Dictionary<string, CommentsShort>> LoadCommentsAsync(IReadOnlyCollection<string> publicationIds)
        {
            var keys = publicationIds.Select(KeysBuilder.PublicationCommentKey).ToList();
            var query = new FeaturedQuery(keys);

            var featuredComments = await commentsApi.SearchFeaturedAsync(query);

            return publicationIds
                .ToDictionary(publicationId => publicationId, publicationId =>
                {
                    var comment = featuredComments.GetValueOrDefault(KeysBuilder.PublicationCommentKey(publicationId));

                    return new CommentsShort(
                        comment?.Comments.Select(ToDomain) ?? Enumerable.Empty<PublicationComment>(),
                        comment?.TotalCount ?? 0);
                });
        }

        private async Task<Dictionary<string, Dictionary<ReactionType, int>>> LoadReactionsAsync(IReadOnlyCollection<string> publicationIds)
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

        private async Task<Dictionary<string, UserReaction>> LoadUserReactionsAsync(string userId, IReadOnlyCollection<string> publicationIds)
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
