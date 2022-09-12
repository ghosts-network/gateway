using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Validation;
using GhostNetwork.Gateway.Chats;

namespace GhostNetwork.Gateway.Messages;

public class MessageValidator : IValidator<MessageContext>
{
    private readonly IChatStorage chatStorage;

    public MessageValidator(IChatStorage chatStorage)
    {
        this.chatStorage = chatStorage;
    }

    public async Task<DomainResult> ValidateAsync(MessageContext context)
    {
        var errors = new List<DomainError>();

        if (string.IsNullOrEmpty(context.Content) || context.Content.Length > 500)
        {
            errors.Add(new DomainError("Content is required and must be less than 500 characters"));
        }

        var chat = await chatStorage.GetByIdAsync(context.ChatId);

        if (chat is null)
        {
            errors.Add(new DomainError("Chat not found"));
        }
        else if (chat.Participants.All(x => x.Id != context.AuthorId))
        {
            errors.Add(new DomainError("You are not a participant of this chat!"));
        }

        return DomainResult.Error(errors);
    }
}