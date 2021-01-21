using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Profiles.Api;
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

        public UsersController(IProfilesApi profilesApi)
        {
            this.profilesApi = profilesApi;
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<User>> SearchCommentsAsync(
            [FromRoute] Guid userId)
        {
            try
            {
                var profile = await profilesApi.GetByIdAsync(userId);
                return Ok(new User(profile.FirstName, profile.LastName, profile.Gender, profile.DateOfBirth));
            }
            catch (Profiles.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
            {
                return NotFound();
            }
        }
    }
}