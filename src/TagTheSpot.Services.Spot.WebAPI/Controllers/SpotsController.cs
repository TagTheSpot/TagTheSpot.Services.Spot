using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TagTheSpot.Services.Spot.Application.Abstractions.Services;

namespace TagTheSpot.Services.Spot.WebAPI.Controllers
{
    [Route("api/spots")]
    public class SpotsController : ControllerBase
    {
        private readonly ISpotService _spotService;

        public SpotsController(ISpotService spotService)
        {
            _spotService = spotService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            Domain.Spots.Spot? spot = await _spotService.GetByIdAsync(id);

            return spot is not null ? Ok(spot) : NotFound();
        }
    }
}
