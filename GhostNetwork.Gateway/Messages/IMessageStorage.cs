using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace GhostNetwork.Gateway.Messages;

public interface IMessageStorage
{
    public Task<(IEnumerable<Message>, string)> SearchAsync(string chatId, string cursor, int limit);

    public Task<Message> GetByIdAsync(string chatId, string id);

    public Task<Message> CreateAsync(string chatId, Guid authorId, string content);

    public Task<DomainResult> UpdateAsync(string chatId, string messageId, string content);

    public Task DeleteAsync(string chatId, string messageId);
}