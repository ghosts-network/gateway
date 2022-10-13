using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GhostNetwork.Gateway.NewsFeed;

namespace GhostNetwork.Gateway.Infrastructure;

public class NewsFeedMediaStorage : INewsFeedMediaStorage
{
    private readonly BlobServiceClient blobClient;

    public NewsFeedMediaStorage(BlobServiceClient blobClient)
    {
        this.blobClient = blobClient;
    }

    public async Task<IEnumerable<string>> GetAllAsync()
    {
        var blobContainer = blobClient.GetBlobContainerClient("media");
        var blob = blobContainer.GetBlobs();
        return blob.Select(x => x.Name);
    }

    public async Task<IEnumerable<Media>> UploadAsync(IEnumerable<MediaStream> media, string userId, CancellationToken cancellationToken = default)
    {
        var links = new List<Media>();

        var blobContainer = blobClient.GetBlobContainerClient("media");
        await blobContainer.CreateIfNotExistsAsync(PublicAccessType.BlobContainer, cancellationToken: cancellationToken);

        foreach (var m in media)
        {
            var blob = blobContainer.GetBlobClient($"{userId}/{m.FileName}");
            await blob.UploadAsync(m.Stream, cancellationToken);
            links.Add(new Media(blob.Uri.ToString()));
        }

        return links;
    }

    public async Task DeleteAsync(IEnumerable<string> fileNames, CancellationToken cancellationToken = default)
    {
        var blobContainer = blobClient.GetBlobContainerClient("media");

        foreach (var fileName in fileNames)
        {
            var blob = blobContainer.GetBlobClient(fileName);

            await blob.DeleteAsync(DeleteSnapshotsOption.None, default, cancellationToken);
        }
    }
}