using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;

namespace GhostNetwork.Gateway.Chats;

public class ChatValidator : IValidator<ChatContext>
{
    public Task<DomainResult> ValidateAsync(ChatContext context)
    {
        var errors = new List<DomainError>();

        if (string.IsNullOrEmpty(context.Name) || context.Name.Length is > 256 or < 3)
        {
            errors.Add(new DomainError("Name is cannot be empty or less than 3 or more than 256 characters!"));
        }

        if (context.Participants.Count() > 10)
        {
            errors.Add(new DomainError("Cannot be more than 10 participants!"));
        }

        return Task.FromResult(DomainResult.Error(errors));
    }
}