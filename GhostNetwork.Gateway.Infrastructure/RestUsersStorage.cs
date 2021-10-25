using System;
using System.Net;
using System.Threading.Tasks;
using Domain;
using GhostEventBus;
using GhostNetwork.Gateway.Users;
using GhostNetwork.Profiles.Api;
using GhostNetwork.Profiles.Model;

namespace GhostNetwork.Gateway.Infrastructure
{
    public class RestUsersStorage : IUsersStorage
    {
        private readonly IProfilesApi profilesApi;
        private readonly IEventSender eventSender;
        private readonly ICurrentUserProvider currentUserProvider;

        public RestUsersStorage(IProfilesApi profilesApi, ICurrentUserProvider currentUserProvider, IEventSender eventSender, IRelationsApi relationsApi)
        {
            this.profilesApi = profilesApi;
            this.currentUserProvider = currentUserProvider;
            this.eventSender = eventSender;

            Relations = new RestUserRelationsStorage(profilesApi, relationsApi);
        }

        public IUsersRelationsStorage Relations { get; }

        public async Task<User> GetByIdAsync(Guid id)
        {
            try
            {
                var profile = await profilesApi.GetByIdAsync(id);
                return new User(profile.Id, profile.FirstName, profile.LastName, profile.Gender, profile.DateOfBirth, profile.City);
            }
            catch (Profiles.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<DomainResult> UpdateAsync(User user)
        {
            var updateCommand = new ProfileUpdateViewModel(
                user.FirstName,
                user.LastName,
                user.Gender,
                user.DateOfBirth,
                user.City);

            try
            {
                await profilesApi.UpdateAsync(user.Id, updateCommand);
                await eventSender.PublishAsync(new ProfileChangedEvent 
                    { 
                        TriggeredBy = currentUserProvider.UserId, 
                        UpdatedUser = new UserInfo(user.Id, $"{user.FirstName} {user.LastName}", null)
                    });

                return DomainResult.Success();
            }
            catch (Profiles.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.BadRequest)
            {
                return DomainResult.Error("ERROR!!!!");
            }
        }
    }
}