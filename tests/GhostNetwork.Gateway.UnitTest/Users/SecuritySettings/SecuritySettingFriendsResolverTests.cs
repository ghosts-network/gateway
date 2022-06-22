using GhostNetwork.Gateway.Infrastructure.SecuritySettingResolver;
using GhostNetwork.Gateway.Users;
using GhostNetwork.Profiles.Api;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.UnitTest.Users.SecuritySettings
{
    [TestFixture]
    public class SecuritySettingFriendsResolverTests
    {
        private const string sectionName = "friends";

        [Test]
        public async Task ResolveFriendsAccess_Everyone_Ok()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(true);

            var resolver = new SecuritySettingsFriendsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task ResolveFriendsAccess_Is_CurrentUser_Ok()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = userId;

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(true);

            var resolver = new SecuritySettingsFriendsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task ResolveFriendsAccess_OnlyFriend_Ok()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(true);

            var resolver = new SecuritySettingsFriendsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task ResolveFriendsAccess_OnlyFriend_Error()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(false);

            var resolver = new SecuritySettingsFriendsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task ResolveFriendsAccess_Is_OnlyCertainUsers_Ok()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(true);

            var resolver = new SecuritySettingsFriendsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task ResolveFriendsAccess_Is_OnlyCertainUsers_Error()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(false);

            var resolver = new SecuritySettingsFriendsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task ResolveFriendsAccess_Is_ExceptCertainUsers_Ok()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(true);

            var resolver = new SecuritySettingsFriendsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task ResolveFriendsAccess_Is_ExceptCertainUsers_Error()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(false);

            var resolver = new SecuritySettingsFriendsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task ResolveFriendsAccess_NoOne_Error()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(false);

            var resolver = new SecuritySettingsFriendsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public void ResolveFriendsAccess_Parse_StringToGuid_Error()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(true);

            var resolver = new SecuritySettingsFriendsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

            // Act
            var exResult = Assert.Throws<FormatException>(() => resolver.ResolveAccessAsync("123").GetAwaiter().GetResult());

            // Assert
            Assert.AreEqual(exResult.GetType(), typeof(FormatException));
        }
    }
}
