using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Infrastructure.SecuritySettingResolver;
using GhostNetwork.Gateway.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Gateway.Api.Users
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class RelationsController : ControllerBase
    {
        private readonly IUsersStorage usersStorage;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly SecuritySettingsFriendsResolver friendsAccessResolver;
        private readonly SecuritySettingsFollowersResolver followersAccessResolver;

        public RelationsController(IUsersStorage usersStorage,
            ICurrentUserProvider currentUserProvider,
            SecuritySettingsFriendsResolver friendsAccessResolver,
            SecuritySettingsFollowersResolver followersAccessResolver)
        {
            this.usersStorage = usersStorage;
            this.currentUserProvider = currentUserProvider;
            this.friendsAccessResolver = friendsAccessResolver;
            this.followersAccessResolver = followersAccessResolver;
        }

        [HttpGet("{userId:guid}/friends")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<User>>> GetFriendsAsync(
            [FromRoute] Guid userId,
            [FromQuery, Range(0, int.MaxValue)] int skip = 0,
            [FromQuery, Range(1, 50)] int take = 20)
        {
            var resolveResult = await friendsAccessResolver.ResolveAccessAsync(userId);
            if (!resolveResult.Successed)
            {
                return Forbid();
            }

            var friends = await usersStorage.Relations.GetFriendsAsync(userId, take, skip);
            return Ok(friends);
        }

        [HttpGet("{userId:guid}/followers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<User>>> GetFollowersAsync(
            [FromRoute] Guid userId,
            [FromQuery, Range(0, int.MaxValue)] int skip = 0,
            [FromQuery, Range(1, 50)] int take = 20)
        {
            var resolveResult = await followersAccessResolver.ResolveAccessAsync(userId);
            if (!resolveResult.Successed)
            {
                return Forbid();
            }

            var followers = await usersStorage.Relations.GetFollowersAsync(userId, take, skip);
            return Ok(followers);
        }

        [HttpGet("friends/incoming-requests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetIncomingRequestsAsync(
            [FromQuery, Range(0, int.MaxValue)] int skip = 0,
            [FromQuery, Range(1, 50)] int take = 20)
        {
            var friends = await usersStorage.Relations.GetIncomingFriendRequestsAsync(Guid.Parse(currentUserProvider.UserId), take, skip);

            return Ok(friends);
        }

        [HttpGet("friends/outgoing-requests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetOutgoingRequestsAsync(
            [FromQuery, Range(0, int.MaxValue)] int skip = 0,
            [FromQuery, Range(1, 50)] int take = 20)
        {
            var friends = await usersStorage.Relations.GetOutgoingFriendRequestsAsync(Guid.Parse(currentUserProvider.UserId), take, skip);

            return Ok(friends);
        }

        [HttpPost("friends/{toUser:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> SendFriendRequestAsync([FromRoute] Guid toUser)
        {
            await usersStorage.Relations.SendFriendRequestAsync(Guid.Parse(currentUserProvider.UserId), toUser);

            return NoContent();
        }

        [HttpPut("friends/{requester:guid}/approve")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> ApproveFriendRequestAsync([FromRoute] Guid requester)
        {
            await usersStorage.Relations.ApproveFriendRequestAsync(Guid.Parse(currentUserProvider.UserId), requester);

            return NoContent();
        }

        [HttpPost("friends/{requester:guid}/decline")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeclineFriendRequestAsync([FromRoute] Guid requester)
        {
            await usersStorage.Relations.DeclineFriendRequestAsync(Guid.Parse(currentUserProvider.UserId), requester);

            return NoContent();
        }

        [HttpDelete("outgoing/{to:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CancelOutgoingRequestAsync([FromRoute] Guid to)
        {
            if (currentUserProvider.UserId == to.ToString())
            {
                return BadRequest();
            }

            await usersStorage.Relations.RemoveOutgoingRequestAsync(new Guid(currentUserProvider.UserId), to);

            return NoContent();
        }

        [HttpDelete("friends/{friend:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> RemoveFriendAsync([FromRoute] Guid friend)
        {
            await usersStorage.Relations.RemoveFriendAsync(Guid.Parse(currentUserProvider.UserId), friend);

            return NoContent();
        }
    }
}