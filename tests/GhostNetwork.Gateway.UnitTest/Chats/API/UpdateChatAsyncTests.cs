﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Gateway.Chats;
using GhostNetwork.Gateway.Messages;
using GhostNetwork.Messages.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Chat = GhostNetwork.Gateway.Chats.Chat;

namespace GhostNetwork.Gateway.UnitTest.Chats.API;

[TestFixture]
public class UpdateChatAsyncTests
{
	[Test]
	public async Task Update_NoContent()
	{
		// Arrange
		var model = new UpdateChatModel()
		{
			Name = "Upd",
			Participants = new List<Guid>() {Guid.NewGuid()}
		};
		
		var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
		var chatId = "someId";
		
		var chat = new Chat(
			chatId,
			model.Name, 
			new List<UserInfo>()
			{
				new(model.Participants[0], "name", null),
				new(userId, "name", null)
			});

		var serviceMock = new Mock<IChatStorage>();
		var currentUserProviderMock = new Mock<ICurrentUserProvider>();
		
		serviceMock
			.Setup(x => x.GetByIdAsync(chatId))
			.ReturnsAsync(chat);

		serviceMock
			.Setup(x => x.UpdateAsync(chatId, model.Name, model.Participants)).ReturnsAsync(DomainResult.Success);
		
		currentUserProviderMock
			.Setup(x => x.UserId)
			.Returns(userId.ToString);
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddAuthentication("Test")
				.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
			collection.AddScoped(_ => serviceMock.Object);
			collection.AddScoped(_ => currentUserProviderMock.Object);
		});
		
		// Act
		var response = await client.PutAsync($"/chats/{chatId}", model.AsJsonContent());
		
		// Assert
		Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
	}

	[Test]
	public async Task Update_NotFound()
	{
		// Arrange
		var model = new UpdateChatModel()
		{
			Name = "Upd",
			Participants = new List<Guid>() {Guid.NewGuid()}
		};
		
		var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
		var chatId = "someId";

		var serviceMock = new Mock<IChatStorage>();
		var currentUserProviderMock = new Mock<ICurrentUserProvider>();
		
		serviceMock
			.Setup(x => x.GetByIdAsync(chatId))
			.ReturnsAsync(default(Chat));

		currentUserProviderMock
			.Setup(x => x.UserId)
			.Returns(userId.ToString);
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddAuthentication("Test")
				.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
			collection.AddScoped(_ => serviceMock.Object);
			collection.AddScoped(_ => currentUserProviderMock.Object);
		});
		
		// Act
		var response = await client.PutAsync($"/chats/{chatId}", model.AsJsonContent());
		
		// Assert
		Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
	}
	
	[Test]
	public async Task Update_Forbidden()
	{
		// Arrange
		var model = new UpdateChatModel()
		{
			Name = "Upd",
			Participants = new List<Guid>() {Guid.NewGuid()}
		};
		
		var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
		var chatId = "someId";
		
		var chat = new Chat(
			chatId,
			model.Name, 
			new List<UserInfo>()
			{
				new(Guid.NewGuid(), "name", null),
				new(Guid.NewGuid(), "name 2", null)
			});

		var serviceMock = new Mock<IChatStorage>();
		var currentUserProviderMock = new Mock<ICurrentUserProvider>();
		
		serviceMock
			.Setup(x => x.GetByIdAsync(chatId))
			.ReturnsAsync(chat);

		serviceMock
			.Setup(x => x.UpdateAsync(chatId, model.Name, model.Participants)).ReturnsAsync(DomainResult.Success);
		
		currentUserProviderMock
			.Setup(x => x.UserId)
			.Returns(userId.ToString);
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddAuthentication("Test")
				.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
			collection.AddScoped(_ => serviceMock.Object);
			collection.AddScoped(_ => currentUserProviderMock.Object);
		});
		
		// Act
		var response = await client.PutAsync($"/chats/{chatId}", model.AsJsonContent());
		
		// Assert
		Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
	}
}