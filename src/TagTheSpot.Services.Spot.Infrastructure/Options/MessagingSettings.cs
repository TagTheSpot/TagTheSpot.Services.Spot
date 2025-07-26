using System.ComponentModel.DataAnnotations;

namespace TagTheSpot.Services.Spot.Infrastructure.Options
{
    public sealed class MessagingSettings
    {
        public const string SectionName = nameof(MessagingSettings);

        [Required]
        public required string QueueName { get; init; }
    }
}
