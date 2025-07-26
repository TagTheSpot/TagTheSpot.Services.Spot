using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using TagTheSpot.Services.Spot.Application.Abstractions.Storage;
using TagTheSpot.Services.Spot.Application.DTO.Blobs;
using TraffiLearn.Infrastructure.External.Blobs.Options;

namespace TagTheSpot.Services.Spot.Infrastructure.Services
{
    public sealed class AzureBlobStorageService : IBlobService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly AzureBlobStorageSettings _storageSettings;

        public AzureBlobStorageService(
            BlobContainerClient containerClient,
            IOptions<AzureBlobStorageSettings> storageSettings)
        {
            _containerClient = containerClient;
            _storageSettings = storageSettings.Value;
        }

        public async Task DeleteAsync(
            string blobUri,
            CancellationToken cancellationToken = default)
        {
            var blobName = blobUri.Split(separator: '/', '\\').Last();

            var succeeded = await _containerClient.DeleteBlobIfExistsAsync(
                blobName,
                cancellationToken: cancellationToken);

            if (!succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to delete blob with URI" + blobUri + " from the storage.");
            }
        }

        public async Task<DownloadBlobResponse> DownloadAsync(
            string blobName,
            CancellationToken cancellationToken = default)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(blobName);

            var blobExists = await blobClient.ExistsAsync(cancellationToken);

            if (!blobExists)
            {
                throw new InvalidOperationException(
                    "Blob with the name " + blobName + " has not been found in the blob storage.");
            }

            var response = await blobClient.DownloadContentAsync(cancellationToken);

            return new DownloadBlobResponse(
                response.Value.Content.ToStream(),
                response.Value.Details.ContentType);
        }

        public async Task<UploadBlobResponse> UploadAsync(
            Stream stream,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            var blobName = Guid.NewGuid()
                               .ToString();

            BlobClient blobClient = _containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(
                stream,
                new BlobHttpHeaders
                {
                    ContentType = contentType
                },
                cancellationToken: cancellationToken);

            return new UploadBlobResponse(
                new Uri(_storageSettings.ImagesContainerUri),
                blobName);
        }
    }
}
