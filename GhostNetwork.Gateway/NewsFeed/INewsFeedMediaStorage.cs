using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.NewsFeed;

public interface INewsFeedMediaStorage
{
    Task UploadAsync(string fileName, Stream stream, CancellationToken cancellationToken = default);

    Task DeleteAsync(string fileName, CancellationToken cancellationToken = default);
}