using Domain;
using GhostNetwork.Gateway.SecuritySettings;
using GhostNetwork.Gateway.Users;
using System;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Infrastructure.SecuritySettingResolver
{
    public class SecuritySettingsFriendsResolver : ISecuritySettingsResolver
    {
        private const string SectionName = "friends";
        private const string FriendsAccessMessage = "You cannot see friends of this user";

        private readonly ISecuritySettingStorage securitySettingStorage;
        private readonly ICurrentUserProvider currentUserProvider;

        public SecuritySettingsFriendsResolver(
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
                return DomainResult.Error(FriendsAccessMessage);
            }

            return DomainResult.Success();
        }
    }
}