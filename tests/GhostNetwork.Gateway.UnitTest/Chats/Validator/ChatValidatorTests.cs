using System;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Chats;
using NUnit.Framework;

namespace GhostNetwork.Gateway.UnitTest.Chats.Validator;

[TestFixture]
public class ChatValidatorTests
{
	[Test]
	public async Task Chat_Success_Validate()
	{
		// Arrange
		var validator = new ChatValidator();
		var name = "Hello World";
		var participants = new[] { Guid.NewGuid(), Guid.NewGuid() };
		
		// Act
		var result = await validator.ValidateAsync(new ChatContext(name, participants));
		
		// Assert
		Assert.IsTrue(result.Successed);
	}
	
	[Test]
	public async Task Chat_Name_Is_Length_Shorter_Than_MaxLength()
	{
		// Arrange
		var validator = new ChatValidator();
		var name = "He";
		var participants = new[] { Guid.NewGuid(), Guid.NewGuid() };
		
		// Act
		var result = await validator.ValidateAsync(new ChatContext(name, participants));
		
		// Assert
		Assert.IsFalse(result.Successed);
	}
	
	[Test]
	public async Task Chat_Name_Is_Length_Longer_Than_MaxLength()
	{
		// Arrange
		var validator = new ChatValidator();
		var name = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce consequat aliquam congue." +
		           " Fusce pellentesque sit amet augue eu finibus. Ut rutrum velit nec libero euismod, nec consectetur" +
		           " nibh porta. Fusce sed mauris elementum, fermentum tortor ac efficitur.";
		var participants = new[] { Guid.NewGuid(), Guid.NewGuid() };
		
		// Act
		var result = await validator.ValidateAsync(new ChatContext(name, participants));
		
		// Assert
		Assert.IsFalse(result.Successed);
	}
	
	[Test]
	public async Task Chat_Participants_Is_More_Then_Ten()
	{
		// Arrange
		var validator = new ChatValidator();
		var name = "Hello World";
		var participants = new[]
		{
			Guid.NewGuid(), Guid.NewGuid(),
			Guid.NewGuid(), Guid.NewGuid(),
			Guid.NewGuid(), Guid.NewGuid(),
			Guid.NewGuid(), Guid.NewGuid(),
			Guid.NewGuid(), Guid.NewGuid(),
			Guid.NewGuid()
		};
		
		// Act
		var result = await validator.ValidateAsync(new ChatContext(name, participants));
		
		// Assert
		Assert.IsFalse(result.Successed);
	}
}