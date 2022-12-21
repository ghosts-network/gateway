using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Infrastructure.SecuritySettingResolver;
using GhostNetwork.Gateway.NewsFeed;
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
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly SecuritySettingsPublicationResolver publicationAccessResolver;
        private readonly FeatureFlags featureFlags;

        public NewsFeedController(
            INewsFeedStorage newsFeedStorage,
            ICurrentUserProvider currentUserProvider,
            SecuritySettingsPublicationResolver publicationAccessResolver,
            FeatureFlags featureFlags)
        {
            this.newsFeedStorage = newsFeedStorage;
            this.currentUserProvider = currentUserProvider;
            this.publicationAccessResolver = publicationAccessResolver;
            this.featureFlags = featureFlags;
        }

        [HttpGet("users/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, Consts.Headers.Cursor, "string", "")]
        public async Task<ActionResult<IEnumerable<NewsFeedPublication>>> GetByUserAsync(
            [FromRoute] Guid userId,
            [FromQuery, Range(1, 50)] int take = 20,
            [FromQuery] string cursor = null)
        {
            var resolveResult = await publicationAccessResolver.ResolveAccessAsync(userId);
            if (!resolveResult.Successed)
            {
                return Forbid();
            }

            var (news, nextCursor) = await newsFeedStorage.GetUserPublicationsAsync(userId, take, cursor);

            Response.Headers.Add(Consts.Headers.Cursor, nextCursor);

            return Ok(news);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerResponseHeader(StatusCodes.Status200OK, Consts.Headers.Cursor, "string", "")]
        public async Task<ActionResult<IEnumerable<NewsFeedPublication>>> GetAsync(
            [FromQuery, Range(1, 50)] int take = 20,
            [FromQuery] string cursor = null)
        {
            if (featureFlags.PersonalizedNewsFeedEnabled(currentUserProvider.UserId))
            {
                var (news, nextCursor) = await newsFeedStorage.GetPersonalizedFeedAsync(currentUserProvider.UserId, take, cursor);

                Response.Headers.Add(Consts.Headers.Cursor, nextCursor);

                return Ok(news);
            }
            else
            {
                var (news, nextCursor) = await newsFeedStorage.GetUserFeedAsync(currentUserProvider.UserId, take, cursor);

                Response.Headers.Add(Consts.Headers.Cursor, nextCursor);

                return Ok(news);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<NewsFeedPublication>> CreateAsync(
            [FromBody] CreateNewsFeedPublication content)
        {
            var media = Enumerable.Empty<Media>();

            if (content.Media is not null)
            {
                var mediaStream = (
                        from m in content.Media
                        let bytes = Convert.FromBase64String(m.Base64)
                        let stream = new MemoryStream(bytes)
                        select new MediaStream(stream, m.FileName))
                    .ToList();

                media = await newsFeedStorage.Media.UploadAsync(mediaStream, currentUserProvider.UserId);
            }

            var publication = await newsFeedStorage.PublishAsync(
                content.Content,
                await currentUserProvider.GetProfileAsync(),
                media.ToList());

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

            var media = Enumerable.Empty<Media>();

            if (model.Media is not null)
            {
                var mediaStream = (
                        from m in model.Media
                        let bytes = Convert.FromBase64String(m.Base64)
                        let stream = new MemoryStream(bytes)
                        select new MediaStream(stream, m.FileName))
                    .ToList();

                media = await newsFeedStorage.Media.UploadAsync(mediaStream, currentUserProvider.UserId);
            }

            await newsFeedStorage.UpdateAsync(publicationId, model.Content, media.ToList());

            return NoContent();
        }

        [HttpDelete("{publicationId}/media")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteManyMedia([FromRoute] string publicationId, [FromQuery] IEnumerable<string> fileNames)
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

            await newsFeedStorage.Media.DeleteManyAsync(fileNames, publicationId);

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

            var fileNames = publication.Media is null
                ? Enumerable.Empty<string>()
                : publication.Media.Select(x => x.Link);

            await newsFeedStorage.DeleteAsync(publicationId, fileNames);

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
        [SwaggerResponseHeader(StatusCodes.Status200OK, Consts.Headers.Cursor, "string", "")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> SearchCommentsAsync(
            [FromRoute] string publicationId,
            [FromQuery, Range(0, int.MaxValue)] int skip,
            [FromQuery, Range(0, 100)] int take = 10,
            [FromQuery] string cursor = null)
        {
            var (comments, totalCount, nextCursor) = await newsFeedStorage.Comments
                .GetAsync(publicationId, skip, take, cursor);

            Response.Headers.Add(Consts.Headers.TotalCount, totalCount.ToString());
            Response.Headers.Add(Consts.Headers.HasMore, (skip + take < totalCount).ToString());
            Response.Headers.Add(Consts.Headers.Cursor, nextCursor);

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
                .PublishAsync(model.Content, publicationId, await currentUserProvider.GetProfileAsync());

            return Created(string.Empty, comment);
        }

        [HttpPut("comments/{commentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateCommentAsync(
            [FromRoute] string commentId,
            [FromBody][Required] UpdateNewsFeedComment model)
        {
            var comment = await newsFeedStorage.Comments.GetByIdAsync(commentId);

            if (comment == null)
            {
                return NotFound();
            }

            if (comment.Author.Id != new Guid(currentUserProvider.UserId))
            {
                return Forbid();
            }

            var result = await newsFeedStorage.Comments.UpdateAsync(commentId, model.Content);

            if (!result.Successed)
            {
                return BadRequest();
            }

            return NoContent();
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
