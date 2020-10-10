using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GhostNetwork.Publications.Api;
using GhostNetwork.Publications.Model;

namespace GhostNetwork.Gateway.Facade
{
    public class CommentsPublicationSource
    {
        private readonly CommentsApi commentsApi;

        public CommentsPublicationSource(CommentsApi commentsApi)
        {
            this.commentsApi = commentsApi;
        }

        public async Task<CommentOfPublication> CreateAsync(string publicationId, string content, string replyCommentId)
        {
            var comment = new CreateCommentModel(publicationId, content, replyCommentId);

            var result = await commentsApi.CommentsCreateAsync(comment);

            return new CommentOfPublication(result.Content);
        }

    }
}
