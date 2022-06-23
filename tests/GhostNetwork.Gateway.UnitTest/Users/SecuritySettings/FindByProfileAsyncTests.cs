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
            var settins = SecuritySettingModel.DefaultForUser(userId);
            
            var serviceMock = new Mock<ISecuritySettingStorage>();
            serviceMock
                .Setup(s => s.FindByProfileAsync(userId))
                .ReturnsAsync(settins);

            var currentProviderMock = new Mock<ICurrentUserProvider>();
            currentProviderMock.Setup(s => s.UserId)
                .Returns(userId.ToString());

            var userStorage = new Mock<IUsersStorage>();
            userStorage.Setup(s => s.SecuritySettings).Returns(serviceMock.Object);
            
            var client = TestServerHelper.New(collection =>
            {
                collection.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                collection.AddScoped(_ => serviceMock.Object);
                collection.AddScoped(_ => userStorage.Object);
                collection.AddScoped(_ => currentProviderMock.Object);
            });

            //Act
            var response = await client.GetAsync($"/SecuritySettings");

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
