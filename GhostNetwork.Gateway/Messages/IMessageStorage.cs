using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Messages;

public interface IMessageStorage
{
    public Task<IEnumerable<Message>> SearchAsync(string chatId, string cursor, int limit);

    public Task<Message> GetByIdAsync(string chatId, string id);

    public Task<Message> CreateAsync(string chatId, Guid authorId, string content);

    public Task UpdateAsync(string chatId, string messageId, string content);

    public Task DeleteAsync(string chatId, string messageId);
}