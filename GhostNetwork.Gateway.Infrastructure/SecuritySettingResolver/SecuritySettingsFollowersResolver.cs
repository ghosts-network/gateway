using Domain;
using GhostNetwork.Gateway.SecuritySettings;
using GhostNetwork.Gateway.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Infrastructure.SecuritySettingResolver
{
    public class SecuritySettingsFollowersResolver : ISecuritySettingsResolver
    {
        private const string followersAccessMessage = "You cannot see followers of this user";

        private readonly ISecuritySettingStorage securitySettingStorage;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IUsersRelationsStorage relationsStorage;

        public SecuritySettingsFollowersResolver(ISecuritySettingStorage securitySettingStorage,
                                        ICurrentUserProvider currentUserProvider,
                                        IUsersRelationsStorage relationsStorage)
        {
            this.securitySettingStorage = securitySettingStorage;
            this.currentUserProvider = currentUserProvider;
            this.relationsStorage = relationsStorage;
        }

        public async Task<DomainResult> ResolveAccessAsync(Guid userId)
        {
            if (userId == new Guid(currentUserProvider.UserId))
            {
                return DomainResult.Success();
            }

            var setting = await securitySettingStorage.FindByProfileAsync(userId);

            if (setting.Followers.Access == Access.NoOne)
            {
                return DomainResult.Error(followersAccessMessage);
            }
            
            if (setting.Followers.Access == Access.OnlyFriends)
            {
                if (!await relationsStorage.IsFriendAsync(new Guid(currentUserProvider.UserId), ofUserId: userId))
                {
                    return DomainResult.Error(followersAccessMessage);
                }
            }

            if (setting.Followers.Access == Access.OnlyCertainUsers)
            {
                if (!setting.Followers.CertainUsers.Contains(new Guid(currentUserProvider.UserId)))
                {
                    return DomainResult.Error(followersAccessMessage);
                }
            }

            if (setting.Followers.Access == Access.EveryoneExceptCertainUsers)
            {
                if (setting.Followers.CertainUsers.Contains(new Guid(currentUserProvider.UserId)))
                {
                    return DomainResult.Error(followersAccessMessage);
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