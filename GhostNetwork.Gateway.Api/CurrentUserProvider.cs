using System;
using System.Threading.Tasks;
using GhostNetwork.Profiles.Api;
using Microsoft.AspNetCore.Http;

namespace GhostNetwork.Gateway.Api
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly IProfilesApi profilesApi;

        public CurrentUserProvider(IHttpContextAccessor httpContext, IProfilesApi profilesApi)
        {
            this.httpContext = httpContext;
            this.profilesApi = profilesApi;
        }

        public string UserId => httpContext.HttpContext?.User.FindFirst(s => s.Type == "sub")?.Value;

        public async Task<UserInfo> GetProfileAsync()
        {
            if (UserId == null)
            {
                return null;
            }

            var profile = await profilesApi.GetByIdAsync(Guid.Parse(UserId));

            return new UserInfo(profile.Id, $"{profile.FirstName} {profile.LastName}", profile.ProfilePicture);
        }
    }
}