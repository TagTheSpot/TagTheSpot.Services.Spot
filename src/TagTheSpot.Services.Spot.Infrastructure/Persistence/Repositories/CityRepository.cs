using CsvHelper;
using Microsoft.Extensions.Options;
using System.Globalization;
using TagTheSpot.Services.Spot.Domain.Cities;
using TagTheSpot.Services.Spot.Infrastructure.Options;

namespace TagTheSpot.Services.Spot.Infrastructure.Persistence.Repositories
{
    public sealed class CityRepository : ICityRepository
    {
        private readonly List<City> _cities;

        public CityRepository(
            IOptions<DataSettings> dataSettings)
        {
            var sourcePath = Path.Combine(AppContext.BaseDirectory, dataSettings.Value.CityDataRelativePath);

            sourcePath = Path.GetFullPath(sourcePath);

            using var reader = new StreamReader(sourcePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            _cities = csv.GetRecords<City>().ToList();
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            return Task.FromResult(_cities.Any(x => x.Id == id));
        }

        public Task<List<City>> GetAllAsync()
        {
            return Task.FromResult(_cities);
        }

        public Task<City?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_cities.FirstOrDefault(c => c.Id == id));
        }

        public Task<List<City>> GetMatchingCitiesAsync(string cityPattern)
        {
            return Task.FromResult(_cities.Where(city =>
                city.Name.StartsWith(cityPattern, StringComparison.OrdinalIgnoreCase)).ToList());
        }
    }
}
