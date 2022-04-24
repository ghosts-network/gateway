using GhostNetwork.Gateway.Users;
using GhostNetwork.Profiles.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Api.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecuritySettingsController : ControllerBase
    {
        private readonly IUsersStorage usersStorage;

        public SecuritySettingsController(IUsersStorage usersStorage)
        {
            this.usersStorage = usersStorage;
        }

        [HttpGet("{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FindByProfileAsync([FromRoute] Guid userId)
        {
            var settings = await usersStorage.SecuritySettings.FindByProfileAsync(userId);
            if (settings == null)
            {
                return NotFound();
            }

            return Ok(settings);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid userId, [FromBody] SecuritySettingUpdateViewModel model)
        {
            var result = await usersStorage.SecuritySettings.UpdateAsync(userId, model);
            if (!result.Successed)
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}
