using Domain;
using GhostNetwork.Gateway.Users.SecuritySection;
using System;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Users
{
    public interface ISecuritySettingStorage
    {
        Task<SecuritySettingModel> FindByProfileAsync(Guid userId);

        Task<bool> CheckAccessAsync(Guid userId, Guid toUserId, string sectionName);

        Task<DomainResult> UpdateAsync(Guid userId, SecuritySettingUpdateModel model);
    }
}
