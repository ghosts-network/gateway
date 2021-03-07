using System;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Profiles.Api;
using GhostNetwork.Profiles.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Gateway.Api.Users
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IProfilesApi profilesApi;
        private readonly ICurrentUserProvider currentUserProvider;

        public UsersController(IProfilesApi profilesApi, ICurrentUserProvider currentUserProvider)
        {
            this.profilesApi = profilesApi;
            this.currentUserProvider = currentUserProvider;
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetByIdAsync(
            [FromRoute] Guid userId)
        {
            try
            {
                var profile = await profilesApi.GetByIdAsync(userId);
                return Ok(new User(profile.Id, profile.FirstName, profile.LastName, profile.Gender, profile.DateOfBirth));
            }
            catch (Profiles.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
            {
                return NotFound();
            }
        }

        [HttpPut("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateAsync(
            [FromRoute] Guid userId,
            [FromBody] UpdateUserInput model)
        {
            if (userId.ToString() != currentUserProvider.UserId)
            {
                return Forbid();
            }

            try
            {
                var profile = await profilesApi.GetByIdAsync(userId);

                var updateCommand = new ProfileUpdateViewModel(
                    profile.FirstName,
                    profile.LastName,
                    model.Gender,
                    model.DateOfBirth,
                    profile.City);

                profilesApi.Update(userId, updateCommand);

                return NoContent();
            }
            catch (Profiles.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
            {
                return NotFound();
            }
        }
    }
}