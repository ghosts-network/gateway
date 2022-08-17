using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace GhostNetwork.Gateway.Chats;

public interface IChatStorage
{
    Task<(IEnumerable<Chat>, string)> GetAsync(string userId, string cursor, int limit);

    Task<Chat> GetByIdAsync(string id);

    Task<Chat> CreateAsync(string name, List<Guid> participants);

    Task<DomainResult> UpdateAsync(string id, string name, List<Guid> participants);

    Task DeleteAsync(string id);
}