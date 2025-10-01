using System.ComponentModel.DataAnnotations;
using TagTheSpot.Services.Shared.Abstractions.Options;
using TagTheSpot.Services.Spot.Application.Options;

namespace TagTheSpot.Services.Spot.Infrastructure.Options
{
    public sealed class GroqApiSettings : IAppOptions
    {
        public static string SectionName => nameof(GroqApiSettings);

        [Required]
        [Url]
        [StringLength(200)]
        public required string CompletionsEndpoint { get; init; }

        [Required]
        [StringLength(100)]
        public required string Model { get; init; }

        [Required]
        public required string ApiKey { get; init; }
    }
}
