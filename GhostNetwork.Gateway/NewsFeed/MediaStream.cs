using System.IO;

namespace GhostNetwork.Gateway.NewsFeed;

public record MediaStream(string FileName, Stream Stream);