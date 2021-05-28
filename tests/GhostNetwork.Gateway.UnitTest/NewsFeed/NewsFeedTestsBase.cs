using GhostNetwork.Gateway.Api.NewsFeed;
using GhostNetwork.Gateway.Facade;
using Moq;
using NUnit.Framework;

namespace GhostNetwork.Gateway.UnitTest.NewsFeed
{
    public abstract class NewsFeedTestsBase
    {
        protected readonly Mock<INewsFeedStorage> NewsFeedStorageMock = new();
        protected readonly Mock<INewsFeedReactionsStorage> NewsFeedReactionsStorageMock = new();
        protected readonly Mock<INewsFeedCommentsStorage> NewsFeedCommentsStorageMock = new();
        protected readonly Mock<ICurrentUserProvider> CurrentUserProviderMock = new();

        [SetUp]
        public void SetUp()
        {
            NewsFeedStorageMock
                .Setup(s => s.Reactions)
                .Returns(NewsFeedReactionsStorageMock.Object);

            NewsFeedStorageMock
                .Setup(s => s.Comments)
                .Returns(NewsFeedCommentsStorageMock.Object);
        }
    }
}