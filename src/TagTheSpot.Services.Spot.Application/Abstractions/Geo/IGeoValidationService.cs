namespace TagTheSpot.Services.Spot.Application.Abstractions.Geo
{
    public interface IGeoValidationService
    {
        bool IsCoordinateWithinCity(
            double latitude,
            double longitude,
            Guid cityId);
    }
}
