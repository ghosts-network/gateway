﻿using System;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Chats;
using GhostNetwork.Gateway.Messages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Gateway.UnitTest.Messages;

[TestFixture]
public class GetMessageAsyncTests
{
	[Test]
	public async Task GetById_Ok()
	{
		// Arrange
		var chatId = "chatId";
		var messageId = "messageId";
		var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
		
		var chat = new Chat(chatId, "name", new []{new UserInfo(userId, "userName", null)});
		var message = new Message(
			messageId,
			chatId,
			new UserInfo(userId, "Name", null),
			"content",
			DateTimeOffset.Now,
			DateTimeOffset.Now);
		
		var chatServiceMock = new Mock<IChatStorage>();
		var messageServiceMock = new Mock<IMessageStorage>();
		var currentUserMock = new Mock<ICurrentUserProvider>();
		
		chatServiceMock.Setup(x => x.GetByIdAsync(chatId)).ReturnsAsync(chat);
		currentUserMock.Setup(x => x.UserId).Returns(userId.ToString);
		messageServiceMock.Setup(x => x.GetByIdAsync(chatId, messageId)).ReturnsAsync(message);

		var client = TestServerHelper.New(collection =>
		{
			collection.AddAuthentication("Test")
				.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
			collection.AddScoped(_ => chatServiceMock.Object);
			collection.AddScoped(_ => messageServiceMock.Object);
			collection.AddScoped(_ => currentUserMock.Object);
		});
		
		// Act
		var response = await client.GetAsync($"/{chatId}/messages/{messageId}");
		
		// Assert
		Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
	}
	
	[Test]
	public async Task GetById_NotFound()
	{
		// Arrange
		var chatId = "chatId";
		var messageId = "messageId";
		var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
		
		var message = new Message(
			messageId,
			chatId,
			new UserInfo(userId, "Name", null),
			"content",
			DateTimeOffset.Now,
			DateTimeOffset.Now);
		
		var chatServiceMock = new Mock<IChatStorage>();
		var messageServiceMock = new Mock<IMessageStorage>();
		var currentUserMock = new Mock<ICurrentUserProvider>();
		
		chatServiceMock.Setup(x => x.GetByIdAsync(chatId)).ReturnsAsync(default(Chat));
		currentUserMock.Setup(x => x.UserId).Returns(userId.ToString);
		messageServiceMock.Setup(x => x.GetByIdAsync(chatId, messageId)).ReturnsAsync(message);

		var client = TestServerHelper.New(collection =>
		{
			collection.AddAuthentication("Test")
				.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
			collection.AddScoped(_ => chatServiceMock.Object);
			collection.AddScoped(_ => messageServiceMock.Object);
			collection.AddScoped(_ => currentUserMock.Object);
		});
		
		// Act
		var response = await client.GetAsync($"/{chatId}/messages/{messageId}");
		
		// Assert
		Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
	}
}