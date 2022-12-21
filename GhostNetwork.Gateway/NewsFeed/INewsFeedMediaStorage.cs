using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.NewsFeed;

public interface INewsFeedMediaStorage
{
    Task<IEnumerable<Media>> UploadAsync(IEnumerable<MediaStream> media, string publicationId, CancellationToken cancellationToken = default);

    Task DeleteManyAsync(IEnumerable<string> fileNames, string publicationId, CancellationToken cancellationToken = default);
}