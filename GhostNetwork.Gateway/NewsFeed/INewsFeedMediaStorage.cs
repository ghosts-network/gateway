using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.NewsFeed;

public interface INewsFeedMediaStorage
{
    Task<IEnumerable<Media>> UploadAsync(IEnumerable<MediaStream> media, string userId, CancellationToken cancellationToken = default);

    Task DeleteAsync(IEnumerable<string> fileNames, CancellationToken cancellationToken = default);
}