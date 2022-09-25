using FluentValidation;
using GhostNetwork.Gateway.GraphQL.Models.InputModels;

namespace GhostNetwork.Gateway.GraphQL.Validators
{
    public class UpdatePublicationValidator : AbstractValidator<UpdatePublicationInput>
    {
        public UpdatePublicationValidator()
        {
            RuleFor(x => x.PublicationId)
                .NotEmpty();
            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Publication cannot be empty");
        }
    }
}
