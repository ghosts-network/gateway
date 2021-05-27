using System;
using System.Net;
using System.Threading.Tasks;
using Domain;
using GhostNetwork.Gateway.Api.Users;
using GhostNetwork.Gateway.Facade;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Gateway.UnitTest.Users
{
    [TestFixture]
    public class UpdateAsyncTests
    {
        [Test]
        public async Task Forbidden_Update_Other_Users()
        {
            // Setup
            var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
            var currentUserId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C7");

            var usersStorageMock = new Mock<IUsersStorage>();
            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            currentUserProviderMock
                .Setup(s => s.UserId)
                .Returns(currentUserId.ToString());

            var client = TestServerHelper.New(collection =>
            {
                collection.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                collection.AddScoped(_ => usersStorageMock.Object);
                collection.AddScoped(_ => currentUserProviderMock.Object);
            });

            // Act
            var response = await client.PutAsync($"/users/{userId}", new UpdateUserInput().AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Test]
        public async Task User_Not_Found()
        {
            // Setup
            var userId = Guid.NewGuid();

            var usersStorageMock = new Mock<IUsersStorage>();
            usersStorageMock
                .Setup(s => s.GetByIdAsync(userId))
                .ReturnsAsync(default(User));

            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            currentUserProviderMock
                .Setup(s => s.UserId)
                .Returns(userId.ToString());

            var client = TestServerHelper.New(collection =>
            {
                collection.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                collection.AddScoped(_ => usersStorageMock.Object);
                collection.AddScoped(_ => currentUserProviderMock.Object);
            });

            // Act
            var response = await client.PutAsync($"/users/{userId}", new UpdateUserInput().AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task Updated_Successfully()
        {
            // Setup
            var userId = Guid.NewGuid();

            var usersStorageMock = new Mock<IUsersStorage>();
            usersStorageMock
                .Setup(s => s.GetByIdAsync(userId))
                .ReturnsAsync(new User(userId, "FirstName", "LastName", "Gender", DateTime.Today));
            usersStorageMock
                .Setup(s => s.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(DomainResult.Success);

            var currentUserProviderMock = new Mock<ICurrentUserProvider>();
            currentUserProviderMock
                .Setup(s => s.UserId)
                .Returns(userId.ToString());

            var client = TestServerHelper.New(collection =>
            {
                collection.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                collection.AddScoped(_ => usersStorageMock.Object);
                collection.AddScoped(_ => currentUserProviderMock.Object);
            });

            var update = new UpdateUserInput
            {
                Gender = "New Gender",
                DateOfBirth = null
            };

            // Act
            var response = await client.PutAsync($"/users/{userId}", update.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}