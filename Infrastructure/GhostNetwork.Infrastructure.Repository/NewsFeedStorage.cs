using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Facade;
using GhostNetwork.Publications.Api;
using GhostNetwork.Publications.Model;
using GhostNetwork.Reactions.Api;

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

        public async Task<IEnumerable<NewsFeedPublication>> FindManyAsync()
        {
            var publications = await publicationsApi.PublicationsSearchAsync();
            var newsFeedPublications = new List<NewsFeedPublication>(publications.Count);

            foreach (var publication in publications)
            {
                var commentsResponse = await commentsApi.CommentsSearchAsyncWithHttpInfo(publication.Id, 0, 3);
                var totalCount = 0;
                if (commentsResponse.Headers.TryGetValue("X-TotalCount", out var headers))
                {
                    if (!int.TryParse(headers.FirstOrDefault(), out totalCount))
                    {
                        totalCount = 0;
                    }
                }

                var reactions = await reactionsApi.ReactionsGetAsync($"publication_{publication.Id}");
                var r = reactions.Keys
                    .Select(k => (Enum.Parse<ReactionType>(k), reactions[k]))
                    .ToDictionary(o => o.Item1, o => o.Item2);

                newsFeedPublications.Add(new NewsFeedPublication(
                    publication.Id,
                    publication.Content,
                    new CommentsShort(commentsResponse.Data.Select(c => new PublicationComment(c.Id, c.Content)).ToList(), totalCount),
                    new ReactionShort(r)));
            }

            return newsFeedPublications;
        }

        public async Task CreateAsync(string content, string author)
        {
            var model = new CreatePublicationModel(content, author);

            await publicationsApi.PublicationsCreateAsync(model);
        }

        public async Task AddCommentAsync(string publicationId, string author, string content)
        {
            await commentsApi.CommentsCreateAsync(new CreateCommentModel(publicationId, content, authorId: author));
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

            await publicationsApi.PublicationsUpdateAsync(id, model);
        }

        public async Task DeleteAsync(string id)
        {
            await publicationsApi.PublicationsDeleteAsync(id);
        }
    }
}
