using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Messages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Gateway.UnitTest.Messages.API;

[TestFixture]
public class SearchMessageAsyncTests
{
	[Test]
	public async Task SearchAsync_Ok()
	{
		// Arrange
		var messages = new List<Message>
		{
			new Message(
				"1",
				"chatId-1",
				new UserInfo(Guid.NewGuid(), "Name", null),
				"content",
				DateTimeOffset.Now,
				DateTimeOffset.Now),
			new Message(
				"2",
				"chatId-1",
				new UserInfo(Guid.NewGuid(), "Name", null),
				"content",
				DateTimeOffset.Now,
				DateTimeOffset.Now)
		};

		var serviceMock = new Mock<IMessageStorage>();
		
		serviceMock
			.Setup(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
			.ReturnsAsync((messages, messages.Last().Id));
		
		var client = TestServerHelper.New(collection =>
		{
			collection.AddAuthentication("Test")
				.AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
			collection.AddScoped(_ => serviceMock.Object);
		});
		
		// Act
		var response = await client.GetAsync($"");
		// Assert
	}
}