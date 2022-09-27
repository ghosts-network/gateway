using FluentValidation;
using GhostNetwork.Gateway.GraphQL.Models.InputModels;

namespace GhostNetwork.Gateway.GraphQL.Validators
{
    public class DeletePublicationValidator : AbstractValidator<DeletePublicationInput>
    {
        public DeletePublicationValidator()
        {
            RuleFor(x => x.PublicationId)
                .NotEmpty();
        }
    }
}
