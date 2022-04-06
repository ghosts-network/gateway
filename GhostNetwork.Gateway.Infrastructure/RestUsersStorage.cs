using System;
using System.Net;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Gateway.Users;
using GhostNetwork.Profiles.Api;
using GhostNetwork.Profiles.Model;

namespace GhostNetwork.Gateway.Infrastructure
{
    public class RestUsersStorage : IUsersStorage
    {
        private readonly IProfilesApi profilesApi;

        public RestUsersStorage(
            IProfilesApi profilesApi,
            IRelationsApi relationsApi,
            IUsersPictureStorage usersPictureStorage)
        {
            this.profilesApi = profilesApi;
            Relations = new RestUserRelationsStorage(profilesApi, relationsApi);
            ProfilePictures = usersPictureStorage;
        }

        public IUsersRelationsStorage Relations { get; }

        public IUsersPictureStorage ProfilePictures { get; }

        public async Task<User> GetByIdAsync(Guid id)
        {
            try
            {
                var profile = await profilesApi.GetByIdAsync(id);
                return new User(profile.Id, profile.FirstName, profile.LastName, profile.Gender, profile.DateOfBirth, profile.City, profile.ProfilePicture);
            }
            catch (Profiles.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<DomainResult> UpdateAsync(User user)
        {
            var profile = await profilesApi.GetByIdAsync(user.Id);

            var updateCommand = new ProfileUpdateViewModel(
                profile.FirstName,
                profile.LastName,
                user.Gender,
                user.DateOfBirth,
                city: user.City);

            try
            {
                await profilesApi.UpdateAsync(user.Id, updateCommand);
                return DomainResult.Success();
            }
            catch (Profiles.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.BadRequest)
            {
                return DomainResult.Error("ERROR!!!!");
            }
        }
    }
}