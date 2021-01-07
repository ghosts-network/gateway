using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Facade;
using GhostNetwork.Publications.Api;
using GhostNetwork.Publications.Model;
using GhostNetwork.Reactions.Api;
using GhostNetwork.Reactions.Client;

namespace GhostNetwork.Infrastructure.Repository
{
    public class NewsFeedStorage : INewsFeedManager
    {
        private readonly IPublicationsApi publicationsApi;
        private readonly ICommentsApi commentsApi;
        private readonly IReactionsApi reactionsApi;

        public NewsFeedStorage(IPublicationsApi publicationsApi, ICommentsApi commentsApi, IReactionsApi reactionsApi)
        {
            this.publicationsApi = publicationsApi;
            this.commentsApi = commentsApi;
            this.reactionsApi = reactionsApi;
        }

        public async Task<(IEnumerable<NewsFeedPublication>, long)> FindManyAsync(int skip, int take, string author)
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
                    var response = await reactionsApi.ReactionsGetAsync($"publication_{publication.Id}");
                    reactions = response.Keys
                        .Select(k => (Enum.Parse<ReactionType>(k), response[k]))
                        .ToDictionary(o => o.Item1, o => o.Item2);
                }
                catch (ApiException)
                {
                    // ignored
                }

                UserReaction userReaction = null;

                if (author != null)
                {
                    try
                    {
                        var reactionByAuthor = await reactionsApi.ReactionsGetReactionByAuthorAsync($"publication_{publication.Id}", author);

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
                    new CommentsShort(commentsResponse.Data.Select(c => new PublicationComment(
                        c.Id, c.Content, c.PublicationId, c.AuthorId, c.CreatedOn)).ToList(), GetTotalCountHeader(commentsResponse)),
                    new ReactionShort(reactions, userReaction)));
            }

            return (newsFeedPublications, GetTotalCountHeader(publicationsResponse));
        }

        public async Task<NewsFeedPublication> CreateAsync(string content, string author)
        {
            var model = new CreatePublicationModel(content, author);
            var entity = await publicationsApi.CreateAsync(model);

            return new NewsFeedPublication(entity.Id, entity.Content, new CommentsShort(Enumerable.Empty<PublicationComment>(), 0), 
                new ReactionShort(new Dictionary<ReactionType, int>(), new UserReaction(new ReactionType())));
        }

        public async Task<ReactionShort> GetReactionsAsync(string publicationId, string author)
        {
            var reactions = new Dictionary<ReactionType, int>();
            try
            {
                var response = await reactionsApi.ReactionsGetAsync($"publication_{publicationId}");
                reactions = response.Keys
                    .Select(k => (Enum.Parse<ReactionType>(k), response[k]))
                    .ToDictionary(o => o.Item1, o => o.Item2);
            }
            catch (ApiException)
            {
                // ignored
            }

            UserReaction userReaction = null;

            if (author != null)
            {
                try
                {
                    var reactionByAuthor = await reactionsApi.ReactionsGetReactionByAuthorAsync($"publication_{publicationId}", author);

                    userReaction = new UserReaction(Enum.Parse<ReactionType>(reactionByAuthor.Type));
                }
                catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
                {
                    // ignored
                }
            }

            return new ReactionShort(reactions, userReaction);
        }

        public async Task AddReactionAsync(string publicationId, string author, ReactionType reaction)
        {
            await reactionsApi.ReactionsAddAsync($"publication_{publicationId}", reaction.ToString(), author);
        }

        public async Task RemoveReactionAsync(string publicationId, string author)
        {
            await reactionsApi.ReactionsDeleteAsync($"publication_{publicationId}", author);
        }

        public async Task UpdateAsync(string id, string content)
        {
            var model = new UpdatePublicationModel(content);

            await publicationsApi.UpdateAsync(id, model);
        }

        public async Task DeleteAsync(string id)
        {
            await publicationsApi.DeleteAsync(id);
        }

        public async Task AddCommentAsync(string publicationId, string author, string content)
        {
            await commentsApi.CreateAsync(new CreateCommentModel(publicationId, content, authorId: author));
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
                entity.AuthorId,
                entity.CreatedOn
                );
        }
    }
}
