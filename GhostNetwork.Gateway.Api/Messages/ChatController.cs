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
	public async Task<ActionResult<IEnumerable<Chat>>> SearchAsync(
		[FromQuery, Range(0, int.MaxValue)] string cursor,
		[FromQuery, Range(1, 50)] int take = 20)
	{
		var chats = await chatStorage.GetAsync(currentUserProvider.UserId, cursor, take);

		return Ok(chats);
	}
}