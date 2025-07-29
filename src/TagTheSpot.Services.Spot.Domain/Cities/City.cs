namespace TagTheSpot.Services.Spot.Domain.Cities
{
    public sealed class City
    {
        public Guid Id { get; init; }

        public required string Name { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
