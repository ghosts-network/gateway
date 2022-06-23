using Domain;
using GhostNetwork.Gateway.Users;
using GhostNetwork.Profiles.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.UnitTest.Users.SecuritySettings
{
    [TestFixture]
    internal class UpdateAsyncTests
    {
        [Test]
        public async Task UpdateAsync_BadRequest()
        {
            //Setup
            var userId = Guid.NewGuid();

            var model = new SecuritySettingUpdateViewModel(
                new SecuritySettingsSectionInputModel(Profiles.Model.Access.NoOne, Enumerable.Empty<Guid>().ToList()),
                new SecuritySettingsSectionInputModel(Profiles.Model.Access.NoOne, Enumerable.Empty<Guid>().ToList()),
                new SecuritySettingsSectionInputModel(Profiles.Model.Access.NoOne, Enumerable.Empty<Guid>().ToList()),
                new SecuritySettingsSectionInputModel(Profiles.Model.Access.NoOne, Enumerable.Empty<Guid>().ToList()),
                new SecuritySettingsSectionInputModel(Profiles.Model.Access.NoOne, Enumerable.Empty<Guid>().ToList()));

            var domainResult = DomainResult.Error(string.Empty);

            var currentProviderMock = new Mock<ICurrentUserProvider>();
            currentProviderMock.Setup(s => s.UserId)
                .Returns(userId.ToString());

            var serviceMock = new Mock<ISecuritySettingStorage>();
            serviceMock.Setup(s => s.UpdateAsync(userId, model)).ReturnsAsync(domainResult);

            var userStorage = new Mock<IUsersStorage>();
            userStorage.Setup(s => s.SecuritySettings).Returns(serviceMock.Object);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                collection.AddScoped(_ => userStorage.Object);
                collection.AddScoped(_ => currentProviderMock.Object);
            });

            //Act
            var response = await client.PutAsync($"/SecuritySettings", model.AsJsonContent());

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task UpdateAsync_NoContent()
        {
            //Setup
            var userId = Guid.NewGuid();

            var model = new SecuritySettingUpdateViewModel(
                new SecuritySettingsSectionInputModel(Profiles.Model.Access.NoOne, Enumerable.Empty<Guid>().ToList()),
                new SecuritySettingsSectionInputModel(Profiles.Model.Access.NoOne, Enumerable.Empty<Guid>().ToList()),
                new SecuritySettingsSectionInputModel(Profiles.Model.Access.NoOne, Enumerable.Empty<Guid>().ToList()),
                new SecuritySettingsSectionInputModel(Profiles.Model.Access.NoOne, Enumerable.Empty<Guid>().ToList()),
                new SecuritySettingsSectionInputModel(Profiles.Model.Access.NoOne, Enumerable.Empty<Guid>().ToList()));

            var domainResult = DomainResult.Success();

            var currentProviderMock = new Mock<ICurrentUserProvider>();
            currentProviderMock.Setup(s => s.UserId)
                .Returns(userId.ToString());

            var serviceMock = new Mock<ISecuritySettingStorage>();
            serviceMock.Setup(s => s.UpdateAsync(userId, model)).ReturnsAsync(domainResult);

            var userStorage = new Mock<IUsersStorage>();
            userStorage.Setup(s => s.SecuritySettings).Returns(serviceMock.Object);

            var client = TestServerHelper.New(collection =>
            {
                collection.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                collection.AddScoped(_ => userStorage.Object);
                collection.AddScoped(_ => currentProviderMock.Object);
            });

            //Act
            var response = await client.PutAsync($"/SecuritySettings", model.AsJsonContent());

            //Assert
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
