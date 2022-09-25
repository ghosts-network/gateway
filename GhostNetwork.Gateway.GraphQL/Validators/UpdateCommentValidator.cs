using FluentValidation;
using GhostNetwork.Gateway.GraphQL.Models.InputModels;

namespace GhostNetwork.Gateway.GraphQL.Validators
{
    public class UpdateCommentValidator : AbstractValidator<UpdateCommentInput>
    {
        public UpdateCommentValidator()
        {
            RuleFor(x => x.CommentId)
                .NotEmpty();

            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Comment cannot be empty");
        }
    }
}
