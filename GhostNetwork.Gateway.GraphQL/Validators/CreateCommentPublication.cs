using FluentValidation;
using GhostNetwork.Gateway.GraphQL.Models.InputModels;

namespace GhostNetwork.Gateway.GraphQL.Validators
{
    public class CreateCommentValidator : AbstractValidator<CreateCommentInput>
    {
        public CreateCommentValidator()
        {
            RuleFor(x => x.PublicationId)
                .NotEmpty();

            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Comment cannot be empty");
        }
    }
}
