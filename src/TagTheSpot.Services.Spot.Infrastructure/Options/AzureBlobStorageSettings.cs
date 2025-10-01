using System.ComponentModel.DataAnnotations;
using TagTheSpot.Services.Shared.Abstractions.Options;

namespace TagTheSpot.Services.Spot.Infrastructure.Options
{
    public sealed class AzureBlobStorageSettings : IAppOptions
    {
        public static string SectionName => nameof(AzureBlobStorageSettings);

        [Required]
        [StringLength(50)]
        public required string ContainerName { get; init; }

        [Required]
        public required string ConnectionString { get; init; }

        [Required]
        [Url]
        public required string ImagesContainerUri { get; init; }
    }
}
