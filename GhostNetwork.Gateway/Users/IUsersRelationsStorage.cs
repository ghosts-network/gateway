using System;
using System.Threading.Tasks;
using Domain;

namespace GhostNetwork.Gateway.Users
{
    public interface IUsersRelationsStorage
    {
        Task<UserInfo> GetFriendsAsync(Guid user, int take, int skip);
        Task<UserInfo> GetFollowersAsync(Guid user, int take, int skip);
        Task<UserInfo> GetIncomingFriendRequestsAsync(Guid user, int take, int skip);
        Task<UserInfo> GetOutgoingFriendRequestsAsync(Guid user, int take, int skip);

        Task<DomainResult> SendFriendRequestAsync(Guid toUser);
        Task<DomainResult> ApproveFriendRequestAsync(Guid fromUser);
        Task<DomainResult> DeclineFriendRequestAsync(Guid fromUser);
        Task<DomainResult> RemoveFriendAsync(Guid friend);
    }
}