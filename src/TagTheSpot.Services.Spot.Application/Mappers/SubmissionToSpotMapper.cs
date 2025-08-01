using TagTheSpot.Services.Spot.Application.Abstractions.Data;
using TagTheSpot.Services.Spot.Domain.Submissions;

namespace TagTheSpot.Services.Spot.Application.Mappers
{
    public sealed class SubmissionToSpotMapper
        : Mapper<Submission, Domain.Spots.Spot>
    {
        public override Domain.Spots.Spot Map(Submission source)
        {
            return new Domain.Spots.Spot()
            {
                Id = Guid.NewGuid(),
                CityId = source.CityId,
                Description = source.Description,
                Type = source.Type,
                Latitude = source.Latitude,
                Longitude = source.Longitude,
                ImagesUrls = source.ImagesUrls,
                CreatedAt = DateTime.UtcNow,
                SkillLevel = source.SkillLevel,
                Accessibility = source.Accessibility,
                Condition = source.Condition,
                IsCovered = source.IsCovered,
                Lighting = source.Lighting,
            };
        }
    }
}
