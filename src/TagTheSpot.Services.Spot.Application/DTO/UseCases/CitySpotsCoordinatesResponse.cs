namespace TagTheSpot.Services.Spot.Application.DTO.UseCases
{
    public sealed record CitySpotsCoordinatesResponse(
        CityResponse City,
        List<CoordinatesResponse> SpotsCoordinates);
}
