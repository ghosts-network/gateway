using Domain;
using GhostNetwork.Gateway.SecuritySettings;
using GhostNetwork.Gateway.Users;
using System;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Infrastructure.SecuritySettingResolver
{
    public class SecuritySettingsPublicationResolver : ISecuritySettingsResolver
    {
        private const string sectionName = "posts";
        private const string publicationAccessMessage = "You cannot see publications of this user";

        private readonly ISecuritySettingStorage securitySettingStorage;
        private readonly ICurrentUserProvider currentUserProvider;

        public SecuritySettingsPublicationResolver(ISecuritySettingStorage securitySettingStorage,
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

            if (!await securitySettingStorage.CheckAccessAsync(new Guid(currentUserProvider.UserId), userId, sectionName))
            {
                return DomainResult.Error(publicationAccessMessage);
            }

            return DomainResult.Success();
        }

        public Task<DomainResult> ResolveAccessAsync(string userId)
        {
            return ResolveAccessAsync(Guid.Parse(userId));
        }
    }
}
