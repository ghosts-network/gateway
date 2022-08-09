using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Gateway.Chats;

namespace GhostNetwork.Gateway.Messages;

public interface IChatStorage
{
    Task<(IEnumerable<Chat>, string)> GetAsync(string userId, string cursor, int limit);

    Task<Chat> GetByIdAsync(string id);

    Task<Chat> CreateAsync(string name, IEnumerable<Guid> participants);

    Task<DomainResult> UpdateAsync(string id, string name, IEnumerable<Guid> participants);

    Task DeleteAsync(string id);
}