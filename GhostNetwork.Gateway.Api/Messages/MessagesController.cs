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

[Route("{chatId}/messages")]
[ApiController]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageStorage messageStorage;
    private readonly IChatStorage chatStorage;
    private readonly ICurrentUserProvider currentUserProvider;

    public MessagesController(
        IMessageStorage messageStorage,
        IChatStorage chatStorage,
        ICurrentUserProvider currentUserProvider)
    {
        this.messageStorage = messageStorage;
        this.chatStorage = chatStorage;
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
        var chat = await chatStorage.GetByIdAsync(chatId);

        if (chat is null)
        {
            return NotFound();
        }

        var (messages, nextCursor) = await messageStorage.SearchAsync(chatId, cursor, take);

        Response.Headers.Add(Consts.Headers.Cursor, nextCursor);

        return Ok(messages);
    }

    /// <summary>
    /// Get message by id.
    /// </summary>
    /// <param name="chatId">Chat id.</param>
    /// <param name="messageId">Message id.</param>
    /// <returns>Message.</returns>
    [HttpGet("{messageId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Message>> GetByIdAsync([FromRoute] string chatId, [FromRoute] string messageId)
    {
        var chat = await chatStorage.GetByIdAsync(chatId);
        var message = await messageStorage.GetByIdAsync(chatId, messageId);

        if (chat is null || message is null)
        {
            return NotFound();
        }

        return Ok(message);
    }

    /// <summary>
    /// Create message.
    /// </summary>
    /// <param name="chatId">Chat id.</param>
    /// <param name="model">Message model.</param>
    /// <returns>Message.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Message>> CreateAsync([FromRoute] string chatId, [FromBody] CreateMessage model)
    {
        var chat = await chatStorage.GetByIdAsync(chatId);

        if (chat is null)
        {
            return NotFound();
        }

        var message = await messageStorage.CreateAsync(chatId, Guid.Parse(currentUserProvider.UserId), model.Content);

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
    public async Task<ActionResult<Message>> UpdateAsync([FromRoute] string chatId, [FromRoute] string messageId, [FromBody] UpdateMessageModel model)
    {
        var chat = await chatStorage.GetByIdAsync(chatId);
        var message = await messageStorage.GetByIdAsync(chatId, messageId);

        if (chat is null || message is null)
        {
            return NotFound();
        }

        if (message.Author.Id.ToString() != currentUserProvider.UserId)
        {
            return Forbid();
        }

        await messageStorage.UpdateAsync(chatId, messageId, model.Content);

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
        var chat = await chatStorage.GetByIdAsync(chatId);
        var message = await messageStorage.GetByIdAsync(chatId, messageId);

        if (chat is null || message is null)
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