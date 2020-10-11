using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Publications.Api;
using GhostNetwork.Publications.Model;

namespace GhostNetwork.Gateway.Facade
{
    public class CommentsPublicationSource
    {
        private readonly ICommentsApi commentsApi;

        public CommentsPublicationSource(ICommentsApi commentsApi)
        {
            this.commentsApi = commentsApi;
        }

        public async Task<DomainResult> CreateAsync(string publicationId, string content, string replyCommentId)
        {
            var comment = new CreateCommentModel(publicationId, content, replyCommentId);
            
            var result = await commentsApi.CommentsCreateAsync(comment);

            if (result != null)
            {
                return DomainResult.Successed();
            }

            return DomainResult.Error("Publication not found.");
        }

        public async Task<List<CommentOfPublication>> FindManyAsync(string publicationId, int skip = 0, int take = 10)
        {
            var comments = await commentsApi.CommentsSearchAsync(publicationId, skip, take);

            return comments.Select(c => new CommentOfPublication(c.Content)).ToList();
        }

    }
}
