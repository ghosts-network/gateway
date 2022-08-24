using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    private readonly MessageValidator validator;

    public MessagesStorage(IMessagesApi messagesApi, MessageValidator validator)
    {
        this.messagesApi = messagesApi;
        this.validator = validator;
    }

    public async Task<(IEnumerable<Message>, string)> SearchAsync(string chatId, string cursor, int limit)
    {
        var response = await messagesApi.SearchWithHttpInfoAsync(chatId, cursor, limit);

        return (response.Data.Select(ToGatewayMessage), GetCursorHeader(response));
    }

    public async Task<Message> GetByIdAsync(string chatId, string id)
    {
        try
        {
            var message = await messagesApi.GetByIdAsync(chatId, id);

            return message is null ? null : ToGatewayMessage(message);
        }
        catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<(DomainResult, Message)> CreateAsync(string chatId, Guid authorId, string content)
    {
        var result = await validator.ValidateAsync(new MessageContext(chatId, authorId, content));

        if (!result.Successed)
        {
            return (result, default);
        }

        var entity = await messagesApi.SendAsync(chatId, new CreateMessageModel(authorId, content));

        return (DomainResult.Success(), entity is null ? null : ToGatewayMessage(entity));
    }

    public async Task<DomainResult> UpdateAsync(string chatId, string messageId, Guid authorId, string content)
    {
        var result = await validator.ValidateAsync(new MessageContext(chatId, authorId, content));

        if (!result.Successed)
        {
            return result;
        }

        try
        {
            await messagesApi.UpdateAsync(chatId, messageId, new UpdateMessageModel(content));
        }
        catch (ApiException ex)
        {
            return DomainResult.Error(ex.Message);
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
        return new Message(
            message.Id,
            message.ChatId,
            new UserInfo(message.Author.Id, message.Author.FullName, message.Author.AvatarUrl),
            message.Content,
            message.SentOn,
            message.UpdatedOn);
    }
}