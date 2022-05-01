using Domain;
using GhostNetwork.Gateway.SecuritySettings;
using GhostNetwork.Gateway.Users;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Infrastructure
{
    public class SecuritySettingsResolver : ISecuritySettingsResolver
    {
        private const string friendsAccessMessage = "You cannot see friends of this user";

        private readonly ISecuritySettingStorage securitySettingStorage;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IUsersRelationsStorage relationsStorage;

        public SecuritySettingsResolver(ISecuritySettingStorage securitySettingStorage,
                                        ICurrentUserProvider currentUserProvider,
                                        IUsersRelationsStorage relationsStorage)
        {
            this.securitySettingStorage = securitySettingStorage;
            this.currentUserProvider = currentUserProvider;
            this.relationsStorage = relationsStorage;
        }

        public async Task<DomainResult> ResolveFriendsAccessAsync(Guid userId)
        {
            if (userId == new Guid(currentUserProvider.UserId))
            {
                return DomainResult.Success();
            }

            var setting = await securitySettingStorage.FindByProfileAsync(userId);

            if (setting.Friends.Access == Access.NoOne)
            {
                return DomainResult.Error(friendsAccessMessage);
            }

            if (setting.Friends.Access == Access.OnlyFriends)
            {
                if (!await relationsStorage.IsFriendAsync(new Guid(currentUserProvider.UserId), ofUserId: userId))
                {
                    return DomainResult.Error(friendsAccessMessage);
                }
            }

            if (setting.Friends.Access == Access.OnlyCertainUsers)
            {
                if (!setting.Friends.CertainUsers.Contains(new Guid(currentUserProvider.UserId)))
                {
                    return DomainResult.Error(friendsAccessMessage);
                }
            }

            if (setting.Friends.Access == Access.EveryoneExceptCertainUsers)
            {
                if (setting.Friends.CertainUsers.Contains(new Guid(currentUserProvider.UserId)))
                {
                    return DomainResult.Error(friendsAccessMessage);
                }
            }

            return DomainResult.Success();
        }

        public Task<DomainResult> ResolveFriendsAccessAsync(string userId)
        {
            return ResolveFriendsAccessAsync(Guid.Parse(userId));
        }
    }
}