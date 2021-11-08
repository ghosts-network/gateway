using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpsertProfilePictureAsync(
            IFormFile file,
            [FromRoute] Guid userId,
            CancellationToken cancellationToken)
        {
            var connectionString =
                "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

            var client = new BlobServiceClient(connectionString);

            var blobContainer = client.GetBlobContainerClient("photos");
            await blobContainer.CreateIfNotExistsAsync(PublicAccessType.BlobContainer, cancellationToken: cancellationToken);

            
            var blob = blobContainer.GetBlobClient($"{userId.ToString()}/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
            await blob.UploadAsync(file.OpenReadStream(), cancellationToken);

            return Ok();
        }
    }
}