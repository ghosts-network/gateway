using FluentValidation;
using GhostNetwork.Gateway.GraphQL.Models.InputModels;

namespace GhostNetwork.Gateway.GraphQL.Validators
{
    public class DeleteCommentValidator : AbstractValidator<DeleteCommentInput>
    {
        public DeleteCommentValidator()
        {
            RuleFor(x => x.CommentId)
                .NotEmpty();
        }
    }
}
