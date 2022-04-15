using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GhostNetwork.Gateway.Users;
using GhostNetwork.Profiles.Api;
using GhostNetwork.Profiles.Model;

namespace GhostNetwork.Gateway.Infrastructure
{
    public class UsersPictureStorage : IUsersPictureStorage
    {
        private readonly BlobServiceClient blobClient;
        private readonly IProfilesApi profilesApi;

        public UsersPictureStorage(BlobServiceClient blobClient, IProfilesApi profilesApi)
        {
            this.blobClient = blobClient;
            this.profilesApi = profilesApi;
        }

        public async Task UploadAsync(
            Guid userId,
            string fileName,
            Stream stream,
            CancellationToken cancellationToken = default)
        {
            var user = await profilesApi.GetByIdAsync(userId, cancellationToken);

            var blobContainer = blobClient.GetBlobContainerClient("photos");
            await blobContainer.CreateIfNotExistsAsync(PublicAccessType.BlobContainer, cancellationToken: cancellationToken);

            var blob = blobContainer.GetBlobClient($"{userId.ToString()}/{fileName}");
            await blob.UploadAsync(stream, cancellationToken);

            var updateModel = new AvatarUpdateViewModel(blob.Uri.ToString());
            await profilesApi.UpdateAvatarAsync(userId, updateModel, cancellationToken);
        }
    }
}