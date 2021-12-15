using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Users;
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
        private readonly IUsersStorage usersStorage;
        private readonly ICurrentUserProvider currentUserProvider;

        public UsersController(IUsersStorage usersStorage, ICurrentUserProvider currentUserProvider)
        {
            this.usersStorage = usersStorage;
            this.currentUserProvider = currentUserProvider;
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetByIdAsync(
            [FromRoute] Guid userId)
        {
            var user = await usersStorage.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
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

            var user = await usersStorage.GetByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.Update(model.Gender, model.DateOfBirth);

            var result = await usersStorage.UpdateAsync(user);
            if (result.Successed)
            {
                return NoContent();
            }

            return BadRequest();
        }

        [HttpPut("{userId}/profile-picture")]
        [RequestSizeLimit(1048576)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpsertProfilePictureAsync(
            IFormFile file,
            [FromRoute] Guid userId,
            CancellationToken cancellationToken)
        {
            if (userId.ToString() != currentUserProvider.UserId)
            {
                return Forbid();
            }

            if (Path.GetExtension(file.FileName) != ".jpg" && Path.GetExtension(file.FileName) != ".png")
            {
                return BadRequest();
            }

            await usersStorage.ProfilePictures
                .UploadAsync(
                    userId,
                    $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}",
                    file.OpenReadStream(),
                    cancellationToken);

            return NoContent();
        }
    }
}