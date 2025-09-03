using TagTheSpot.Services.Spot.Application.Abstractions.Data;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;
using TagTheSpot.Services.Spot.Domain.Spots;
using TagTheSpot.Services.Spot.Domain.Submissions;

namespace TagTheSpot.Services.Spot.Application.Mappers
{
    public sealed class AddSubmissionRequestToSubmissionMapper
        : Mapper<AddSubmissionRequest, Submission>
    {
        public override Submission Map(AddSubmissionRequest source)
        {
            return new Submission()
            {
                Id = Guid.NewGuid(),
                Latitude = source.Latitude,
                Longitude = source.Longitude,
                Type = Enum.Parse<SpotType>(source.SpotType),
                Description = source.Description,
                SkillLevel = source.SkillLevel is null ? null : Enum.Parse<SkillLevel>(source.SkillLevel),
                IsCovered = source.IsCovered,
                Lighting = source.Lighting,
                Accessibility = source.Accessibility is null ? null : Enum.Parse<Accessibility>(source.Accessibility),
                Condition = source.Condition is null ? null : Enum.Parse<Condition>(source.Condition),
                SubmittedAt = DateTime.UtcNow
            }; 
        }
    }
}
