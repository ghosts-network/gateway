using GhostNetwork.Gateway.Infrastructure;
using GhostNetwork.Gateway.Users;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.UnitTest.Users.SecuritySettings
{
    [TestFixture]
    public class SecuritySettingResolverTests
    {
        [Test]
        public async Task ResolveFriendsAccess_Everyone_Ok()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var settings = SecuritySetting.DefaultForUser(userId);

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IUsersRelationsStorage>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId))
                .ReturnsAsync(true);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveFriendsAccessAsync(userId);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task ResolveFriendsAccess_Is_CurrentUser_Ok()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = userId;

            var settings = SecuritySetting.DefaultForUser(userId);

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IUsersRelationsStorage>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId))
                .ReturnsAsync(true);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveFriendsAccessAsync(userId);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task ResolveFriendsAccess_OnlyFriend_Ok()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var settings = new SecuritySetting(userId,
                new SecuritySettingSection(Access.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(Access.OnlyFriends, new List<Guid> { currentUserId }));

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IUsersRelationsStorage>();
            relationStorageMock.Setup(x => x.IsFriendAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(true);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveFriendsAccessAsync(userId);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task ResolveFriendsAccess_OnlyFriend_Error()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var settings = new SecuritySetting(userId,
                new SecuritySettingSection(Access.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(Access.OnlyFriends, new List<Guid> { currentUserId }));

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IUsersRelationsStorage>();
            relationStorageMock.Setup(x => x.IsFriendAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(false);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveFriendsAccessAsync(userId);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task ResolveFriendsAccess_Is_OnlyCertainUsers_Ok()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var settings = new SecuritySetting(userId,
                new SecuritySettingSection(Access.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(Access.OnlyCertainUsers, new List<Guid> { currentUserId }));

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IUsersRelationsStorage>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId))
                .ReturnsAsync(false);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveFriendsAccessAsync(userId);

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

            var settings = new SecuritySetting(userId,
                new SecuritySettingSection(Access.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(Access.OnlyCertainUsers, new List<Guid> { anotherUserId }));

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IUsersRelationsStorage>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId))
                .ReturnsAsync(false);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveFriendsAccessAsync(userId);

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

            var settings = new SecuritySetting(userId,
                new SecuritySettingSection(Access.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(Access.EveryoneExceptCertainUsers, new List<Guid> { anotherUserId }));

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IUsersRelationsStorage>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId))
                .ReturnsAsync(false);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveFriendsAccessAsync(userId);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task ResolveFriendsAccess_Is_ExceptCertainUsers_Error()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var settings = new SecuritySetting(userId,
                new SecuritySettingSection(Access.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(Access.EveryoneExceptCertainUsers, new List<Guid> { currentUserId }));

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IUsersRelationsStorage>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId))
                .ReturnsAsync(false);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveFriendsAccessAsync(userId);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task ResolveFriendsAccess_NoOne_Error()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var settings = new SecuritySetting(userId,
                new SecuritySettingSection(Access.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(Access.NoOne, Enumerable.Empty<Guid>()));

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IUsersRelationsStorage>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId))
                .ReturnsAsync(true);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveFriendsAccessAsync(userId);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task ResolveFriendsAccess_Parse_StringToGuid_Error()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var settings = new SecuritySetting(userId,
                new SecuritySettingSection(Access.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(Access.NoOne, Enumerable.Empty<Guid>()));

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IUsersRelationsStorage>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId))
                .ReturnsAsync(true);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var exResult = Assert.Throws<FormatException>(() => resolver.ResolveFriendsAccessAsync("123").GetAwaiter().GetResult());

            // Assert
            Assert.AreEqual(exResult.GetType(), typeof(FormatException));
        }
    }
}
