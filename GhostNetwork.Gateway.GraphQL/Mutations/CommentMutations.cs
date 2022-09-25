using AppAny.HotChocolate.FluentValidation;
using GhostNetwork.Content.Api;
using GhostNetwork.Content.Model;
using GhostNetwork.Gateway.GraphQL.Models;
using GhostNetwork.Gateway.GraphQL.Models.InputModels;
using GhostNetwork.Gateway.GraphQL.Payloads;
using System.Net;

namespace GhostNetwork.Gateway.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class CommentMutations
    {
        private readonly IPublicationsApi publicationsApi;
        private readonly ICommentsApi commentsApi;
        private readonly ICurrentUserProvider currentUserProvider;

        public CommentMutations(
            IPublicationsApi publicationsApi,
            ICommentsApi commentsApi,
            ICurrentUserProvider currentUserProvider)
        {
            this.commentsApi = commentsApi;
            this.publicationsApi = publicationsApi;
            this.currentUserProvider = currentUserProvider;
        }

        [GraphQLName("createComment")]
        public async Task<CreateCommentPayload> CreateCommentAsync(
            [UseFluentValidation] CreateCommentInput input,
            CancellationToken cancellationToken)
        {
            if ((await publicationsApi.GetByIdAsync(input.PublicationId)) == null)
            {
                return new CreateCommentPayload
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Comment = null
                };
            }

            var author = await currentUserProvider.GetProfileAsync();
            var comment = await commentsApi.CreateAsync(
                new CreateCommentModel(
                    KeysBuilder.PublicationCommentKey(input.PublicationId), 
                    input.Content, 
                    input.CommentReplyId, 
                    new UserInfoModel(author.Id, author.FullName, author.AvatarUrl)), 
                cancellationToken: cancellationToken);

            return new CreateCommentPayload
            {
                Code = (int)HttpStatusCode.OK,
                Comment = new CommentEntity
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    AuthorId = comment.Author.Id.ToString(),
                    CreatedOn = comment.CreatedOn,
                    Key = comment.Key
                }
            };
        }

        [GraphQLName("updateComment")]
        public async Task<UpdateCommentPayload> UpdateCommentAsync(
            [UseFluentValidation] UpdateCommentInput input,
            CancellationToken cancellationToken)
        {
            if ((await commentsApi.GetByIdAsync(input.CommentId, cancellationToken: cancellationToken)) == null)
            {
                return new UpdateCommentPayload
                {
                    Code = (int)HttpStatusCode.NotFound
                };
            }

            var response = await commentsApi.UpdateWithHttpInfoAsync(input.CommentId, new UpdateCommentModel(input.Content), cancellationToken: cancellationToken);

            return new UpdateCommentPayload
            {
                Code = (int)response.StatusCode
            };
        }

        [GraphQLName("deleteComment")]
        public async Task<DeleteCommentPayload> DeleteCommentAsync(
            [UseFluentValidation] DeleteCommentInput input,
            CancellationToken cancellationToken)
        {
            if ((await commentsApi.GetByIdAsync(input.CommentId, cancellationToken: cancellationToken)) == null)
            {
                return new DeleteCommentPayload
                {
                    Code = (int)HttpStatusCode.NotFound
                };
            }

            var response = await commentsApi.DeleteWithHttpInfoAsync(input.CommentId, cancellationToken: cancellationToken);

            return new DeleteCommentPayload
            {
                Code = (int)response.StatusCode
            };
        }
    }
}
