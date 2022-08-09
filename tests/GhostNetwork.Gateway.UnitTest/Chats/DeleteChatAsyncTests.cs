using System;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Chats;
using GhostNetwork.Gateway.Messages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Gateway.UnitTest.Chats;

[TestFixture]
public class DeleteChatAsyncTests
{
	[Test]
	public async Task Delete_NoContent()
	{
		// Arrange
		var chatId = "someId";
		var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");

		var chat = new Chat(chatId, "name", new[] {new UserInfo(userId, "", null)});

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
		var response = await client.DeleteAsync($"/chats/{chatId}");

		// Assert
		Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
	}
}