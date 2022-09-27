using AppAny.HotChocolate.FluentValidation;
using GhostNetwork.Content.Api;
using GhostNetwork.Gateway.GraphQL.Models.InputModels;
using GhostNetwork.Gateway.GraphQL.Payloads;
using System.Net;

namespace GhostNetwork.Gateway.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class ReactionMutations
    {
        private readonly IReactionsApi reactionsApi;
        private readonly IPublicationsApi publicationsApi;
        private readonly ICurrentUserProvider currentUserProvider;

        public ReactionMutations(
            IReactionsApi reactionsApi,
            IPublicationsApi publicationsApi,
            ICurrentUserProvider currentUserProvider)
        {
            this.reactionsApi = reactionsApi;
            this.publicationsApi = publicationsApi;
            this.currentUserProvider = currentUserProvider;
        }

        [GraphQLName("upsertReaction")]
        public async Task<UpsertReactionPayload> UpsertReactionAsync(
            [UseFluentValidation] UpsertReactionInput input,
            CancellationToken cancellationToken)
        {
            if ((await publicationsApi.GetByIdAsync(input.PublicationId, cancellationToken: cancellationToken)) == null)
            {
                return new UpsertReactionPayload
                {
                    Code = (int)HttpStatusCode.NotFound,
                    ReactionType = null
                };
            }

            await reactionsApi.UpsertAsync(
                KeysBuilder.PublicationReactionsKey(input.PublicationId),
                input.ReactionType.ToString(),
                currentUserProvider.UserId,
                cancellationToken: cancellationToken
            );

            return new UpsertReactionPayload
            {
                Code = (int)HttpStatusCode.OK,
                ReactionType = input.ReactionType
            };
        }

        [GraphQLName("deleteReaction")]
        public async Task<DeleteReactionPayload> DeleteReactionAsync(
            [UseFluentValidation] DeleteReactionInput input,
            CancellationToken cancellationToken)
        {
            if ((await publicationsApi.GetByIdAsync(input.PublicationId, cancellationToken: cancellationToken)) == null)
            {
                return new DeleteReactionPayload
                {
                    Code = (int)HttpStatusCode.NotFound
                };
            }

            await reactionsApi.DeleteByAuthorAsync(
                KeysBuilder.PublicationReactionsKey(input.PublicationId),
                currentUserProvider.UserId,
                cancellationToken: cancellationToken
            );

            return new DeleteReactionPayload
            {
                Code= (int)HttpStatusCode.OK,
            };
        }
    }
}