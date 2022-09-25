using GhostNetwork.Content.Api;
using GhostNetwork.Content.Model;
using GhostNetwork.Gateway.GraphQL.Models;

namespace GhostNetwork.Gateway.GraphQL.DataLoaders
{
    public class CommentsByPublicationIdDataLoader : BatchDataLoader<string, IEnumerable<CommentEntity>>
    {
        private readonly ICommentsApi commentsApi;

        public CommentsByPublicationIdDataLoader(
            ICommentsApi commentsApi,
            IBatchScheduler batchScheduler, 
            DataLoaderOptions? options = null) : base(batchScheduler, options)
        {
            this.commentsApi = commentsApi;
        }

        protected override async Task<IReadOnlyDictionary<string, IEnumerable<CommentEntity>>> LoadBatchAsync(IReadOnlyList<string> keys, CancellationToken cancellationToken)
        {
            var response = await commentsApi.SearchFeaturedAsync(new FeaturedQuery(keys.ToList()), cancellationToken: cancellationToken);

            return response.ToDictionary(k => k.Key, v => new List<CommentEntity>(v.Value.Comments.Select(c => new CommentEntity
            {
                Id = c.Id,
                Content = c.Content,
                Key = c.Key,
                AuthorId = c.Author.Id.ToString(),
                CommentReplyId = c.ReplyCommentId,
                CreatedOn = c.CreatedOn,
            })) as IEnumerable<CommentEntity>);
        }
    }
}
