using FluentValidation;
using GhostNetwork.Gateway.GraphQL.Models.InputModels;

namespace GhostNetwork.Gateway.GraphQL.Validators
{
    public class CreatePublicationValidator : AbstractValidator<CreatePublicationInput>
    {
        public CreatePublicationValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Publication cannot be empty");
        }
    }
}
