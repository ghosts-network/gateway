using GhostNetwork.Gateway.GraphQL.DataLoaders;
using GhostNetwork.Gateway.GraphQL.Models;

namespace GhostNetwork.Gateway.GraphQL.TypeExtenssions
{
    [ExtendObjectType(typeof(PublicationEntity))]
    public class PublicationCommentExtension
    {
        [GraphQLName("comments")]
        public async Task<IEnumerable<CommentEntity>> GetCommentsAsync(
            [Parent] PublicationEntity publication,
            CommentsByPublicationIdDataLoader byPublicationIdDataLoader,
            CancellationToken cancellationToken)
        {
            return await byPublicationIdDataLoader.LoadAsync($"publication_{publication.Id}", cancellationToken);
        }
    }
}
