using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Messages;
using GhostNetwork.Messages.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Chat = GhostNetwork.Gateway.Chats.Chat;

namespace GhostNetwork.Gateway.UnitTest.Chats;

[TestFixture]
public class CreateChatAsyncTests
{
	[Test]
	public async Task Chat_Created()
	{
		// Arrange
		var model = new CreateChatModel()
		{
			Name = "Chat name",
			Participants = new List<Guid>() {Guid.NewGuid(), Guid.NewGuid()}
		};
		
		var currentUser = Guid.NewGuid();

		var chat = new Chat(
			"someId",
			model.Name, 
			new List<UserInfo>()
			{
				new(model.Participants[0], "name", null),
				new(model.Participants[1], "name", null)
			});

		var serviceMock = new Mock<IChatStorage>();
		var currentUserMock = new Mock<ICurrentUserProvider>();
		
		serviceMock.Setup(x => x.CreateAsync(model.Name, model.Participants)).ReturnsAsync(chat);
		currentUserMock.Setup(x => x.UserId).Returns(currentUser.ToString);
		
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddAuthentication("Test")
				.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
			collection.AddScoped(_ => serviceMock.Object);
			collection.AddScoped(_ => currentUserMock.Object);
		});
		
		// Act
		var response = await client.PostAsync("/chats", model.AsJsonContent());
		
		// Assert
		Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
	}
}