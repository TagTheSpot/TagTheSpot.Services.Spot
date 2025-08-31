namespace TagTheSpot.Services.Spot.Application.DTO.UseCases
{
    public sealed record GetRandomSpotsByCityIdRequest(
        Guid CityId,
        int Count);
}
