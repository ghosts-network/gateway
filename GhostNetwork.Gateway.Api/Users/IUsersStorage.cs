using System;
using System.Threading.Tasks;
using Domain;

namespace GhostNetwork.Gateway.Api.Users
{
    public interface IUsersStorage
    {
        Task<User> GetByIdAsync(Guid id);

        Task<DomainResult> UpdateAsync(User user);
    }
}