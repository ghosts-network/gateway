using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

namespace GhostNetwork.Gateway.Users
{
    public interface IUsersRelationsStorage
    {
        Task<IEnumerable<UserInfo>> GetFriendsAsync(Guid user, int take, int skip);

        Task<IEnumerable<UserInfo>> GetFollowersAsync(Guid user, int take, int skip);

        Task<IEnumerable<UserInfo>> GetIncomingFriendRequestsAsync(Guid user, int take, int skip);

        Task<IEnumerable<UserInfo>> GetOutgoingFriendRequestsAsync(Guid user, int take, int skip);

        Task<RelationType> RelationTypeAsync(Guid fromUser, Guid toUser);

        Task<DomainResult> SendFriendRequestAsync(Guid fromUser, Guid toUser);

        Task<DomainResult> ApproveFriendRequestAsync(Guid user, Guid requester);

        Task<DomainResult> DeclineFriendRequestAsync(Guid user, Guid requester);

        Task<DomainResult> RemoveOutgoingRequestAsync(Guid from, Guid to);

        Task<DomainResult> RemoveFriendAsync(Guid user, Guid friend);
    }
}