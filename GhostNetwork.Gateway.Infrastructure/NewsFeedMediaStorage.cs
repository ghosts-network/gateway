using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using GhostNetwork.Gateway.NewsFeed;

namespace GhostNetwork.Gateway.Infrastructure;

public class NewsFeedMediaStorage : INewsFeedMediaStorage
{
    private readonly BlobServiceClient blob;

    public NewsFeedMediaStorage(BlobServiceClient blob)
    {
        this.blob = blob;
    }

    public Task UploadAsync(string fileName, Stream stream, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public Task DeleteAsync(string fileName, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}