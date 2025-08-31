using Microsoft.AspNetCore.Mvc;
using TagTheSpot.Services.Spot.Application.Abstractions.Services;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;
using TagTheSpot.Services.Spot.WebAPI.Factories;

namespace TagTheSpot.Services.Spot.WebAPI.Controllers
{
    [Route("api/cities")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;
        private readonly ISpotService _spotService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public CitiesController(ICityService cityService, ISpotService spotService, ProblemDetailsFactory problemDetailsFactory)
        {
            _cityService = cityService;
            _spotService = spotService;
            _problemDetailsFactory = problemDetailsFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetMatchingCities(
            [FromQuery] GetMatchingCitiesRequest request)
        {
            var result = await _cityService.GetMatchingCitiesAsync(request);

            return Ok(result);
        }

        [HttpGet("{cityId}/spots")]
        public async Task<IActionResult> GetSpotsByCityId(Guid cityId)
        {
            var result = await _spotService.GetByCityIdAsync(cityId);

            return result.IsSuccess
                ? Ok(result.Value)
                : _problemDetailsFactory.GetProblemDetails(result);
        }
    }
}
