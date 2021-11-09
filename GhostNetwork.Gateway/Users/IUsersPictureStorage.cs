using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GhostNetwork.Gateway.Users
{
    public interface IUsersPictureStorage
    {
        Task UploadAsync(Guid userId, string fileName, Stream stream, CancellationToken cancellationToken = default);
    }
}