using GhostNetwork.Content.Api;
using GhostNetwork.Content.Model;
using GhostNetwork.Gateway.GraphQL.Models;
using GhostNetwork.Gateway.GraphQL.Payloads;

namespace GhostNetwork.Gateway.GraphQL.Queries
{
    public class NewsFeedPublicationQuery
    {
        private readonly IPublicationsApi publicationApi;

        public NewsFeedPublicationQuery(IPublicationsApi publicationApi)
        {
            this.publicationApi = publicationApi;
        }

        [GraphQLName("publications")]
        public async Task<NewsFeedPayload> GetNewsFeedAsync(int? take, string? cursor, Ordering? order, CancellationToken cancellationToken)
        {
            var publications = await publicationApi.SearchAsync(cursor, take, order: order, cancellationToken: cancellationToken);

            return new NewsFeedPayload
            {
                Publications = publications.Select(p => new PublicationEntity
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedOn = p.CreatedOn,
                    UpdatedOn = p.UpdatedOn,
                    Author = new UserInfoEntity
                    {
                        Id = p.Author.Id.ToString(),
                        FirstName = p.Author.FullName.Split(' ')[0],
                        LastName = p.Author.FullName.Split(' ')[1],
                        AvatarUrl = p.Author.AvatarUrl
                    }
                }),
                Cursor = publications.LastOrDefault()?.Id ?? null
            };
        }
    }
}
