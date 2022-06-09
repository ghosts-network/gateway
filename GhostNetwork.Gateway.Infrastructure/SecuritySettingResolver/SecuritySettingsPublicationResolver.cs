using Domain;
using GhostNetwork.Gateway.SecuritySettings;
using GhostNetwork.Gateway.Users;
using GhostNetwork.Profiles.Api;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Infrastructure.SecuritySettingResolver
{
    public class SecuritySettingsPublicationResolver : ISecuritySettingsResolver
    {
        private const string publicationAccessMessage = "You cannot see publications of this user";

        private readonly ISecuritySettingStorage securitySettingStorage;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IRelationsApi relationsApi;

        public SecuritySettingsPublicationResolver(ISecuritySettingStorage securitySettingStorage,
                                        ICurrentUserProvider currentUserProvider,
                                        IRelationsApi relationsApi)
        {
            this.securitySettingStorage = securitySettingStorage;
            this.currentUserProvider = currentUserProvider;
            this.relationsApi = relationsApi;
        }

        public async Task<DomainResult> ResolveAccessAsync(Guid userId)
        {
            if (userId == new Guid(currentUserProvider.UserId))
            {
                return DomainResult.Success();
            }

            var setting = await securitySettingStorage.FindByProfileAsync(userId);

            if (setting.Posts.Access == AccessLevel.NoOne)
            {
                return DomainResult.Error(publicationAccessMessage);
            }

            if (setting.Posts.Access == AccessLevel.OnlyFriends)
            {
                if (!await relationsApi.IsFriendAsync(new Guid(currentUserProvider.UserId), friend: userId))
                {
                    return DomainResult.Error(publicationAccessMessage);
                }
            }

            if (setting.Posts.Access == AccessLevel.OnlyCertainUsers)
            {
                if (!setting.Posts.CertainUsers.Contains(new Guid(currentUserProvider.UserId)))
                {
                    return DomainResult.Error(publicationAccessMessage);
                }
            }

            if (setting.Posts.Access == AccessLevel.EveryoneExceptCertainUsers)
            {
                if (setting.Posts.CertainUsers.Contains(new Guid(currentUserProvider.UserId)))
                {
                    return DomainResult.Error(publicationAccessMessage);
                }
            }

            return DomainResult.Success();
        }

        public Task<DomainResult> ResolveAccessAsync(string userId)
        {
            return ResolveAccessAsync(Guid.Parse(userId));
        }
    }
}
