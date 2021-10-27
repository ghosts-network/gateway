using GhostNetwork.Content.Client;
using GhostNetwork.Gateway.Api.NewsFeed;
using GhostNetwork.Gateway.NewsFeed;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.UnitTest.NewsFeed
{
    [TestFixture]
    public class UpdateCommentAsyncTests : NewsFeedTestsBase
    {
        [Test]
        public async Task Comment_NotFound()
        {
            // Setup
            var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
            var commentId = Guid.NewGuid().ToString();

            const string content = "New Content";

            NewsFeedStorageMock
                .Setup(s => s.GetCommentByIdAsync(commentId))
                .ReturnsAsync(default(PublicationComment));

            var client = TestServerHelper.New(collection =>
            {
                collection.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
                collection.AddScoped(_ => NewsFeedStorageMock.Object);
            });

            var input = new UpdateNewsFeedComment { Content = content };

            // Act
            var response = await client.PutAsync($"/newsfeed/comments/{commentId}", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Test]
        public async Task Comment_Forbidden()
        {
            // Setup
            var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
            var publicationId = Guid.NewGuid().ToString();
            var commentId = Guid.NewGuid().ToString();

            const string content = "New Content";

            NewsFeedStorageMock
                .Setup(s => s.GetCommentByIdAsync(commentId))
                .ReturnsAsync(
                    new PublicationComment(
                        commentId,
                        content,
                        publicationId,
                        new UserInfo(Guid.NewGuid(), string.Empty, null),
                        DateTimeOffset.Now));

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

            var input = new UpdateNewsFeedComment { Content = content };

            // Act
            var response = await client.PutAsync($"/newsfeed/comments/{commentId}", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Test]
        public async Task Comment_BadRequest()
        {
            // Setup
            var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
            var publicationId = Guid.NewGuid().ToString();
            var commentId = Guid.NewGuid().ToString();

            const string oldContent = "Old Content";
            string newContent = string.Empty;

            NewsFeedStorageMock
                .Setup(s => s.GetCommentByIdAsync(commentId))
                .ReturnsAsync(
                    new PublicationComment(
                        commentId,
                        oldContent,
                        publicationId,
                        new UserInfo(userId, string.Empty, null),
                        DateTimeOffset.Now));

            NewsFeedStorageMock
                .Setup(s => s.UpdateCommentAsync(commentId, newContent))
                .ThrowsAsync(new ApiException { ErrorCode = (int)HttpStatusCode.BadRequest });

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

            var input = new UpdateNewsFeedComment { Content = newContent };

            // Act
            var response = await client.PutAsync($"/newsfeed/comments/{commentId}", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public async Task Comment_NoContent()
        {
            // Setup
            var userId = Guid.Parse("B4E69138-CE54-444A-8226-2CFABFD352C6");
            var publicationId = Guid.NewGuid().ToString();
            var commentId = Guid.NewGuid().ToString();

            const string oldContent = "Old Content";
            string newContent = "New Content";

            NewsFeedStorageMock
                .Setup(s => s.GetCommentByIdAsync(commentId))
                .ReturnsAsync(
                    new PublicationComment(
                        commentId,
                        oldContent,
                        publicationId,
                        new UserInfo(userId, string.Empty, null),
                        DateTimeOffset.Now));

            NewsFeedStorageMock
                .Setup(s => s.UpdateCommentAsync(commentId, newContent))
                .Returns(Task.CompletedTask);

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

            var input = new UpdateNewsFeedComment { Content = newContent };

            // Act
            var response = await client.PutAsync($"/newsfeed/comments/{commentId}", input.AsJsonContent());

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
