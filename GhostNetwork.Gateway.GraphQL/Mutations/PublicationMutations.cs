using AppAny.HotChocolate.FluentValidation;
using GhostNetwork.Content.Api;
using GhostNetwork.Content.Model;
using GhostNetwork.Gateway.GraphQL.Models;
using GhostNetwork.Gateway.GraphQL.Models.InputModels;
using GhostNetwork.Gateway.GraphQL.Payloads;

namespace GhostNetwork.Gateway.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class PublicationMutations
    {
        private readonly IPublicationsApi publicationApi;
        private readonly ICurrentUserProvider currentUserProvider;

        public PublicationMutations(
            IPublicationsApi publicationApi,
            ICurrentUserProvider currentUserProvider)
        {
            this.publicationApi = publicationApi;
            this.currentUserProvider = currentUserProvider;
        }

        [GraphQLName("createPublication")]
        public async Task<CreatePublicationPayload> CreatePublicationAsync(
            [UseFluentValidation] CreatePublicationInput publicationInput, 
            CancellationToken cancellationToken)
        {
            var author = await currentUserProvider.GetProfileAsync();
            var publication = await publicationApi.CreateAsync(
                new CreatePublicationModel(publicationInput.Content,
                new UserInfoModel(author.Id, author.FullName, author.AvatarUrl)),
                cancellationToken: cancellationToken);

            return new CreatePublicationPayload
            {
                Publication = new PublicationEntity
                {
                    Id = publication.Id,
                    Content = publication.Content,
                    CreatedOn = publication.CreatedOn,
                    UpdatedOn = publication.UpdatedOn
                }
            };
        }

        [GraphQLName("updatePublication")]
        public async Task<UpdatePublicationPayload> UpdatePublicationAsync(
            [UseFluentValidation] UpdatePublicationInput input,
            CancellationToken cancellationToken)
        {
            var response = await publicationApi.UpdateWithHttpInfoAsync(
                                    input.PublicationId, 
                                    new UpdatePublicationModel(input.Content), 
                                    cancellationToken: cancellationToken);

            return new UpdatePublicationPayload
            {
                Code = (int)response.StatusCode
            };
        }

        [GraphQLName("deletePublication")]
        public async Task<DeletePublicationPayload> DeletePublicationAsync(
            [UseFluentValidation] DeletePublicationInput input,
            CancellationToken cancellationToken)
        {
            var response = await publicationApi.DeleteWithHttpInfoAsync(input.PublicationId, cancellationToken: cancellationToken);

            return new DeletePublicationPayload
            {
                Code = (int)response.StatusCode
            };
        }
    }
}
