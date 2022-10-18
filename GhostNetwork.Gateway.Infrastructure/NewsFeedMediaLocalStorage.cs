using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GhostNetwork.Gateway.NewsFeed;
using GhostNetwork.Profiles.Model;

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

    public async Task<IEnumerable<Media>> UploadAsync(IEnumerable<MediaStream> media, string userId, CancellationToken cancellationToken = default)
    {
        var links = new List<Media>();

        foreach (var m in media)
        {
            var userPhotosDirectory = Directory.CreateDirectory(Path.Combine(@"D:\gn", "media", userId));
            await using (var fileStream = File.Create(Path.Combine(userPhotosDirectory.FullName, m.FileName)))
            {
                await m.Stream.CopyToAsync(fileStream, cancellationToken);
            }

            var uri = new Uri(new Uri(server, UriKind.Absolute), $"media/{userId}/{m.FileName}");

            links.Add(new Media(uri.AbsoluteUri));
        }

        return links;
    }

    public async Task DeleteAsync(IEnumerable<string> fileNames, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }
}