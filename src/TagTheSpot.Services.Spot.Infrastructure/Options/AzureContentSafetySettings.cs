using System.ComponentModel.DataAnnotations;

namespace TagTheSpot.Services.Spot.Infrastructure.Options
{
    public sealed class AzureContentSafetySettings
    {
        public const string SectionName = nameof(AzureContentSafetySettings);

        [Required]
        [Url]
        public required string Endpoint { get; init; }

        [Required]
        public required string ApiKey { get; init; }

        [Range(0, 7)]
        public int HateThreshold { get; init; } = 2;

        [Range(0, 7)]
        public int SexualThreshold { get; init; } = 1;

        [Range(0, 7)]
        public int ViolenceThreshold { get; init; } = 2;

        [Range(0, 7)]
        public int SelfHarmThreshold { get; init; } = 2;

        /// <summary>
        /// Set to false in development to skip moderation.
        /// </summary>
        public bool Enabled { get; init; } = true;
    }
}
