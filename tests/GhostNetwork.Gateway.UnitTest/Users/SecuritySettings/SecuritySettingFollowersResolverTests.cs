﻿using GhostNetwork.Gateway.Infrastructure.SecuritySettingResolver;
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
        private const string sectionName = "followers";

        [Test]
        public async Task ResolveFollowersAccess_Everyone_Ok()
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

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

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

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(true);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

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

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(true);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

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

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(false);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

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

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(true);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

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

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(false);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

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

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(true);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

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

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var relationStorageMock = new Mock<IRelationsApi>();
            relationStorageMock.Setup(x => x.IsFriendAsync(userId, currentUserId, default))
                .ReturnsAsync(false);

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(false);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

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

            var currentUserMock = new Mock<ICurrentUserProvider>();
            currentUserMock.Setup(x => x.UserId)
                .Returns(currentUserId.ToString());

            var securitySettingsMock = new Mock<ISecuritySettingStorage>();
            securitySettingsMock.Setup(x => x.CheckAccessAsync(currentUserId, userId, sectionName))
                .ReturnsAsync(false);

            var resolver = new SecuritySettingsFollowersResolver(
                securitySettingsMock.Object,
                currentUserMock.Object);

            // Act
            var result = await resolver.ResolveAccessAsync(userId);

            // Assert
            Assert.IsFalse(result.Successed);
        }
    }
}
