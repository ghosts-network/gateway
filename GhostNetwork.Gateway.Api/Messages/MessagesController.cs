using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Gateway.Api.Messages;

[Route("/chats/{chatId}/messages")]
[ApiController]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageStorage messageStorage;
    private readonly ICurrentUserProvider currentUserProvider;

    public MessagesController(
        IMessageStorage messageStorage,
        ICurrentUserProvider currentUserProvider)
    {
        this.messageStorage = messageStorage;
        this.currentUserProvider = currentUserProvider;
    }

    /// <summary>
    /// Search messages.
    /// </summary>
    /// <param name="chatId">Chat id.</param>
    /// <param name="cursor">Skip messages up to a specified id.</param>
    /// <param name="take">Take messages up to a specified position.</param>
    /// <returns>Messages.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerResponseHeader(StatusCodes.Status200OK, Consts.Headers.Cursor, "string", "")]
    public async Task<ActionResult<IEnumerable<Message>>> SearchAsync(
        [FromRoute] string chatId,
        [FromQuery, Range(0, int.MaxValue)] string cursor,
        [FromQuery, Range(1, 50)] int take = 20)
    {
        var (messages, nextCursor) = await messageStorage.SearchAsync(chatId, cursor, take);

        Response.Headers.Add(Consts.Headers.Cursor, nextCursor);

        return Ok(messages);
    }

    /// <summary>
    /// Send message.
    /// </summary>
    /// <param name="chatId">Chat id.</param>
    /// <param name="model">Message model.</param>
    /// <returns>New message.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Message>> SendAsync([FromRoute] string chatId, [FromBody] CreateMessage model)
    {
        var (result, message) = await messageStorage.CreateAsync(chatId, Guid.Parse(currentUserProvider.UserId), model.Content);

        if (!result.Successed)
        {
            return BadRequest(result.ToProblemDetails());
        }

        return Created(string.Empty, message);
    }

    /// <summary>
    /// Update message.
    /// </summary>
    /// <param name="chatId">Chat id.</param>
    /// <param name="messageId">Message id.</param>
    /// <param name="model">Update message model.</param>
    [HttpPut("{messageId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Message>> UpdateAsync([FromRoute] string chatId, [FromRoute] string messageId, [FromBody] UpdateMessageModel model)
    {
        var message = await messageStorage.GetByIdAsync(chatId, messageId);

        if (message is null)
        {
            return NotFound();
        }

        if (message.Author.Id.ToString() != currentUserProvider.UserId)
        {
            return Forbid();
        }

        var result = await messageStorage.UpdateAsync(chatId, messageId, Guid.Parse(currentUserProvider.UserId),  model.Content);

        if (!result.Successed)
        {
            return BadRequest(result.ToProblemDetails());
        }

        return NoContent();
    }

    /// <summary>
    /// Delete message.
    /// </summary>
    /// <param name="chatId">Chat id.</param>
    /// <param name="messageId">Message id.</param>
    [HttpDelete("{messageId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync([FromRoute] string chatId, [FromRoute] string messageId)
    {
        var message = await messageStorage.GetByIdAsync(chatId, messageId);

        if (message is null)
        {
            return NotFound();
        }

        if (message.Author.Id.ToString() != currentUserProvider.UserId)
        {
            return Forbid();
        }

        await messageStorage.DeleteAsync(chatId, messageId);

        return NoContent();
    }
}