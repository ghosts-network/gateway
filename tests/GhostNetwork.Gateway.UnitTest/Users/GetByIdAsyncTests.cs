using System;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Gateway.UnitTest.Users
{
    [TestFixture]
    public class GetByIdAsyncTests
    {
        [Test]
        public async Task User_Not_Found()
        {
            // Setup
            var id = Guid.NewGuid();
            var serviceMock = new Mock<IUsersStorage>();
            serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(default(User));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.GetAsync($"/users/{id}");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task User_Exists()
        {
            // Setup
            var id = Guid.NewGuid();
            var user = new User(id, "FirstName", "LastName", "Gender", DateTime.Today, null);
            var serviceMock = new Mock<IUsersStorage>();
            serviceMock
                .Setup(s => s.GetByIdAsync(id))
                .ReturnsAsync(user);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                collection.AddScoped(_ => serviceMock.Object);
            });

            // Act
            var response = await client.GetAsync($"/users/{id}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}