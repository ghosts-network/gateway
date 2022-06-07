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

        public async Task<SecuritySettingModel?> FindByProfileAsync(Guid userId)
        {
            var setting = await securitySettingsApi.FindByProfileAsync(userId);

            if (setting == null)
            {
                return null;
            }

            return new SecuritySettingModel(setting.UserId,
                new SecuritySettingSection((AccessLevel)setting.Friends.Access, setting.Friends.CertainUsers),
                new SecuritySettingSection((AccessLevel)setting.Followers.Access, setting.Followers.CertainUsers),
                new SecuritySettingSection((AccessLevel)setting.Posts.Access, setting.Posts.CertainUsers),
                new SecuritySettingSection((AccessLevel)setting.Comments.Access, setting.Comments.CertainUsers),
                new SecuritySettingSection((AccessLevel)setting.ProfilePhoto.Access, setting.ProfilePhoto.CertainUsers));
        }

        public async Task<DomainResult> UpdateAsync(Guid userId, SecuritySettingUpdateViewModel model)
        {
            try
            {
                await securitySettingsApi.UpdateAsync(userId, model);
                return DomainResult.Success();
            }
            catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.BadRequest)
            {
                return DomainResult.Error(ex.Message);
            }
        }
    }
}
