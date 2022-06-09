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
    public class SecuritySettingFollowersResolverTests
    {
        [Test]
        public async Task ResolveFollowersAccess_Everyone_Ok()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var settings = SecuritySettingModel.DefaultForUser(userId);

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IRelationsApi>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId, default, default))
                .ReturnsAsync(true);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task ResolveFollowersAccess_Is_CurrentUser_Ok()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = userId;

            var settings = SecuritySettingModel.DefaultForUser(userId);

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IRelationsApi>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId, default, default))
                .ReturnsAsync(true);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task ResolveFollowersAccess_OnlyFriend_Ok()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var settings = new SecuritySettingModel(userId,
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.OnlyFriends, new List<Guid> { currentUserId }),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()));

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IRelationsApi>();
            relationStorageMock.Setup(x => x.IsFriendAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), default, default))
                .ReturnsAsync(true);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task ResolveFollowersAccess_OnlyFriend_Error()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var settings = new SecuritySettingModel(userId,
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.OnlyFriends, new List<Guid> { currentUserId }),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()));

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IRelationsApi>();
            relationStorageMock.Setup(x => x.IsFriendAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), default, default))
                .ReturnsAsync(false);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task ResolveFollowersAccess_Is_OnlyCertainUsers_Ok()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var settings = new SecuritySettingModel(userId,
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.OnlyCertainUsers, new List<Guid> { currentUserId }),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()));

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IRelationsApi>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId, default, default))
                .ReturnsAsync(false);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task ResolveFollowersAccess_Is_OnlyCertainUsers_Error()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();

            var settings = new SecuritySettingModel(userId,
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.OnlyCertainUsers, new List<Guid> { anotherUserId }),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()));

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IRelationsApi>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId, default, default))
                .ReturnsAsync(false);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task ResolveFollowersAccess_Is_ExceptCertainUsers_Ok()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var anotherUserId = Guid.NewGuid();

            var settings = new SecuritySettingModel(userId,
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.EveryoneExceptCertainUsers, new List<Guid> { anotherUserId }),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()));

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IRelationsApi>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId, default, default))
                .ReturnsAsync(false);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsTrue(result.Successed);
        }

        [Test]
        public async Task ResolveFollowersAccess_Is_ExceptCertainUsers_Error()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var settings = new SecuritySettingModel(userId,
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.EveryoneExceptCertainUsers, new List<Guid> { currentUserId }),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()));

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IRelationsApi>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId, default, default))
                .ReturnsAsync(false);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public async Task ResolveFollowersAccess_NoOne_Error()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var settings = new SecuritySettingModel(userId,
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()));

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IRelationsApi>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId, default, default))
                .ReturnsAsync(true);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsFalse(result.Successed);
        }

        [Test]
        public void ResolveFollowersAccess_Parse_StringToGuid_Error()
        {
            // Setup
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var settings = new SecuritySettingModel(userId,
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()),
                new SecuritySettingSection(AccessLevel.NoOne, Enumerable.Empty<Guid>()));

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IRelationsApi>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId, default, default))
                .ReturnsAsync(true);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.FindByProfileAsync(userId))
                .ReturnsAsync(settings);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object,
                relationStorageMock.Object);

            // Act
            var exResult = Assert.Throws<FormatException>(() => resolver.ResolveAccessAsync("123").GetAwaiter().GetResult());

            // Assert
            Assert.AreEqual(exResult.GetType(), typeof(FormatException));
        }
    }
}
