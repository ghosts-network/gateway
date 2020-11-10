using System.Collections.Generic;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Facade;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Gateway.Api
{
    [Route("[controller]")]
    [ApiController]
    public class NewsFeedController : ControllerBase
    {
        private readonly INewsFeedManager newsFeedManager;

        public NewsFeedController(INewsFeedManager newsFeedManager)
        {
            this.newsFeedManager = newsFeedManager;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NewsFeedPublication>>> GetAsync()
        {
            return Ok(await newsFeedManager.FindManyAsync());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<NewsFeedPublication>> CreateAsync(
            [FromServices] ICurrentUserProvider currentUserProvider,
            [FromBody] CreateNewsFeedPublication model)
        {
            await newsFeedManager.CreateAsync(model.Content, currentUserProvider.UserId);

            return Ok();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateAsync(
            [FromRoute] string id,
            [FromBody] CreateNewsFeedPublication model)
        {
            await newsFeedManager.UpdateAsync(id, model.Content);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> DeleteAsync([FromRoute] string id)
        {
            await newsFeedManager.DeleteAsync(id);

            return Ok();
        }

        [HttpGet("{commentId}/comment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCommentById([FromRoute] string commentId)
        {
            return Ok(await newsFeedManager.GetCommentByIdAsync(commentId));
        }

        [HttpPost("{publicationId}/comment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> AddCommentAsync(
            [FromServices] ICurrentUserProvider currentUserProvider,
            [FromRoute] string publicationId,
            [FromBody] AddNewsFeedComment model)
        {
            await newsFeedManager.AddCommentAsync(publicationId, currentUserProvider.UserId, model.Content);

            return Ok();
        }

        [HttpPost("{publicationId}/reaction")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> AddReactionAsync(
            [FromServices] ICurrentUserProvider currentUserProvider,
            [FromRoute] string publicationId,
            [FromBody] AddNewsFeedReaction model)
        {
            await newsFeedManager.AddReactionAsync(publicationId, currentUserProvider.UserId, model.Reaction);

            return Ok();
        }

        [HttpDelete("{publicationId}/reaction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RemoveReactionAsync(
            [FromServices] ICurrentUserProvider currentUserProvider,
            [FromRoute] string publicationId)
        {
            await newsFeedManager.RemoveReactionAsync(publicationId, currentUserProvider.UserId);

            return Ok();
        }
    }
}
