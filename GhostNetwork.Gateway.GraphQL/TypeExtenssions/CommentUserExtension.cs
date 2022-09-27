using GhostNetwork.Gateway.GraphQL.DataLoaders;
using GhostNetwork.Gateway.GraphQL.Models;

namespace GhostNetwork.Gateway.GraphQL.TypeExtenssions
{
    [ExtendObjectType(typeof(CommentEntity))]
    public class CommentUserExtension
    {
        [GraphQLName("author")]
        public async Task<UserInfoEntity> GetAuthorAsync(
            [Parent] CommentEntity comment,
            UserByIdDataLoader dataLoader,
            CancellationToken cancellationToken)
        {
            return await dataLoader.LoadAsync(comment.AuthorId, cancellationToken: cancellationToken);
        }
    }
}
