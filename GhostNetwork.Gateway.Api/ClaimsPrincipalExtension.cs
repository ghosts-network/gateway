#nullable enable
using System.Security.Claims;

namespace GhostNetwork.Gateway.Api;

public static class ClaimsPrincipalExtension
{
    public static string? UserId(this ClaimsPrincipal user)
    {
        return user.FindFirst(s => s.Type == "sub")?.Value;
    }
}
