using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Facade;
using GhostNetwork.Publications.Api;
using GhostNetwork.Publications.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using UserInfo = GhostNetwork.Gateway.Facade.UserInfo;

namespace GhostNetwork.Gateway.Api.NewsFeed
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class NewsFeedController : ControllerBase
    {
        private readonly IPublicationsApi publicationsApi;
        private readonly ICommentsApi commentsApi;
        private readonly IReactionsApi reactionsApi;
        private readonly ICurrentUserProvider currentUserProvider;

        public NewsFeedController(IPublicationsApi publicationsApi, ICommentsApi commentsApi, IReactionsApi reactionsApi, ICurrentUserProvider currentUserProvider)
        {
            this.publicationsApi = publicationsApi;
            this.commentsApi = commentsApi;
            this.reactionsApi = reactionsApi;
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
            var publicationsResponse = await publicationsApi.SearchWithHttpInfoAsync(skip, take, order: Ordering.Desc);
            var publications = publicationsResponse.Data;
            var totalCount = GetTotalCountHeader(publicationsResponse);

            var featuredComments = await LoadCommentsAsync(publications.Select(p => p.Id));
            var reactions = await LoadReactionsAsync(publications.Select(p => p.Id));
            var userReactions = currentUserProvider.UserId == null
                ? new Dictionary<string, UserReaction>()
                : await LoadUserReactionsAsync(currentUserProvider.UserId, publications.Select(p => p.Id));

            var news = new List<NewsFeedPublication>(publications.Count);

            foreach (var publication in publications)
            {
                news.Add(new NewsFeedPublication(
                    publication.Id,
                    publication.Content,
                    featuredComments[publication.Id],
                    new ReactionShort(reactions[publication.Id], userReactions[publication.Id]),
                    ToUser(publication.Author)));
            }

            Response.Headers.Add(Consts.Headers.TotalCount, totalCount.ToString());
            Response.Headers.Add(Consts.Headers.HasMore, (skip + take < totalCount).ToString());

            return Ok(news);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<NewsFeedPublication>> CreateAsync(
            [FromBody] CreateNewsFeedPublication content)
        {
            var model = new CreatePublicationModel(content.Content, ToUserModel(await currentUserProvider.GetProfileAsync()));
            var entity = await publicationsApi.CreateAsync(model);

            var backModel = new NewsFeedPublication(
                entity.Id,
                entity.Content,
                new CommentsShort(Enumerable.Empty<PublicationComment>(), 0),
                new ReactionShort(new Dictionary<ReactionType, int>(), null),
                ToUser(entity.Author));

            return Created(string.Empty, backModel);
        }

        [HttpPut("{publicationId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateAsync(
            [FromRoute] string publicationId,
            [FromBody] CreateNewsFeedPublication model)
        {
            try
            {
                var publication = await publicationsApi.GetByIdAsync(publicationId);

                if (publication.Author.Id.ToString() != currentUserProvider.UserId)
                {
                    return Forbid();
                }
            }
            catch (Publications.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            try
            {
                await publicationsApi.UpdateAsync(publicationId, new UpdatePublicationModel(model.Content));
            }
            catch (Publications.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
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
            try
            {
                var publication = await publicationsApi.GetByIdAsync(publicationId);

                if (publication.Author.Id.ToString() != currentUserProvider.UserId)
                {
                    return Forbid();
                }
            }
            catch (Publications.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            await reactionsApi.DeleteAsync($"publication_${publicationId}");
            await publicationsApi.DeleteAsync(publicationId);

            return NoContent();
        }

        [HttpPost("{publicationId}/reaction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ReactionShort>> AddReactionAsync(
            [FromRoute] string publicationId,
            [FromBody] AddNewsFeedReaction model)
        {
            try
            {
                await publicationsApi.GetByIdAsync(publicationId);
            }
            catch (Publications.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
            {
                return BadRequest();
            }

            var result = await reactionsApi.UpsertAsync($"publication_{publicationId}", model.Reaction.ToString(), currentUserProvider.UserId);

            return Ok(await ToReactionShort(publicationId, result));
        }

        [HttpDelete("{publicationId}/reaction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RemoveReactionAsync(
            [FromRoute] string publicationId)
        {
            try
            {
                await publicationsApi.GetByIdAsync(publicationId);
            }
            catch (Publications.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
            {
                return BadRequest();
            }

            var result = await reactionsApi.DeleteByAuthorAsync($"publication_{publicationId}", currentUserProvider.UserId);

            return Ok(await ToReactionShort(publicationId, result));
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
            var response = await commentsApi.SearchWithHttpInfoAsync(publicationId, skip, take);

            var totalCount = GetTotalCountHeader(response);

            Response.Headers.Add(Consts.Headers.TotalCount, totalCount.ToString());
            Response.Headers.Add(Consts.Headers.HasMore, (skip + take < totalCount).ToString());

            return Ok(response.Data.Select(ToDomain).ToList());
        }

        [HttpPost("{publicationId}/comments")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<PublicationComment>> AddCommentAsync(
            [FromRoute] string publicationId,
            [FromBody] AddNewsFeedComment model)
        {
            try
            {
                await publicationsApi.GetByIdAsync(publicationId);
            }
            catch (Publications.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
            {
                return BadRequest();
            }

            var comment = await commentsApi.CreateAsync(new CreateCommentModel(publicationId, model.Content, author: ToUserModel(await currentUserProvider.GetProfileAsync())));

            return Created(string.Empty, new PublicationComment(comment.Id, comment.Content, comment.PublicationId, ToUser(comment.Author), comment.CreatedOn));
        }

        [HttpDelete("comments/{commentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PublicationComment>> DeleteCommentAsync([FromRoute] string commentId)
        {
            try
            {
                var comment = await commentsApi.GetByIdAsync(commentId);

                if (comment.Author.Id.ToString() != currentUserProvider.UserId)
                {
                    return Forbid();
                }
            }
            catch (Publications.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            await commentsApi.DeleteAsync(commentId);

            return NoContent();
        }

        private static UserInfo ToUser(Publications.Model.UserInfo userInfo)
        {
            return new UserInfo(userInfo.Id, userInfo.FullName, userInfo.AvatarUrl);
        }

        private static UserInfoModel ToUserModel(UserInfo userInfo)
        {
            return new UserInfoModel(userInfo.Id, userInfo.FullName, userInfo.AvatarUrl);
        }

        private static long GetTotalCountHeader<T>(Publications.Client.ApiResponse<T> response)
        {
            var totalCount = 0;
            if (response.Headers.TryGetValue("X-TotalCount", out var headers))
            {
                if (!int.TryParse(headers.FirstOrDefault(), out totalCount))
                {
                    totalCount = 0;
                }
            }

            return totalCount;
        }

        private static PublicationComment ToDomain(Comment entity)
        {
            return new PublicationComment(
                entity.Id,
                entity.Content,
                entity.PublicationId,
                ToUser(entity.Author),
                entity.CreatedOn);
        }

        private async Task<ReactionShort> ToReactionShort(string publicationId, Dictionary<string, int> response)
        {
            var reactions = response.Keys
                    .Select(k => (Enum.Parse<ReactionType>(k), response[k]))
                    .ToDictionary(o => o.Item1, o => o.Item2);

            UserReaction userReaction = null;

            if (currentUserProvider.UserId != null)
            {
                try
                {
                    var reactionByAuthor = await reactionsApi.GetReactionByAuthorAsync($"publication_{publicationId}", currentUserProvider.UserId);

                    userReaction = new UserReaction(Enum.Parse<ReactionType>(reactionByAuthor.Type));
                }
                catch (Publications.Client.ApiException ex) when (ex.ErrorCode == (int)HttpStatusCode.NotFound)
                {
                    // ignored
                }
            }

            return new ReactionShort(reactions, userReaction);
        }

        private async Task<Dictionary<string, CommentsShort>> LoadCommentsAsync(IEnumerable<string> publicationIds)
        {
            var query = new FeaturedQuery(publicationIds.ToList());
            var featuredComments = await commentsApi.SearchFeaturedAsync(query);

            return publicationIds
                .ToDictionary(id => id, id =>
                {
                    var comment = featuredComments.GetValueOrDefault(id);

                    return new CommentsShort(
                        comment?.Comments.Select(ToDomain) ?? Enumerable.Empty<PublicationComment>(),
                        comment?.TotalCount ?? 0);
                });
        }

        private async Task<Dictionary<string, Dictionary<ReactionType, int>>> LoadReactionsAsync(IEnumerable<string> publicationIds)
        {
            var query = new ReactionsQuery
            {
                Keys = publicationIds.Select(id => $"publication_{id}").ToList()
            };
            var reactions = await reactionsApi.GetGroupedReactionsAsync(query);

            return publicationIds
                .ToDictionary(id => id, id =>
                {
                    var r = reactions.ContainsKey($"publication_{id}")
                        ? reactions[$"publication_{id}"]
                        : new Dictionary<string, int>();
                    return r.Keys
                        .Select(k => (Enum.Parse<ReactionType>(k), r[k]))
                        .ToDictionary(o => o.Item1, o => o.Item2);
                });
        }

        private async Task<Dictionary<string, UserReaction>> LoadUserReactionsAsync(string userId, IEnumerable<string> publicationIds)
        {
            var query = new ReactionsQuery
            {
                Keys = publicationIds.Select(id => $"publication_{id}").ToList()
            };
            var userReactionsResponse = await reactionsApi.SearchAsync(userId, query);
            var userReactions = currentUserProvider.UserId == null
                ? new Dictionary<string, Reaction>()
                : userReactionsResponse
                    .GroupBy(r => r.Key, r => r)
                    .ToDictionary(k => k.Key, k => k.First());

            return publicationIds
                .ToDictionary(id => id, id => userReactions.ContainsKey($"publication_{id}")
                    ? new UserReaction(Enum.Parse<ReactionType>(userReactions[$"publication_{id}"].Type))
                    : null);
        }
    }
}
