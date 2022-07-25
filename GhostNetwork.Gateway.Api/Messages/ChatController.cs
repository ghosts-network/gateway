using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace GhostNetwork.Gateway.Api.Messages;

[Route("[controller]")]
[ApiController]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatStorage chatStorage;
    private readonly ICurrentUserProvider currentUserProvider;

    public ChatController(IChatStorage chatStorage, ICurrentUserProvider currentUserProvider)
    {
        this.chatStorage = chatStorage;
        this.currentUserProvider = currentUserProvider;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerResponseHeader(StatusCodes.Status200OK, Consts.Headers.Cursor, "string", "")]
    public async Task<ActionResult<IEnumerable<Chat>>> SearchAsync(
        [FromQuery, Range(0, int.MaxValue)] string cursor,
        [FromQuery, Range(1, 50)] int take = 20)
    {
        var chats = await chatStorage.GetAsync(currentUserProvider.UserId, cursor, take);

        Response.Headers.Add(Consts.Headers.Cursor, chats.Last().Id);

        return Ok(chats);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Chat>> CreateAsync([FromBody] CreateChat model)
    {
        var chat = await chatStorage.CreateAsync(model.Name, model.Participants);

        return Created(string.Empty, chat);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Chat>> GetByIdAsync([FromRoute] string id)
    {
        var chat = await chatStorage.GetByIdAsync(id);

        if (chat == null)
        {
            return NotFound();
        }

        return Ok(chat);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UpdateAsync([FromRoute] string id, [FromBody] UpdateChat model)
    {
        var chat = await chatStorage.GetByIdAsync(id);

        if (chat == null)
        {
            return NotFound();
        }

        await chatStorage.UpdateAsync(id, model.Name, model.Participants);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAsync([FromRoute] string id)
    {
        var chat = await chatStorage.GetByIdAsync(id);

        if (chat == null)
        {
            return NotFound();
        }

        await chatStorage.DeleteAsync(id);

        return NoContent();
    }
}