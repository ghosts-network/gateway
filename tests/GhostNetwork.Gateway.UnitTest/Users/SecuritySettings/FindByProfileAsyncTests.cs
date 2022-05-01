using GhostNetwork.Gateway.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.UnitTest.Users.SecuritySettings
{
    [TestFixture]
    internal class FindByProfileAsyncTests
    {
        [Test]
        public async Task FindByProfileAsync_OK()
        {
            //Setup
            var userId = Guid.NewGuid();
            var settins = SecuritySetting.DefaultForUser(userId);
            
            var serviceMock = new Mock<ISecuritySettingStorage>();
            serviceMock
                .Setup(s => s.FindByProfileAsync(userId))
                .ReturnsAsync(settins);

            var userStorage = new Mock<IUsersStorage>();
            userStorage.Setup(s => s.SecuritySettings).Returns(serviceMock.Object);
            
            var client = TestServerHelper.New(collection =>
            {
                collection.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                collection.AddScoped(_ => serviceMock.Object);
                collection.AddScoped(_ => userStorage.Object);
            });

            //Act
            var response = await client.GetAsync($"/SecuritySettings/{userId}");

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [Test]
        public async Task FindByProfileAsync_NotFound()
        {
            //Setup
            var userId = Guid.NewGuid();
            
            var serviceMock = new Mock<ISecuritySettingStorage>();
            serviceMock
                .Setup(s => s.FindByProfileAsync(userId))
                .ReturnsAsync(default(SecuritySetting));

            var userStorage = new Mock<IUsersStorage>();
            userStorage.Setup(s => s.SecuritySettings).Returns(serviceMock.Object);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                collection.AddScoped(_ => serviceMock.Object);
                collection.AddScoped(_ => userStorage.Object);
            });

            //Act
            var response = await client.GetAsync($"/SecuritySettings/{userId}");

            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
