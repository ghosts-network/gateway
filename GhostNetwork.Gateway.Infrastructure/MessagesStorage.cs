using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Gateway.Messages;
using GhostNetwork.Messages.Api;
using GhostNetwork.Messages.Client;
using GhostNetwork.Messages.Model;
using Message = GhostNetwork.Gateway.Messages.Message;

namespace GhostNetwork.Gateway.Infrastructure;

public class MessagesStorage : IMessageStorage
{
    private readonly IMessagesApi messagesApi;

    public MessagesStorage(IMessagesApi messagesApi)
    {
        this.messagesApi = messagesApi;
    }

    public async Task<(IEnumerable<Message>, string)> SearchAsync(string chatId, string cursor, int limit)
    {
        var response = await messagesApi.SearchWithHttpInfoAsync(chatId, cursor, limit);

        return (response.Data.Select(ToGatewayMessage), GetCursorHeader(response));
    }

    public async Task<Message> GetByIdAsync(string chatId, string id)
    {
        var message = await messagesApi.GetByIdAsync(chatId, id);

        return message is null ? null : ToGatewayMessage(message);
    }

    public async Task<Message> CreateAsync(string chatId, Guid authorId, string content)
    {
        var entity = await messagesApi.SendAsync(chatId, new CreateMessageModel(authorId, content));

        return entity is null ? null : ToGatewayMessage(entity);
    }

    public async Task<DomainResult> UpdateAsync(string chatId, string messageId, string content)
    {
        try
        {
            await messagesApi.UpdateAsync(chatId, messageId, new UpdateMessageModel(content));
        }
        catch (ApiException)
        {
            return DomainResult.Error("API error!");
        }

        return DomainResult.Success();
    }

    public async Task DeleteAsync(string chatId, string messageId)
    {
        await messagesApi.DeleteAsync(chatId, messageId);
    }

    private static string GetCursorHeader(IApiResponse response)
    {
        return !response.Headers.TryGetValue("X-Cursor", out var headers)
            ? default
            : headers.FirstOrDefault();
    }

    private static Message ToGatewayMessage(GhostNetwork.Messages.Model.Message message)
    {
        return new Message
        {
            Id = message.Id,
            ChatId = message.ChatId,
            Author = new(message.Author.Id, message.Author.FullName, message.Author.AvatarUrl),
            Content = message.Content,
            SentOn = message.SentOn,
            UpdatedOn = message.UpdatedOn
        };
    }
}