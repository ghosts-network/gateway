using Domain;
using GhostNetwork.Gateway.Users;
using GhostNetwork.Profiles.Api;
using GhostNetwork.Profiles.Client;
using GhostNetwork.Profiles.Model;
using System;
using System.Net;
using System.Threading.Tasks;
using Access = GhostNetwork.Gateway.Users.Access;

namespace GhostNetwork.Gateway.Infrastructure
{
    public class SecuritySettingStorage : ISecuritySettingStorage
    {
        private readonly ISecuritySettingsApi securitySettingsApi;

        public SecuritySettingStorage(ISecuritySettingsApi securitySettingsApi)
        {
            this.securitySettingsApi = securitySettingsApi;
        }

        public async Task<Users.SecuritySetting?> FindByProfileAsync(Guid userId)
        {
            var setting = await securitySettingsApi.FindByProfileAsync(userId);

            if (setting == null)
            {
                return null;
            }

            return new Users.SecuritySetting(setting.UserId,
                new SecuritySettingSection((Access)setting.Posts.Access, setting.Posts.CertainUsers),
                new SecuritySettingSection((Access)setting.Posts.Access, setting.Posts.CertainUsers));
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
