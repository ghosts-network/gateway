using GhostNetwork.Gateway.GraphQL.DataLoaders;
using GhostNetwork.Gateway.GraphQL.Models;

namespace GhostNetwork.Gateway.GraphQL.TypeExtenssions
{
    [ExtendObjectType(typeof(PublicationEntity))]
    public class PublicationReactionsExtension
    {
        [GraphQLName("reactions")]
        public async Task<IEnumerable<ReactionsEntity>> GetReactionsAsync(
            [Parent] PublicationEntity publication,
            ReactionsByPublicationIdDataLoader dataLoader,
            CancellationToken cancelettionToken)
        {
            return await dataLoader.LoadAsync($"publication_{publication.Id}", cancellationToken: cancelettionToken);
        }
    }
}
