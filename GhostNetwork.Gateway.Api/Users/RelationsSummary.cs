#nullable enable
using System.Collections.Generic;

namespace GhostNetwork.Gateway.Api.Users;

public record RelationsSummary(
    IEnumerable<UserInfo> Friends,
    IEnumerable<UserInfo> Followers,
    IEnumerable<UserInfo>? OutgoingRequests,
    IEnumerable<UserInfo>? IncomingRequests,
    RelationsActions? Actions);

public record RelationsActions(
    bool AddToFriends,
    bool RemoveFromFriends,
    bool ReactIncomingRequest,
    bool CancelOutgoingRequest)
{
    public static RelationsActions NoAvailable { get; } = new RelationsActions(false, false, false, false);
}
