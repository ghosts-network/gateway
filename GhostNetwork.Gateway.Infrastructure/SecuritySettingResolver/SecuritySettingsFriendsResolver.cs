using Domain;
using GhostNetwork.Gateway.SecuritySettings;
using GhostNetwork.Gateway.Users;
using GhostNetwork.Profiles.Api;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Infrastructure.SecuritySettingResolver
{
    public class SecuritySettingsFriendsResolver : ISecuritySettingsResolver
    {
        private const string friendsAccessMessage = "You cannot see friends of this user";

        private readonly ISecuritySettingStorage securitySettingStorage;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IRelationsApi relationsApi;

        public SecuritySettingsFriendsResolver(ISecuritySettingStorage securitySettingStorage,
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

            if (setting.Friends.Access == Access.NoOne)
            {
                return DomainResult.Error(friendsAccessMessage);
            }

            if (setting.Friends.Access == Access.OnlyFriends)
            {
                if (!await relationsApi.IsFriendAsync(new Guid(currentUserProvider.UserId), friend: userId))
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

        public Task<DomainResult> ResolveAccessAsync(string userId)
        {
            return ResolveAccessAsync(Guid.Parse(userId));
        }
    }
}