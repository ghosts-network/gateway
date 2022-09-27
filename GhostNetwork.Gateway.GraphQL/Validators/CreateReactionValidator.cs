using FluentValidation;
using GhostNetwork.Gateway.GraphQL.Models.InputModels;

namespace GhostNetwork.Gateway.GraphQL.Validators
{
    public class UpsertReactionValidator : AbstractValidator<UpsertReactionInput>
    {
        public UpsertReactionValidator()
        {
            RuleFor(x => x.PublicationId)
                .NotEmpty();
        }
    }
}
