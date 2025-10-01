using TagTheSpot.Services.Shared.Abstractions.Mappers;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;
using TagTheSpot.Services.Spot.Domain.Submissions;

namespace TagTheSpot.Services.Spot.Application.Mappers
{
    public sealed class SubmissionToSubmissionResponseMapper
        : Mapper<Submission, SubmissionResponse>
    {
        public override SubmissionResponse Map(Submission source)
        {
            return new SubmissionResponse(
                Id: source.Id,
                UserId: source.UserId,
                CityId: source.CityId,
                Latitude: source.Latitude,
                Longitude: source.Longitude,
                SpotType: source.Type.ToString(),
                Description: source.Description,
                SkillLevel: source.SkillLevel.ToString(),
                IsCovered: source.IsCovered,
                Lighting: source.Lighting,
                Accessibility: source.Accessibility.ToString(),
                Condition: source.Condition.ToString(),
                ImagesUrls: source.ImagesUrls,
                SubmissionStatus: source.Status.ToString(),
                RejectionReason: source.RejectionReason,
                SubmittedAt: source.SubmittedAt);
        }
    }
}
