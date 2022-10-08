using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GhostNetwork.Gateway.Users;
using GhostNetwork.Profiles.Api;
using GhostNetwork.Profiles.Model;

namespace GhostNetwork.Gateway.Infrastructure
{
    public class UsersPictureLocalStorage : IUsersPictureStorage
    {
        private readonly string rootDirectory;
        private readonly string server;
        private readonly IProfilesApi profilesApi;

        public UsersPictureLocalStorage(string rootDirectory, string server, IProfilesApi profilesApi)
        {
            this.rootDirectory = rootDirectory;
            this.server = server;
            this.profilesApi = profilesApi;
        }

        public async Task UploadAsync(
            Guid userId,
            string fileName,
            Stream stream,
            CancellationToken cancellationToken = default)
        {
            var userPhotosDirectory = Directory.CreateDirectory(Path.Combine(rootDirectory, "photos", userId.ToString()));
            await using (var fileStream = File.Create(Path.Combine(userPhotosDirectory.FullName, fileName)))
            {
                await stream.CopyToAsync(fileStream, cancellationToken);
            }

            var r = new Uri(new Uri(server, UriKind.Absolute), $"photos/{userId.ToString()}/{fileName}");
            var updateModel = new AvatarUpdateViewModel(r.ToString());
            await profilesApi.UpdateAvatarAsync(userId, updateModel, cancellationToken: cancellationToken);
        }
    }
}