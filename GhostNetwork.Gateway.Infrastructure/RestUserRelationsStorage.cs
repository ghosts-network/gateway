using Domain;
using GhostNetwork.Gateway.SecuritySettings;
using GhostNetwork.Gateway.Users;
using GhostNetwork.Profiles.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Infrastructure
{
    public class RestUserRelationsStorage : IUsersRelationsStorage
    {
        private readonly IProfilesApi profilesApi;
        private readonly IRelationsApi relationsApi;
        private readonly ISecuritySettingsResolver securitySettingsResolver;

        public RestUserRelationsStorage(IProfilesApi profilesApi, IRelationsApi relationsApi, ISecuritySettingsResolver securitySettingsResolver)
        {
            this.profilesApi = profilesApi;
            this.relationsApi = relationsApi;
            this.securitySettingsResolver = securitySettingsResolver;
        }

        public async Task<IEnumerable<UserInfo>> GetFriendsAsync(Guid user, int take, int skip)
        {
            var resolveResult = await securitySettingsResolver.ResolveFriendsAccessAsync(user);

            if (!resolveResult.Successed)
            {
                return Enumerable.Empty<UserInfo>();
            }

            var ids = await relationsApi.SearchFriendsAsync(user, skip, take);
            return await GetProfilesByIdsAsync(ids);
        }

        public async Task<IEnumerable<UserInfo>> GetFollowersAsync(Guid user, int take, int skip)
        {
            var ids = await relationsApi.SearchFollowersAsync(user, skip, take);
            return await GetProfilesByIdsAsync(ids);
        }

        public Task<bool> IsFriendAsync(Guid userId, Guid ofUserId)
        {
            return relationsApi.IsFriendAsync(userId, ofUserId);
        }

        public async Task<IEnumerable<UserInfo>> GetIncomingFriendRequestsAsync(Guid user, int take, int skip)
        {
            var ids = await relationsApi.SearchIncomingFriendsRequestsAsync(user, skip, take);
            return await GetProfilesByIdsAsync(ids);
        }

        public async Task<IEnumerable<UserInfo>> GetOutgoingFriendRequestsAsync(Guid user, int take, int skip)
        {
            var ids = await relationsApi.SearchOutgoingFriendsRequestsAsync(user, skip, take);
            return await GetProfilesByIdsAsync(ids);
        }

        public async Task<DomainResult> SendFriendRequestAsync(Guid fromUser, Guid toUser)
        {
            await relationsApi.SendFriendRequestAsync(fromUser, toUser);

            return DomainResult.Success();
        }

        public async Task<DomainResult> ApproveFriendRequestAsync(Guid user, Guid requester)
        {
            await relationsApi.ApproveFriendRequestAsync(user, requester);

            return DomainResult.Success();
        }

        public async Task<DomainResult> DeclineFriendRequestAsync(Guid user, Guid requester)
        {
            await relationsApi.DeclineFriendRequestAsync(user, requester);

            return DomainResult.Success();
        }

        public async Task<DomainResult> RemoveOutgoingRequestAsync(Guid from, Guid to)
        {
            await relationsApi.CancelOutgoingRequestAsync(from, to);

            return DomainResult.Success();
        }

        public async Task<DomainResult> RemoveFriendAsync(Guid user, Guid friend)
        {
            await relationsApi.DeleteFriendRequestAsync(user, friend);

            return DomainResult.Success();
        }

        private async Task<IEnumerable<UserInfo>> GetProfilesByIdsAsync(IEnumerable<Guid> ids)
        {
            var friends = new List<UserInfo>(ids.Count());
            foreach (var id in ids)
            {
                var friend = await profilesApi.GetByIdAsync(id);
                friends.Add(new UserInfo(friend.Id, $"{friend.FirstName} {friend.LastName}", string.Empty));
            }

            return friends;
        }
    }
}