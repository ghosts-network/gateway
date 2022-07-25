using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GhostNetwork.Gateway.Api.Messages;

[Route("[controller]")]
[ApiController]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageStorage messageStorage;
    private readonly ICurrentUserProvider currentUserProvider;

    public MessagesController(IMessageStorage messageStorage, ICurrentUserProvider currentUserProvider)
    {
        this.messageStorage = messageStorage;
        this.currentUserProvider = currentUserProvider;
    }

    [HttpGet("{chatId}/messages")]
    public async Task<ActionResult<IEnumerable<Message>>> SearchAsync(
        [FromRoute] string chatId,
        [FromQuery, Range(0, int.MaxValue)] string cursor,
        [FromQuery, Range(1, 50)] int take = 20)
    {
        var messages = await messageStorage.SearchAsync(chatId, cursor, take);

        return Ok(messages);
    }

    [HttpGet("{chatId}/messages/{messageId}")]
    public async Task<ActionResult<Message>> GetByIdAsync([FromRoute] string chatId, [FromRoute] string messageId)
    {
        var message = await messageStorage.GetByIdAsync(chatId, messageId);

        if (message is null)
        {
            return NotFound();
        }

        return Ok(message);
    }

    [HttpPost("{chatId}/messages")]
    public async Task<ActionResult<Message>> CreateAsync([FromRoute] string chatId, [FromBody] CreateMessage model)
    {
        var message = await messageStorage.CreateAsync(chatId, Guid.Parse(currentUserProvider.UserId), model.Content);

        return Ok(message);
    }

    [HttpPut("{chatId}/messages/{messageId}")]
    public async Task<ActionResult<Message>> UpdateAsync([FromRoute] string chatId, [FromRoute] string messageId, [FromBody] UpdateMessage model)
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

        await messageStorage.UpdateAsync(chatId, messageId, model.Content);

        return NoContent();
    }

    [HttpDelete("{chatId}/messages/{messageId}")]
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