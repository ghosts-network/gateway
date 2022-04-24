using System;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Gateway.SecuritySettings;
using GhostNetwork.Profiles.Api;
using GhostNetwork.Profiles.Model;

namespace GhostNetwork.Gateway.Infrastructure
{
    public class SecuritySettingsResolver : ISecuritySettingsResolver
    {
        private const string friendsAccessMessage = "You cannot see friends of this user";

        private readonly ISecuritySettingsApi securitySettingsApi;
        private readonly IRelationsApi relationsApi;
        private readonly ICurrentUserProvider currentUserProvider;

        public SecuritySettingsResolver(ISecuritySettingsApi securitySettingsApi, ICurrentUserProvider currentUserProvider)
        {
            this.securitySettingsApi = securitySettingsApi;
            this.currentUserProvider = currentUserProvider;
        }

        public async Task<DomainResult> ResolveFriendsAccessAsync(Guid userId)
        {
            if (userId == new Guid(currentUserProvider.UserId))
            {
                return DomainResult.Success();
            }

            var setting = await securitySettingsApi.FindByProfileAsync(userId);

            if (setting.Friends.Access == Access.NoOne)
            {
                return DomainResult.Error(friendsAccessMessage);
            }

            if (setting.Friends.Access == Access.OnlyFriends)
            {
                var friendsIds = await relationsApi.SearchFriendsAsync(userId);

                if (!friendsIds.Contains(new Guid(currentUserProvider.UserId)))
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
            return ResolveFriendsAccessAsync(new Guid(userId));
        }
    }
}
