using Domain;
using GhostNetwork.Profiles.Model;
using System;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Users
{
    public interface ISecuritySettingStorage
    {
        Task<SecuritySettingModel> FindByProfileAsync(Guid userId);

        Task<DomainResult> UpdateAsync(Guid userId, SecuritySettingUpdateViewModel model);
    }
}
