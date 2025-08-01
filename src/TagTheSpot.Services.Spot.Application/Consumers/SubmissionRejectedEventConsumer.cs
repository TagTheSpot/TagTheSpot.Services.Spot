using MassTransit;
using Microsoft.Extensions.Logging;
using TagTheSpot.Services.Shared.Messaging.Events.Submissions;
using TagTheSpot.Services.Spot.Domain.Submissions;

namespace TagTheSpot.Services.Spot.Application.Consumers
{
    public sealed class SubmissionRejectedEventConsumer
        : IConsumer<SubmissionRejectedEvent>
    {
        private readonly ISubmissionRepository _submissionRepository;
        private readonly ILogger<SubmissionRejectedEventConsumer> _logger;

        public SubmissionRejectedEventConsumer(
            ISubmissionRepository submissionRepository,
            ILogger<SubmissionRejectedEventConsumer> logger)
        {
            _submissionRepository = submissionRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SubmissionRejectedEvent> context)
        {
            var message = context.Message;
            var cancellationToken = context.CancellationToken;
            var submissionId = message.SubmissionId;

            _logger.LogInformation("Consuming SubmissionRejectedEvent for submission with id: {SubmissionId}", submissionId);

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

                submission.Status = SubmissionStatus.Rejected;

                await _submissionRepository.UpdateAsync(
                    submission, cancellationToken);

                _logger.LogInformation("Status of submission with id: {SubmissionId} is successfully set to Rejected.", submissionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while consuming SubmissionRejectedEvent for submission with id: {SubmissionId}", submissionId);

                throw;
            }
        }
    }
}
