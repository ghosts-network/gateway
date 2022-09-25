using GhostNetwork.Gateway.GraphQL.Models;
using GhostNetwork.Profiles.Api;
using GhostNetwork.Profiles.Model;

namespace GhostNetwork.Gateway.GraphQL.DataLoaders
{
    public class UserByIdDataLoader : BatchDataLoader<string, UserInfoEntity>
    {
        private readonly IProfilesApi profilesApi;

        public UserByIdDataLoader(
            IProfilesApi profilesApi,
            IBatchScheduler batchScheduler,
            DataLoaderOptions? options = null) : base(batchScheduler, options)
        {
            this.profilesApi = profilesApi;
        }

        protected override async Task<IReadOnlyDictionary<string, UserInfoEntity>> LoadBatchAsync(IReadOnlyList<string> keys, CancellationToken cancellationToken)
        {
            var profiles = await profilesApi.SearchByIdsAsync(new ProfilesQueryModel(keys.Select(k => new Guid(k)).ToList()), cancellationToken: cancellationToken);

            return profiles.Select(p => new UserInfoEntity
            {
                Id = p.Id.ToString(),
                FirstName = p.FirstName,
                LastName = p.LastName,
                AvatarUrl = p.ProfilePicture
            }).ToDictionary(k => k.Id, v => v);
        }
    }
}
