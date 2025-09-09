using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Collections.ObjectModel;
using TagTheSpot.Services.Spot.Application.Abstractions.Geo;
using TagTheSpot.Services.Spot.Domain.Cities;
using TagTheSpot.Services.Spot.Infrastructure.Options;

namespace TagTheSpot.Services.Spot.Infrastructure.Services
{
    public sealed class UAGeoValidationService : IGeoValidationService
    {
        private readonly GeometryFactory _geometryFactory = new();
        private readonly ReadOnlyDictionary<Guid, Geometry> _cityPolygons;
        private readonly ICityRepository _cityRepository;
        private readonly ILogger<UAGeoValidationService> _logger;

        public UAGeoValidationService(
            ICityRepository cityRepository,
            IOptions<DataSettings> dataSettings,
            ILogger<UAGeoValidationService> logger)
        {
            _cityRepository = cityRepository;
            _logger = logger;

            var geoJsonSourcePath = Path.GetFullPath(
                Path.Combine(AppContext.BaseDirectory, dataSettings.Value.UAGeoJsonRelativePath));

            _cityPolygons = MapCitiesToPolygons(geoJsonSourcePath);
        }

        public bool IsCoordinateWithinCity(
            double latitude,
            double longitude,
            Guid cityId)
        {
            if (!_cityPolygons.TryGetValue(cityId, out var polygon))
            {
                _logger.LogWarning("City with ID {CityId} has no associated polygon", cityId);

                return false;
            }

            var point = _geometryFactory.CreatePoint(new Coordinate(longitude, latitude));

            return polygon.Contains(point);
        }

        private ReadOnlyDictionary<Guid, Geometry> MapCitiesToPolygons(string filePath)
        {
            try
            {
                _logger.LogInformation("Loading GeoJSON polygons from {FilePath}", filePath);

                if (!File.Exists(filePath))
                {
                    _logger.LogError("GeoJSON file not found at {FilePath}", filePath);

                    throw new FileNotFoundException($"GeoJSON file not found at {filePath}");
                }

                var geoJson = File.ReadAllText(filePath);
                var reader = new GeoJsonReader();
                var features = reader.Read<FeatureCollection>(geoJson);

                var polygons = features
                    .Where(f => f.Geometry is not null)
                    .Select(f => f.Geometry)
                    .ToList();

                var result = new Dictionary<Guid, Geometry>();

                var cities = _cityRepository.GetAllAsync().Result;

                foreach (var city in cities)
                {
                    var cityPoint = _geometryFactory.CreatePoint(new Coordinate(city.Longitude, city.Latitude));

                    var polygon = polygons.FirstOrDefault(p => p.Contains(cityPoint));

                    if (polygon is not null)
                    {
                        result[city.Id] = polygon;

                        _logger.LogDebug("Mapped city {CityName} to polygon", city.Name);
                    }
                    else
                    {
                        _logger.LogWarning("No polygon found for city {CityName}", city.Name);
                    }
                }

                _logger.LogInformation("Mapped {Count} cities to polygons", result.Count);

                return new ReadOnlyDictionary<Guid, Geometry>(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to map cities to polygons from {FilePath}", filePath);

                throw;
            }
        }
    }
}
