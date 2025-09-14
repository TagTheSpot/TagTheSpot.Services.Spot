using System.ComponentModel.DataAnnotations;

namespace TagTheSpot.Services.Spot.Infrastructure.Options
{
    public sealed class GroqApiSettings
    {
        public const string SectionName = nameof(GroqApiSettings);

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
