using CsvHelper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Globalization;
using TagTheSpot.Services.Spot.Domain.Cities;
using TagTheSpot.Services.Spot.Infrastructure.Options;

namespace TagTheSpot.Services.Spot.Infrastructure.Persistence
{
    public sealed class CityRepository : ICityRepository
    {
        private readonly List<City> _cities;

        public CityRepository(
            IHostEnvironment environment,
            IOptions<DataSettings> dataSettings)
        {
            var sourcePath = Path.Combine(environment.ContentRootPath,
                "..", dataSettings.Value.CityDataRelativePath);

            sourcePath = Path.GetFullPath(sourcePath);

            using var reader = new StreamReader(sourcePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            _cities = csv.GetRecords<City>().ToList();
        }

        public Task<IEnumerable<City>> GetMatchingCities(string cityPattern)
        {
            return Task.FromResult(_cities.Where(city => city.Name.StartsWith(cityPattern)));
        }
    }
}
