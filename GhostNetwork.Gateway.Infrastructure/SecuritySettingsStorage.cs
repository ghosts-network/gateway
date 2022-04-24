using Domain;
using GhostNetwork.Gateway.Users;
using GhostNetwork.Profiles.Api;
using GhostNetwork.Profiles.Client;
using GhostNetwork.Profiles.Model;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Infrastructure
{
    public class SecuritySettingStorage : ISecuritySettingStorage
    {
        private readonly ISecuritySettingsApi securitySettingsApi;

        public SecuritySettingStorage(ISecuritySettingsApi securitySettingsApi)
        {
            this.securitySettingsApi = securitySettingsApi;
        }

        public Task<SecuritySetting?> FindByProfileAsync(Guid userId)
        {
            return securitySettingsApi.FindByProfileAsync(userId);
        }

        public async Task<DomainResult> UpdateAsync(Guid userId, SecuritySettingUpdateViewModel model)
        {
            try
            {
                await securitySettingsApi.UpdateAsync(userId, model);
                return DomainResult.Success();
            }
            catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
            {
                return DomainResult.Error(ex.Message);
            }
        }
    }
}
