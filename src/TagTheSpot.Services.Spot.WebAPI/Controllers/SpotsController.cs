using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TagTheSpot.Services.Spot.Application.Abstractions.Services;
using TagTheSpot.Services.Spot.Application.DTO.UseCases;
using TagTheSpot.Services.Spot.WebAPI.Factories;

namespace TagTheSpot.Services.Spot.WebAPI.Controllers
{
    [Route("api/spots")]
    [ApiController]
    public class SpotsController : ControllerBase
    {
        private readonly ISpotService _spotService;
        private readonly ISubmissionService _submissionService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public SpotsController(
            ISpotService spotService,
            ProblemDetailsFactory problemDetailsFactory,
            ISubmissionService submissionService)
        {
            _spotService = spotService;
            _problemDetailsFactory = problemDetailsFactory;
            _submissionService = submissionService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetSpotById(
            [FromRoute] Guid id)
        {
            var result = await _spotService.GetByIdAsync(id);

            return result.IsSuccess ?
                Ok(result.Value) : _problemDetailsFactory.GetProblemDetails(result);
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetRandomSpotsByCityId(
            [FromQuery] GetRandomSpotsByCityIdRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _spotService.GetRandomByCityIdAsync(
                request, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : _problemDetailsFactory.GetProblemDetails(result);
        }

        [Authorize(Roles = "Admin,Owner")]
        [HttpPost]
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

        [Authorize(Roles = "Owner")]
        [HttpDelete]
        public async Task<IActionResult> DeleteSpot(Guid id)
        {
            var result = await _spotService.DeleteSpotAsync(id);

            return result.IsSuccess
                ? Ok(result.Value)
                : _problemDetailsFactory.GetProblemDetails(result);
        }

        [Authorize]
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitSpot(
            [FromForm] AddSubmissionRequest request)
        {
            var result = await _submissionService.AddSubmissionAsync(request);

            return result.IsSuccess
                ? CreatedAtAction(
                    actionName: nameof(SubmissionsController.GetSubmissionById),
                    controllerName: "Submissions",
                    routeValues: new { submissionId = result.Value },
                    value: result.Value)
                : _problemDetailsFactory.GetProblemDetails(result);
        }
    }
}
