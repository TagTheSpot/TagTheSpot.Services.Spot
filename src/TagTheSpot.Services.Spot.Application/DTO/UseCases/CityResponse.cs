namespace TagTheSpot.Services.Spot.Application.DTO.UseCases
{
    public sealed record CityResponse(
        Guid Id,
        string Name,
        double Latitude,
        double Longitude);
}
