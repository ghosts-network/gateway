using System;
using System.Threading.Tasks;
using Domain;

namespace GhostNetwork.Gateway.Users
{
    public interface IUsersStorage
    {
        IUsersRelationsStorage Relations { get; }

        IUsersPictureStorage ProfilePictures { get; }

        ISecuritySettingStorage SecuritySettings { get; }

        Task<User> GetByIdAsync(Guid id);

        Task<DomainResult> UpdateAsync(User user);
    }
}