namespace TagTheSpot.Services.Spot.Domain.Entities
{
    public sealed class City
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
