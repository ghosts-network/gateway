using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Publications.Api;
using GhostNetwork.Publications.Model;

namespace GhostNetwork.Gateway.Facade
{
    public class NewsFeedPublicationsSource
    {
        private readonly IPublicationsApi publicationsApi;
        private readonly ICommentsApi commentsApi;

        public NewsFeedPublicationsSource(IPublicationsApi publicationsApi, ICommentsApi commentsApi)
        {
            this.publicationsApi = publicationsApi;
            this.commentsApi = commentsApi;
        }

        public async Task<ICollection<NewsFeedPublication>> FindManyAsync()
        {
            var publications = await publicationsApi.PublicationsSearchAsync();
            var newsFeedPublications = new List<NewsFeedPublication>(publications.Count);

            foreach (var publication in publications)
            {
                var commentsResponse = await commentsApi.CommentsSearchAsyncWithHttpInfo(publication.Id, 0, 1);
                var totalCount = 0;
                if (commentsResponse.Headers.TryGetValue("X-TotalCount", out var headers))
                {
                    if (!int.TryParse(headers.FirstOrDefault(), out totalCount))
                    {
                        totalCount = 0;
                    }
                }

                var reactions = new ReactionShort(new Dictionary<ReactionType, int>
                {
                    [ReactionType.Like] = 4,
                    [ReactionType.Love] = 2
                });

                newsFeedPublications.Add(new NewsFeedPublication(
                    publication.Id,
                    publication.Content,
                    totalCount,
                    reactions));
            }

            return newsFeedPublications;
        }

        public async Task CreateAsync(string content)
        {
            var model = new CreatePublicationModel(content);

            await publicationsApi.PublicationsCreateAsync(model);
        }

        public async Task AddCommentAsync(string publicationId, string content)
        {
            var model = new CreateCommentModel(publicationId, content);

            await commentsApi.CommentsCreateAsync(model);
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
