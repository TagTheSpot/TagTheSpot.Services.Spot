using TagTheSpot.Services.Spot.Application.DTO.AI;
using TagTheSpot.Services.Spot.Domain.Submissions;

namespace TagTheSpot.Services.Spot.Application.Abstractions.AI
{
    public interface ISubmissionModerationService
    {
        Task<DescriptionModerationResult> ModerateDescriptionAsync(Submission submission);
    }
}
