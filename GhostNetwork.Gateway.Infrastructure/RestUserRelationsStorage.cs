using GhostNetwork.Gateway.Users;
using GhostNetwork.Profiles.Api;

namespace GhostNetwork.Gateway.Infrastructure
{
    public class RestUserRelationsStorage : IUsersRelationsStorage
    {
        private readonly IProfilesApi profilesApi;
        private readonly IRelationsApi relationsApi;

        public RestUserRelationsStorage(IProfilesApi profilesApi, IRelationsApi relationsApi)
        {
            this.profilesApi = profilesApi;
            this.relationsApi = relationsApi;
        }
    }
}