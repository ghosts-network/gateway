using GhostNetwork.Content.Api;
using GhostNetwork.Content.Model;
using GhostNetwork.Gateway.GraphQL.Models;

namespace GhostNetwork.Gateway.GraphQL.Queries
{
    public class NewsFeedPublicationQuery
    {
        private readonly IPublicationsApi publicationApi;
        private readonly ICurrentUserProvider currentUserProvider;

        public NewsFeedPublicationQuery(
            ICurrentUserProvider currentUserProvider,
            IPublicationsApi publicationApi)
        {
            this.publicationApi = publicationApi;
            this.currentUserProvider = currentUserProvider;
        }

        [GraphQLName("publications")]
        public async Task<IEnumerable<PublicationEntity>> GetPublicationsByUserIdAsync(int? take, string? cursor, Ordering? order, CancellationToken cancellationToken)
        {
            var publications = await publicationApi.SearchAsync(cursor, take, order: order, cancellationToken: cancellationToken);
            
            return publications.Select(p => new PublicationEntity
            {
                Id = p.Id,
                Content = p.Content,
                CreatedOn = p.CreatedOn,
                UpdatedOn = p.UpdatedOn,
                AuthorId = p.Author.Id.ToString()
            });
        }
    }
}
