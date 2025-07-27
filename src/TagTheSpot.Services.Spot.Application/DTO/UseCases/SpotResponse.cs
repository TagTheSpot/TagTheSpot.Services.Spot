using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagTheSpot.Services.Spot.Domain.Spots;

namespace TagTheSpot.Services.Spot.Application.DTO.UseCases
{
    public sealed record SpotResponse(
        Guid Id,
        Guid CityId,
        List<string> ImagesUrls,
        double Latitude,
        double Longitude,
        SpotType Type,
        string Description,
        SkillLevel? SkillLevel,
        bool? IsCovered,
        bool? Lighting,
        DateTime CreatedAt,
        Accessibility? Accessibility,
        Condition? Condition);
}
