using System;
using System.Net;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Gateway.Events;
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

        public RestUsersStorage(
            IProfilesApi profilesApi, 
            IRelationsApi relationsApi, 
            IEventSender eventSender, 
            ICurrentUserProvider currentUserProvider)
        {
            this.profilesApi = profilesApi;
            this.eventSender = eventSender;
            this.currentUserProvider = currentUserProvider;

            Relations = new RestUserRelationsStorage(profilesApi, relationsApi);
        }

        public IUsersRelationsStorage Relations { get; }

        public async Task<User> GetByIdAsync(Guid id)
        {
            try
            {
                var profile = await profilesApi.GetByIdAsync(id);
                return new User(profile.Id, profile.FirstName, profile.LastName, profile.Gender, profile.DateOfBirth);
            }
            catch (Profiles.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<DomainResult> UpdateAsync(User user)
        {
            // var profile = await profilesApi.GetByIdAsync(user.Id);

            var updateCommand = new ProfileUpdateViewModel(
                "FNale",
                "LName",
                user.Gender,
                user.DateOfBirth,
                "Odessa");

            try
            {
                // await profilesApi.UpdateAsync(user.Id, updateCommand);
                await eventSender.PublishAsync(new ProfileChangedEvent { TriggeredBy = currentUserProvider.UserId, UpdatedUser = user });

                return DomainResult.Success();
            }
            catch (Profiles.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.BadRequest)
            {
                return DomainResult.Error("ERROR!!!!");
            }
        }
    }
}