using MassTransit;
using Microsoft.Extensions.Logging;
using TagTheSpot.Services.Shared.Messaging.Events.Submissions;
using TagTheSpot.Services.Spot.Application.Abstractions.Data;
using TagTheSpot.Services.Spot.Domain.Spots;
using TagTheSpot.Services.Spot.Domain.Submissions;

namespace TagTheSpot.Services.Spot.Application.Consumers
{
    public sealed class SubmissionApprovedEventConsumer
        : IConsumer<SubmissionApprovedEvent>
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly ISpotRepository _spotRepository;
        private readonly Mapper<Submission, Domain.Spots.Spot> _mapper;
        private readonly ILogger<SubmissionApprovedEventConsumer> _logger;

        public SubmissionApprovedEventConsumer(
            ISubmissionRepository submissionRepository,
            ISpotRepository spotRepository,
            Mapper<Submission, Domain.Spots.Spot> mapper,
            ILogger<SubmissionApprovedEventConsumer> logger)
        {
            _submissionRepository = submissionRepository;
            _spotRepository = spotRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SubmissionApprovedEvent> context)
        {
            var message = context.Message;
            var cancellationToken = context.CancellationToken;
            var submissionId = message.SubmissionId;

            _logger.LogInformation("Consuming SubmissionApprovedEvent for submission with id: {SubmissionId}", submissionId);

            try
            {
                var submission = await _submissionRepository
                    .GetByIdAsync(
                        id: message.SubmissionId,
                        cancellationToken);

                if (submission is null)
                {
                    throw new InvalidOperationException($"Submission with id {submissionId} was not found in the storage. This indicates a possible data inconsistency.");
                }

                if (submission.Status != SubmissionStatus.Pending)
                {
                    throw new InvalidOperationException($"Submission with id {submissionId} is not in the pending state. This indicates a possible data inconsistency.");
                }

                submission.Status = SubmissionStatus.Approved;

                var spot = _mapper.Map(submission);

                await _submissionRepository.UpdateAsync(
                    submission, cancellationToken);

                await _spotRepository.InsertAsync(
                    spot, cancellationToken);

                _logger.LogInformation("Status of submission with id {SubmissionId} has been successfully approved. A respective spot entity was added.", submissionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while consuming SubmissionApprovedEvent for submission with id: {SubmissionId}", submissionId);

                throw;
            }
        }
    }
}
