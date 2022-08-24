using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Gateway.Api.Messages;
using GhostNetwork.Gateway.Chats;
using GhostNetwork.Gateway.Messages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Gateway.UnitTest.Messages.API;

[TestFixture]
public class CreateMessageAsyncTests
{
	[Test]
	public async Task Create_Created()
	{
		// Arrange
		const string chatId = "chatId";
		var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
		var currentUser = new UserInfo(userId, "CurrentUser", null);
		
		var model = new CreateMessage()
		{
			Content = "Test",
		};
		
		var chat = new Chat(chatId, "Test chat name", new List<UserInfo> { currentUser });
		var message = new Message("messageId", chatId, currentUser, "content", DateTimeOffset.Now, DateTimeOffset.Now);
		
		var chatServiceMock = new Mock<IChatStorage>();
		var messageServiceMock = new Mock<IMessageStorage>();
		var currentUserMock = new Mock<ICurrentUserProvider>();

		chatServiceMock.Setup(x => x.GetByIdAsync(chatId)).ReturnsAsync(chat);
		currentUserMock.Setup(x => x.UserId).Returns(userId.ToString);
		messageServiceMock.Setup(x => x.CreateAsync(chatId, userId, model.Content)).ReturnsAsync((DomainResult.Success(), message));
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddAuthentication("Test")
				.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
			collection.AddScoped(_ => chatServiceMock.Object);
			collection.AddScoped(_ => messageServiceMock.Object);
			collection.AddScoped(_ => currentUserMock.Object);
		});
		
		// Act
		var response = await client.PostAsync($"/chats/{chatId}/messages", model.AsJsonContent());
		
		// Assert
		Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
	}
}