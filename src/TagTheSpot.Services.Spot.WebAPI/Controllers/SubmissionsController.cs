using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TagTheSpot.Services.Spot.Application.Abstractions.Services;
using TagTheSpot.Services.Spot.WebAPI.Factories;

namespace TagTheSpot.Services.Spot.WebAPI.Controllers
{
    [Route("api/submissions")]
    [ApiController]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public SubmissionsController(
            ISubmissionService submissionService,
            ProblemDetailsFactory problemDetailsFactory)
        {
            _submissionService = submissionService;
            _problemDetailsFactory = problemDetailsFactory;
        }

        [Authorize]
        [HttpGet("mine")]
        public async Task<IActionResult> GetCurrentUserSubmissions(
            CancellationToken cancellationToken)
        {
            var result = await _submissionService.GetCurrentUserSubmissionsAsync(cancellationToken);

            return result.IsSuccess 
                ? Ok(result.Value) 
                : _problemDetailsFactory.GetProblemDetails(result);
        }

        [Authorize]
        [HttpGet("{submissionId:guid:required}")]
        public async Task<IActionResult> GetSubmissionById(
            [FromRoute] Guid submissionId,
            CancellationToken cancellationToken)
        {
            var result = await _submissionService.GetSubmissionByIdAsync(
                submissionId, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : _problemDetailsFactory.GetProblemDetails(result);
        }
    }
}
