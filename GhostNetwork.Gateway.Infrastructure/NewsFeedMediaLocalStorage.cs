using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GhostNetwork.Gateway.NewsFeed;

namespace GhostNetwork.Gateway.Infrastructure;

public class NewsFeedMediaLocalStorage : INewsFeedMediaStorage
{
    private readonly string rootDirectory;
    private readonly string server;

    public NewsFeedMediaLocalStorage(string rootDirectory, string server)
    {
        this.rootDirectory = rootDirectory;
        this.server = server;
    }

    public async Task<IEnumerable<Media>> UploadAsync(IEnumerable<MediaStream> media, string publicationId, CancellationToken cancellationToken = default)
    {
        var links = new List<Media>();

        try
        {
            foreach (var m in media)
            {
                var userMediaDirectory = Directory.CreateDirectory(Path.Combine(@"D:\gn", "media", publicationId));

                await using (var fileStream = File.Create(Path.Combine(userMediaDirectory.FullName, m.FileName)))
                {
                    await m.Stream.CopyToAsync(fileStream, cancellationToken);
                }

                var uri = new Uri(new Uri(server, UriKind.Absolute), $"media/{publicationId}/{m.FileName}");

                links.Add(new Media(uri.AbsoluteUri));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return links;
    }

    public async Task DeleteManyAsync(IEnumerable<string> fileNames, string publicationId, CancellationToken cancellationToken = default)
    {
        var mediaDirectory = Path.Combine(@"D:\gn", "media", publicationId);

        try
        {
            foreach (var fileName in fileNames)
            {
                var t = fileName.Split('/').Last();
                File.Delete(Path.Combine(mediaDirectory, t));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}