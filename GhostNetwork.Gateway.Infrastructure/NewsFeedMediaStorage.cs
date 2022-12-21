using System.Collections.Generic;
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

    public async Task<IEnumerable<Media>> UploadAsync(IEnumerable<MediaStream> media, string publicationId, CancellationToken cancellationToken = default)
    {
        var links = new List<Media>();

        var blobContainer = blobClient.GetBlobContainerClient("media");
        await blobContainer.CreateIfNotExistsAsync(PublicAccessType.BlobContainer, cancellationToken: cancellationToken);

        foreach (var m in media)
        {
            var blob = blobContainer.GetBlobClient($"{publicationId}/{m.FileName}");
            await blob.UploadAsync(m.Stream, cancellationToken);
            links.Add(new Media(blob.Uri.ToString()));
        }

        return links;
    }

    public async Task DeleteManyAsync(IEnumerable<string> fileNames, string publicationId, CancellationToken cancellationToken = default)
    {
        var blobContainer = blobClient.GetBlobContainerClient("media");

        foreach (var fileName in fileNames)
        {
            await blobContainer
                .GetBlobClient(fileName)
                .DeleteAsync(DeleteSnapshotsOption.None, default, cancellationToken);
        }
    }
}