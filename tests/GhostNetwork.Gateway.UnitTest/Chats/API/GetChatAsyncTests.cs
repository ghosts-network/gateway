using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Chats;
using GhostNetwork.Gateway.Messages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Gateway.UnitTest.Chats.API;

[TestFixture]
public class GetChatAsyncTests
{
	[Test]
	public async Task Chat_Ok()
	{
		// Arrange
		var chatId = "someId";
		var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
		
		var chat = new Chat(chatId, "name", new []{new UserInfo(userId, "userName", null)});

		var serviceMock = new Mock<IChatStorage>();
		var currentUserProviderMock = new Mock<ICurrentUserProvider>();

		currentUserProviderMock.Setup(x => x.UserId).Returns(userId.ToString);

		serviceMock.Setup(x => x.GetByIdAsync(chatId)).ReturnsAsync(chat);
		
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddAuthentication("Test")
				.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
			collection.AddScoped(_ => serviceMock.Object);
			collection.AddScoped(_ => currentUserProviderMock.Object);
		});
		// Act
		var response = await client.GetAsync($"/chats/{chatId}");
		
		// Assert
		Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
	}
	
	[Test]
	public async Task Chat_NotFound()
	{
		// Arrange
		var chatId = "someId";

		var serviceMock = new Mock<IChatStorage>();

		serviceMock.Setup(x => x.GetByIdAsync(chatId)).ReturnsAsync(default(Chat));
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddAuthentication("Test")
				.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
			collection.AddScoped(_ => serviceMock.Object);
		});
		// Act
		var response = await client.GetAsync($"/chat/{chatId}");
		
		// Assert
		Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
	}
	
	[Test]
	public async Task Chat_Forbidden()
	{
		// Arrange
		var chatId = "someId";
		var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
		
		var chat = new Chat(chatId, "name", new []{new UserInfo(Guid.NewGuid(), "userName", null)});

		var serviceMock = new Mock<IChatStorage>();
		var currentUserProviderMock = new Mock<ICurrentUserProvider>();

		currentUserProviderMock.Setup(x => x.UserId).Returns(userId.ToString);
		
		serviceMock.Setup(x => x.GetByIdAsync(chatId)).ReturnsAsync(chat);
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddAuthentication("Test")
				.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
			collection.AddScoped(_ => serviceMock.Object);
			collection.AddScoped(_ => currentUserProviderMock.Object);
		});
		// Act
		var response = await client.GetAsync($"/chats/{chatId}");
		
		// Assert
		Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Test]
	public async Task SearchChats_Ok()
	{
		// Arrange
		string cursor = null;
		var take = 10;
		var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
		
		var chats = new[]{
			new Chat("id1", "name1", new []{new UserInfo(userId, "userName", null)}),
			new Chat("id2", "name2", new []{new UserInfo(userId, "userName", null)})
		};
		
		var serviceMock = new Mock<IChatStorage>();
		
		serviceMock
			.Setup(x => x.GetAsync(userId.ToString(), cursor, take))
			.ReturnsAsync((chats, chats.Last().Id));
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddAuthentication("Test")
				.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
			collection.AddScoped(_ => serviceMock.Object);
		});
		
		// Act
		var response = await client.GetAsync($"/chats");
		
		// Assert
		Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
	}
}