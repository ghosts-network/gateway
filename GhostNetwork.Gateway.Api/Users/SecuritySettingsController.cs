﻿using GhostNetwork.Gateway.Users;
using GhostNetwork.Profiles.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Api.Users
{
    [Route("[controller]")]
    [ApiController]
    public class SecuritySettingsController : ControllerBase
    {
        private readonly IUsersStorage usersStorage;
        private readonly ICurrentUserProvider currentUserProvider;

        public SecuritySettingsController(IUsersStorage usersStorage, ICurrentUserProvider currentUserProvider)
        {
            this.usersStorage = usersStorage;
            this.currentUserProvider = currentUserProvider;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserSecuritySettings()
        {
            var settings = await usersStorage.SecuritySettings.FindByProfileAsync(new Guid(currentUserProvider.UserId));
            if (settings == null)
            {
                return NotFound();
            }

            return Ok(settings);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAsync([FromBody] SecuritySettingUpdateViewModel model)
        {
            var result = await usersStorage.SecuritySettings.UpdateAsync(new Guid(currentUserProvider.UserId), model);
            if (!result.Successed)
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}