using GhostNetwork.Gateway.GraphQL.DataLoaders;
using GhostNetwork.Gateway.GraphQL.Models;

namespace GhostNetwork.Gateway.GraphQL.TypeExtenssions
{
    [ExtendObjectType(typeof(PublicationEntity))]
    public class PublicationUserExtension
    {
        [GraphQLName("author")]
        public async Task<UserInfoEntity> GetAuthorAsync(
            [Parent] PublicationEntity publication,
            UserByIdDataLoader userByIdDataLoader,
            CancellationToken cancellationToken)
        {
            return await userByIdDataLoader.LoadAsync(publication.AuthorId, cancellationToken);
        }
    }
}
