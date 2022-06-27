using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Messages;
using GhostNetwork.Messages.Api;
using GhostNetwork.Messages.Model;
using Chat = GhostNetwork.Gateway.Messages.Chat;

namespace GhostNetwork.Gateway.Infrastructure;

public class ChatStorage : IChatStorage
{
    private readonly IChatsApi chatsApi;

    public ChatStorage(IChatsApi chatsApi)
    {
        this.chatsApi = chatsApi;
    }

    public async Task<IEnumerable<Chat>> GetAsync(string userId, string cursor, int limit)
    {
        var chats = await chatsApi.SearchAsync(Guid.Parse(userId), cursor, limit);

        return chats.Select(ToGatewayChat);
    }

    public async Task<Chat> GetByIdAsync(string id)
    {
        var chat = await chatsApi.GetByIdAsync(id);

        return chat is null ? null : ToGatewayChat(chat);
    }

    public async Task<Chat> CreateAsync(string name, IEnumerable<Guid> participants)
    {
        var chat = await chatsApi.CreateAsync(new CreateChatModel(name, participants.ToList()));

        return chat is null ? null : ToGatewayChat(chat);
    }

    public async Task UpdateAsync(string id, string name, IEnumerable<Guid> participants)
    {
        await chatsApi.UpdateAsync(id, new UpdateChatModel(name, participants.ToList()));
    }

    public async Task DeleteAsync(string id)
    {
        await chatsApi.DeleteAsync(id);
    }

    private static Chat ToGatewayChat(GhostNetwork.Messages.Model.Chat chat)
    {
        return new Chat(chat.Id, chat.Name, chat.Participants.Select(p => new UserInfo(p.Id, p.FullName, p.AvatarUrl)));
    }
}