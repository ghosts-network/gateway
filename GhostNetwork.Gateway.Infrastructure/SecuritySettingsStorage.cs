﻿using Domain;
using GhostNetwork.Gateway.Users;
using GhostNetwork.Gateway.Users.SecuritySection;
using GhostNetwork.Profiles.Api;
using GhostNetwork.Profiles.Client;
using GhostNetwork.Profiles.Model;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Infrastructure
{
    public class SecuritySettingsStorage : ISecuritySettingStorage
    {
        private readonly ISecuritySettingsApi securitySettingsApi;

        public SecuritySettingsStorage(ISecuritySettingsApi securitySettingsApi)
        {
            this.securitySettingsApi = securitySettingsApi;
        }

        public async Task<SecuritySettingModel> FindByProfileAsync(Guid userId)
        {
            var setting = await securitySettingsApi.FindByProfileAsync(userId);

            if (setting == null)
            {
                return SecuritySettingModel.DefaultForUser(userId);
            }

            return new SecuritySettingModel(
                setting.UserId,
                new SecuritySettingSection((AccessLevel)setting.Friends.Access, setting.Friends.CertainUsers),
                new SecuritySettingSection((AccessLevel)setting.Followers.Access, setting.Followers.CertainUsers),
                new SecuritySettingSection((AccessLevel)setting.Posts.Access, setting.Posts.CertainUsers));
        }

        public async Task<bool> CheckAccessAsync(Guid userId, Guid toUserId, string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                throw new ArgumentException(nameof(sectionName));
            }

            try
            {
                await securitySettingsApi.CheckAccessWithHttpInfoAsync(userId, new SecuritySettingResolvingInputModel
                {
                    ToUserId = toUserId,
                    SectionName = sectionName,
                });
            }
            catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.Forbidden)
            {
                return false;
            }

            return true;
        }

        public async Task<DomainResult> UpdateAsync(Guid userId, SecuritySettingUpdateModel model)
        {
            try
            {
                await securitySettingsApi.UpdateAsync(userId, new SecuritySettingUpdateViewModel(
                    friends: new Profiles.Model.SecuritySettingsSectionInputModel((Access)model.Friends.Access, model.Friends.CertainUsers.ToList()),
                    followers: new Profiles.Model.SecuritySettingsSectionInputModel((Access)model.Followers.Access, model.Followers.CertainUsers.ToList()),
                    posts: new Profiles.Model.SecuritySettingsSectionInputModel((Access)model.Posts.Access, model.Posts.CertainUsers.ToList())));

                return DomainResult.Success();
            }
            catch (ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.BadRequest)
            {
                return DomainResult.Error(ex.Message);
            }
        }
    }
}
