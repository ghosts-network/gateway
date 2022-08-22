using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Chats;
using GhostNetwork.Gateway.Messages;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Gateway.UnitTest.Messages.Validator;

[TestFixture]
public class MessageValidatorTests
{
	[Test]
	public async Task Message_Validate_Success()
	{
		// Arrange
		var chatId = "chatId";
		var authorId = Guid.NewGuid();
		var context = new MessageContext(chatId, authorId, "message");
		var chatStorageMock = new Mock<IChatStorage>();

		chatStorageMock
			.Setup(x => x.GetByIdAsync(chatId))
			.ReturnsAsync(new Chat(chatId, "Name", new List<UserInfo>() {new(authorId, "name", null)}));
		
		var validator = new MessageValidator(chatStorageMock.Object);
		
		// Act
		var result = await validator.ValidateAsync(context);
		
		// Assert
		Assert.IsTrue(result.Successed);
	}
	
	[Test]
	public async Task Message_Validate_Chat_Is_NotFound()
	{
		// Arrange
		var chatId = "chatId";
		var authorId = Guid.NewGuid();
		var context = new MessageContext(chatId, authorId, "message");
		var chatStorageMock = new Mock<IChatStorage>();

		chatStorageMock
			.Setup(x => x.GetByIdAsync(chatId))
			.ReturnsAsync(default(Chat));
		
		var validator = new MessageValidator(chatStorageMock.Object);
		
		// Act
		var result = await validator.ValidateAsync(context);
		
		// Assert
		Assert.IsFalse(result.Successed);
	}
	
	[Test]
	public async Task Message_Validate_Author_Is_Not_Participant()
	{
		// Arrange
		var chatId = "chatId";
		var authorId = Guid.NewGuid();
		var context = new MessageContext(chatId, authorId, "message");
		var chatStorageMock = new Mock<IChatStorage>();

		chatStorageMock
			.Setup(x => x.GetByIdAsync(chatId))
			.ReturnsAsync(new Chat(chatId, "Name", new List<UserInfo>() {new(Guid.NewGuid(), "name", null)}));
		
		var validator = new MessageValidator(chatStorageMock.Object);
		
		// Act
		var result = await validator.ValidateAsync(context);
		
		// Assert
		Assert.IsFalse(result.Successed);
	}
	
	[Test]
	public async Task Message_Validate_Content_Is_Empty()
	{
		// Arrange
		var chatId = "chatId";
		var authorId = Guid.NewGuid();
		var context = new MessageContext(chatId, authorId, "");
		var chatStorageMock = new Mock<IChatStorage>();

		chatStorageMock
			.Setup(x => x.GetByIdAsync(chatId))
			.ReturnsAsync(new Chat(chatId, "Name", new List<UserInfo>() {new(authorId, "name", null)}));
		
		var validator = new MessageValidator(chatStorageMock.Object);
		
		// Act
		var result = await validator.ValidateAsync(context);
		
		// Assert
		Assert.IsFalse(result.Successed);
	}
}