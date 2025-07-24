namespace TagTheSpot.Services.Spot.Application.DTO
{
    public sealed record CityResponse(
        Guid Id,
        string Name,
        double Latitude,
        double Longitude);
}
