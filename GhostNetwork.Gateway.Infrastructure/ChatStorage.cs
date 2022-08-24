using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Gateway.Chats;
using GhostNetwork.Messages.Api;
using GhostNetwork.Messages.Client;
using GhostNetwork.Messages.Model;
using Chat = GhostNetwork.Gateway.Chats.Chat;

namespace GhostNetwork.Gateway.Infrastructure;

public class ChatStorage : IChatStorage
{
    private readonly IChatsApi chatsApi;
    private readonly ChatValidator validator;

    public ChatStorage(IChatsApi chatsApi, ChatValidator validator)
    {
        this.chatsApi = chatsApi;
        this.validator = validator;
    }

    public async Task<(IEnumerable<Chat>, string)> GetAsync(string userId, string cursor, int limit)
    {
        var response = await chatsApi.SearchWithHttpInfoAsync(Guid.Parse(userId), cursor, limit);

        return (response.Data.Select(chat => new Chat(chat.Id, chat.Name, chat.Participants.Select(p => new UserInfo(p.Id, p.FullName, p.AvatarUrl)))),
            GetCursorHeader(response));
    }

    public async Task<Chat> GetByIdAsync(string id)
    {
        try
        {
            var chat = await chatsApi.GetByIdAsync(id);

            return chat is null ? null : new Chat(chat.Id, chat.Name, chat.Participants.Select(p => new UserInfo(p.Id, p.FullName, p.AvatarUrl)));
        }
        catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Chat> CreateAsync(string name, List<Guid> participants)
    {
        await validator.ValidateAsync(new ChatContext(name, participants));

        var chat = await chatsApi.CreateAsync(new CreateChatModel(name, participants));

        return chat is null ? null : new Chat(chat.Id, chat.Name, chat.Participants.Select(p => new UserInfo(p.Id, p.FullName, p.AvatarUrl)));
    }

    public async Task<DomainResult> UpdateAsync(string id, string name, List<Guid> participants)
    {
        await validator.ValidateAsync(new ChatContext(name, participants));

        try
        {
            await chatsApi.UpdateAsync(id, new UpdateChatModel(name, participants.ToList()));
        }
        catch (ApiException ex)
        {
            return DomainResult.Error($"API error! {ex.ErrorCode} {ex.Message}");
        }

        return DomainResult.Success();
    }

    public async Task DeleteAsync(string id)
    {
        await chatsApi.DeleteAsync(id);
    }

    private static string GetCursorHeader(IApiResponse response)
    {
        return !response.Headers.TryGetValue("X-Cursor", out var headers)
            ? default
            : headers.FirstOrDefault();
    }
}