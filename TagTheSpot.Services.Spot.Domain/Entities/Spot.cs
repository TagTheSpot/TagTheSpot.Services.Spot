using TagTheSpot.Services.Spot.Domain.Enums;

namespace TagTheSpot.Services.Spot.Domain.Entities
{
    public class Spot
    {
        public Guid Id { get; set; }
        public Guid CityId { get; set; }
        public string[] ImagesUrls { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public SpotType Type { get; set; }
        public string Description { get; set; }
        public SkillLevel? SkillLevel { get; set; }
        public bool? IsCovered { get; set; }
        public bool? Lighting { get; set; }
        public DateTime CreatedAt { get; set; }
        public Accessibility? Accessibility { get; set; }
        public Condition? Condition { get; set; }
    }
}
