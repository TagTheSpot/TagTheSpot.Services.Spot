using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TagTheSpot.Services.Spot.Application.Abstractions.Services;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;
using TagTheSpot.Services.Spot.WebAPI.Factories;

namespace TagTheSpot.Services.Spot.WebAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class SpotsController : ControllerBase
    {
        private readonly ISpotService _spotService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public SpotsController(
            ISpotService spotService,
            ProblemDetailsFactory problemDetailsFactory)
        {
            _spotService = spotService;
            _problemDetailsFactory = problemDetailsFactory;
        }

        [HttpGet("spots/{id:guid}")]
        public async Task<IActionResult> GetSpotById(
            [FromRoute] Guid id)
        {
            var result = await _spotService.GetByIdAsync(id);

            return result.IsSuccess ?
                Ok(result.Value) : _problemDetailsFactory.GetProblemDetails(result);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpPost("spots")]
        public async Task<IActionResult> AddSpot(
            [FromForm] AddSpotRequest request)
        {
            var result = await _spotService.AddSpotAsync(request);

            return result.IsSuccess
                ? CreatedAtAction(
                    actionName: nameof(GetSpotById),
                    routeValues: new { id = result.Value },
                    value: result.Value)
                : _problemDetailsFactory.GetProblemDetails(result);
        }
    }
}
