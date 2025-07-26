namespace TagTheSpot.Services.Spot.Application.DTO.Blobs
{
    public sealed record DownloadBlobResponse(
        Stream Stream,
        string ContentType);
}
