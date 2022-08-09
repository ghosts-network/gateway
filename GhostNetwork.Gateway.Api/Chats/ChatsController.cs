using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Chats;
using GhostNetwork.Gateway.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Gateway.Api.Chats;

[Route("[controller]")]
[ApiController]
[Authorize]
public class ChatsController : ControllerBase
{
    private readonly IChatStorage chatStorage;
    private readonly ICurrentUserProvider currentUserProvider;

    public ChatsController(IChatStorage chatStorage, ICurrentUserProvider currentUserProvider)
    {
        this.chatStorage = chatStorage;
        this.currentUserProvider = currentUserProvider;
    }

    /// <summary>
    /// Search user chats.
    /// </summary>
    /// <param name="cursor">Skip chats up to a specified id.</param>
    /// <param name="take">Take chats up to a specified position.</param>
    /// <returns>Filtered sequence of chats.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerResponseHeader(StatusCodes.Status200OK, Consts.Headers.Cursor, "string", "")]
    public async Task<ActionResult<IReadOnlyCollection<Chat>>> SearchAsync(
        [FromQuery, Range(0, int.MaxValue)] string cursor,
        [FromQuery, Range(1, 50)] int take = 20)
    {
        var (chats, nextCursor) = await chatStorage.GetAsync(currentUserProvider.UserId, cursor, take);

        Response.Headers.Add(Consts.Headers.Cursor, nextCursor);

        return Ok(chats);
    }

    /// <summary>
    /// Create chat.
    /// </summary>
    /// <param name="model">Chat model.</param>
    /// <returns>New chat.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Chat>> CreateAsync([FromBody] CreateChat model)
    {
        var chat = await chatStorage.CreateAsync(model.Name, model.Participants);

        return Created(string.Empty, chat);
    }

    /// <summary>
    /// Get chat by id.
    /// </summary>
    /// <param name="id">Chat id.</param>
    /// <returns>Chat.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Chat>> GetByIdAsync([FromRoute] string id)
    {
        var chat = await chatStorage.GetByIdAsync(id);

        if (chat is null)
        {
            return NotFound();
        }

        if (chat.Participants.All(x => x.Id.ToString() != currentUserProvider.UserId))
        {
            return Forbid();
        }

        return Ok(chat);
    }

    /// <summary>
    /// Update chat.
    /// </summary>
    /// <param name="id">Chat id.</param>
    /// <param name="model">Update chat model.</param>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UpdateAsync([FromRoute] string id, [FromBody] UpdateChat model)
    {
        var chat = await chatStorage.GetByIdAsync(id);

        if (chat is null)
        {
            return NotFound();
        }

        if (chat.Participants.All(x => x.Id.ToString() != currentUserProvider.UserId))
        {
            return Forbid();
        }

        await chatStorage.UpdateAsync(id, model.Name, model.Participants);

        return NoContent();
    }

    /// <summary>
    /// Delete chat.
    /// </summary>
    /// <param name="id">Chat id.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAsync([FromRoute] string id)
    {
        var chat = await chatStorage.GetByIdAsync(id);

        if (chat is null)
        {
            return NotFound();
        }

        if (chat.Participants.All(x => x.Id.ToString() != currentUserProvider.UserId))
        {
            return Forbid();
        }

        await chatStorage.DeleteAsync(id);

        return NoContent();
    }
}