using Domain;
using GhostNetwork.Gateway.Users;
using System;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Infrastructure.SecuritySettingResolver
{
    public class SecuritySettingsFollowersResolver
    {
        private const string SectionName = "followers";
        private const string FollowersAccessMessage = "You cannot see followers of this user";

        private readonly ISecuritySettingStorage securitySettingStorage;
        private readonly ICurrentUserProvider currentUserProvider;

        public SecuritySettingsFollowersResolver(
            ISecuritySettingStorage securitySettingStorage,
            ICurrentUserProvider currentUserProvider)
        {
            this.securitySettingStorage = securitySettingStorage;
            this.currentUserProvider = currentUserProvider;
        }

        public async Task<DomainResult> ResolveAccessAsync(Guid userId)
        {
            if (userId == new Guid(currentUserProvider.UserId))
            {
                return DomainResult.Success();
            }

            if (!await securitySettingStorage.CheckAccessAsync(new Guid(currentUserProvider.UserId), userId, SectionName))
            {
                return DomainResult.Error(FollowersAccessMessage);
            }

            return DomainResult.Success();
        }
    }
}