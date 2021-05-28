using System;
using System.Net;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Api.NewsFeed;
using GhostNetwork.Gateway.NewsFeed;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Gateway.UnitTest.NewsFeed
{
    [TestFixture]
    public class UpdateAsyncTests : NewsFeedTestsBase
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

            var input = new CreateNewsFeedPublication {Content = "123"};

            // Act
            var response = await client.PutAsync($"/newsfeed/{publicationId}", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task Forbidden_Update_Others_Publication()
        {
            // Setup
            var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
            var currentUserId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C7");
            var publicationId = Guid.NewGuid().ToString();

            NewsFeedStorageMock
                .Setup(s => s.GetByIdAsync(publicationId))
                .ReturnsAsync(new NewsFeedPublication("", "", null, null, new UserInfo(userId, "", null)));

            CurrentUserProviderMock
                .Setup(s => s.UserId)
                .Returns(currentUserId.ToString());

            var client = TestServerHelper.New(collection =>
            {
                collection.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                collection.AddScoped(_ => NewsFeedStorageMock.Object);
                collection.AddScoped(_ => CurrentUserProviderMock.Object);
            });

            var input = new CreateNewsFeedPublication {Content = "123"};

            // Act
            var response = await client.PutAsync($"/newsfeed/{publicationId}", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Test]
        public async Task Publication_NoContent()
        {
            // Setup
            var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
            var publicationId = Guid.NewGuid().ToString();

            NewsFeedStorageMock
                .Setup(s => s.GetByIdAsync(publicationId))
                .ReturnsAsync(new NewsFeedPublication("", "", null, null, new UserInfo(userId, "", null)));

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

            var input = new CreateNewsFeedPublication {Content = "123"};

            // Act
            var response = await client.PutAsync($"/newsfeed/{publicationId}", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
