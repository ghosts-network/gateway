using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Gateway.NewsFeed;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Gateway.UnitTest.NewsFeed
{
    [TestFixture]
    public class RemoveReactionAsyncTests : NewsFeedTestsBase
    {
        [Test]
        public async Task Publication_NotFound()
        {
            // Setup
            var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
            var publicationId = Guid.NewGuid().ToString();

            NewsFeedStorageMock
                .Setup(s => s.GetByIdAsync(publicationId))
                .ReturnsAsync(default(NewsFeedPublication));

            CurrentUserProviderMock
                .Setup(s => s.UserId)
                .Returns(userId.ToString());

            var client = TestServerHelper.New(collection =>
            {
                collection.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                collection.AddScoped(_ => NewsFeedStorageMock.Object);
                collection.AddScoped(_ => CurrentUserProviderMock.Object);
            });

            // Act
            var response = await client.DeleteAsync($"/newsfeed/{publicationId}/reaction");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task Reaction_Added()
        {
            // Setup
            var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
            var publicationId = Guid.NewGuid().ToString();

            // NewsFeedStorageMock
            //     .Setup(s => s.GetByIdAsync(publicationId))
            //     .ReturnsAsync(new NewsFeedPublication("", "", default, default, null, null, new UserInfo(userId, "", null))); TODO

            NewsFeedReactionsStorageMock
                .Setup(s => s.RemoveAsync(publicationId, userId.ToString()))
                .ReturnsAsync(new ReactionShort(new Dictionary<ReactionType, int>(), new UserReaction(ReactionType.Angry)));

            CurrentUserProviderMock
                .Setup(s => s.UserId)
                .Returns(userId.ToString());

            var client = TestServerHelper.New(collection =>
            {
                collection.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                collection.AddScoped(_ => NewsFeedStorageMock.Object);
                collection.AddScoped(_ => CurrentUserProviderMock.Object);
            });

            // Act
            var response = await client.DeleteAsync($"/newsfeed/{publicationId}/reaction");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}