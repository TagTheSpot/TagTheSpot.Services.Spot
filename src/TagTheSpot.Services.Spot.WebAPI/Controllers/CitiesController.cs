using Microsoft.AspNetCore.Mvc;
using TagTheSpot.Services.Spot.Application.Abstractions.Services;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;

namespace TagTheSpot.Services.Spot.WebAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;

        public CitiesController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpGet("cities")]
        public async Task<IActionResult> Register(
            [FromQuery] GetMatchingCitiesRequest request)
        {
            var result = await _cityService.GetMatchingCities(request);

            return Ok(result);
        }
    }
}
