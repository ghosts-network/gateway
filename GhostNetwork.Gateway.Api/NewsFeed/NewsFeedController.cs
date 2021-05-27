using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Content.Client;
using GhostNetwork.Gateway.Facade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Gateway.Api.NewsFeed
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class NewsFeedController : ControllerBase
    {
        private readonly INewsFeedStorage newsFeedStorage;
        private readonly CurrentUserProvider currentUserProvider;

        public NewsFeedController(INewsFeedStorage newsFeedStorage, CurrentUserProvider currentUserProvider)
        {
            this.newsFeedStorage = newsFeedStorage;
            this.currentUserProvider = currentUserProvider;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, Consts.Headers.TotalCount, "number", "")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, Consts.Headers.HasMore, "boolean", "")]
        public async Task<ActionResult<IEnumerable<NewsFeedPublication>>> GetAsync(
            [FromQuery, Range(0, int.MaxValue)] int skip = 0,
            [FromQuery, Range(1, 50)] int take = 20)
        {
            var (news, totalCount) = await newsFeedStorage.GetUserFeedAsync(currentUserProvider.UserId, skip, take);

            Response.Headers.Add(Consts.Headers.TotalCount, totalCount.ToString());
            Response.Headers.Add(Consts.Headers.HasMore, (skip + take < totalCount).ToString());

            return Ok(news);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<NewsFeedPublication>> CreateAsync(
            [FromBody] CreateNewsFeedPublication content)
        {
            var publication = await newsFeedStorage.PublishAsync(content.Content, currentUserProvider.UserId);

            return Created(string.Empty, publication);
        }

        [HttpPut("{publicationId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateAsync(
            [FromRoute] string publicationId,
            [FromBody] CreateNewsFeedPublication model)
        {
            var publication = await newsFeedStorage.GetByIdAsync(publicationId);

            if (publication == null)
            {
                return NotFound();
            }

            if (publication.Author.Id.ToString() != currentUserProvider.UserId)
            {
                return Forbid();
            }

            try
            {
                await newsFeedStorage.UpdateAsync(publicationId, model.Content);
            }
            catch (ApiException ex) when (ex.ErrorCode == (int) HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{publicationId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAsync([FromRoute] string publicationId)
        {
            var publication = await newsFeedStorage.GetByIdAsync(publicationId);

            if (publication == null)
            {
                return NotFound();
            }

            if (publication.Author.Id.ToString() != currentUserProvider.UserId)
            {
                return Forbid();
            }

            await newsFeedStorage.DeleteAsync(publicationId);

            return NoContent();
        }

        [HttpPost("{publicationId}/reaction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ReactionShort>> AddReactionAsync(
            [FromRoute] string publicationId,
            [FromBody] AddNewsFeedReaction model)
        {
            if (await newsFeedStorage.GetByIdAsync(publicationId) == null)
            {
                return NotFound();
            }

            var result = await newsFeedStorage.Reactions
                .AddOrUpdateAsync(publicationId, model.Reaction, currentUserProvider.UserId);

            return Ok(result);
        }

        [HttpDelete("{publicationId}/reaction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RemoveReactionAsync(
            [FromRoute] string publicationId)
        {
            if (await newsFeedStorage.GetByIdAsync(publicationId) == null)
            {
                return NotFound();
            }

            var result = await newsFeedStorage.Reactions
                .RemoveAsync(publicationId, currentUserProvider.UserId);

            return Ok(result);
        }

        [HttpGet("{publicationId}/comments")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, Consts.Headers.TotalCount, "number", "")]
        [SwaggerResponseHeader(StatusCodes.Status200OK, Consts.Headers.HasMore, "boolean", "")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> SearchCommentsAsync(
            [FromRoute] string publicationId,
            [FromQuery, Range(0, int.MaxValue)] int skip,
            [FromQuery, Range(0, 100)] int take = 10)
        {
            var (comments, totalCount) = await newsFeedStorage.Comments
                .GetAsync(publicationId, skip, take);

            Response.Headers.Add(Consts.Headers.TotalCount, totalCount.ToString());
            Response.Headers.Add(Consts.Headers.HasMore, (skip + take < totalCount).ToString());

            return Ok(comments);
        }

        [HttpPost("{publicationId}/comments")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<PublicationComment>> AddCommentAsync(
            [FromRoute] string publicationId,
            [FromBody] AddNewsFeedComment model)
        {
            if (await newsFeedStorage.GetByIdAsync(publicationId) == null)
            {
                return NotFound();
            }

            var comment = await newsFeedStorage.Comments
                .PublishAsync(model.Content, publicationId, currentUserProvider.UserId);

            return Created(string.Empty, comment);
        }

        [HttpDelete("comments/{commentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PublicationComment>> DeleteCommentAsync([FromRoute] string commentId)
        {
            var comment = await newsFeedStorage.Comments
                .GetByIdAsync(commentId);

            if (comment == null)
            {
                return NotFound();
            }

            if (comment.Author.Id.ToString() != currentUserProvider.UserId)
            {
                return Forbid();
            }

            await newsFeedStorage.Comments
                .DeleteAsync(commentId);

            return NoContent();
        }
    }
}
