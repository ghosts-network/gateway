using FluentValidation;
using GhostNetwork.Gateway.GraphQL.Models.InputModels;

namespace GhostNetwork.Gateway.GraphQL.Validators
{
    public class DeleteReactionValidator : AbstractValidator<DeleteReactionInput>
    {
        public DeleteReactionValidator()
        {
            RuleFor(x => x.PublicationId)
                .NotEmpty();
        }
    }
}
