using Domain;
using System;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.SecuritySettings;

public interface ISecuritySettingsResolver
{
    Task<DomainResult> ResolveFriendsAccessAsync(Guid userId);

    Task<DomainResult> ResolveFriendsAccessAsync(string userId);
}
