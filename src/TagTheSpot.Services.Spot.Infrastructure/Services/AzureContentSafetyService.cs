using Azure;
using Azure.AI.ContentSafety;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TagTheSpot.Services.Spot.Application.Abstractions.AI;
using TagTheSpot.Services.Spot.Infrastructure.Options;

namespace TagTheSpot.Services.Spot.Infrastructure.Services
{
    public sealed class AzureContentSafetyService : IContentSafetyService
    {
        private readonly AzureContentSafetySettings _settings;
        private readonly ContentSafetyClient _client;
        private readonly ILogger<AzureContentSafetyService> _logger;

        public AzureContentSafetyService(
            IOptions<AzureContentSafetySettings> settings,
            ILogger<AzureContentSafetyService> logger)
        {
            _settings = settings.Value;

            var credential = new AzureKeyCredential(_settings.ApiKey);
            _client = new ContentSafetyClient(new Uri(_settings.Endpoint), credential);

            _logger = logger;
        }

        public async Task<bool> IsTextSafeAsync(string text)
        {
            if (!_settings.Enabled)
            {
                return true;
            }

            var request = new AnalyzeTextOptions(text);

            try
            {
                var response = await _client.AnalyzeTextAsync(request);

                return AnalyzeTextResponse(response);
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "Analyze text failed.\nStatus code: {StatusCode}, Error code: {ErrorCode}, Error message: {Message}", ex.Status, ex.ErrorCode, ex.Message);

                return false;
            }
        }

        private bool AnalyzeTextResponse(
            Response<AnalyzeTextResult> response)
        {
            var hateSeverity = GetCategorySeverity(response, TextCategory.Hate);
            var sexualSeverity = GetCategorySeverity(response, TextCategory.Sexual);
            var selfHarmSeverity = GetCategorySeverity(response, TextCategory.SelfHarm);
            var violenceSeverity = GetCategorySeverity(response, TextCategory.Violence);

            _logger.LogInformation("Content Safety analysis results: Hate={Hate}, Sexual={Sexual}, Violence={Violence}, SelfHarm={SelfHarm}",
                hateSeverity, sexualSeverity, violenceSeverity, selfHarmSeverity);

            if (hateSeverity > _settings.HateThreshold)
            {
                _logger.LogWarning("Text blocked due to Hate severity ({Severity}) exceeding threshold ({Threshold})", 
                    hateSeverity, _settings.HateThreshold);

                return false;
            }

            if (sexualSeverity > _settings.SexualThreshold)
            {
                _logger.LogWarning("Text blocked due to Sexual severity ({Severity}) exceeding threshold ({Threshold})", 
                    sexualSeverity, _settings.SexualThreshold);

                return false;
            }

            if (violenceSeverity > _settings.ViolenceThreshold)
            {
                _logger.LogWarning("Text blocked due to Violence severity ({Severity}) exceeding threshold ({Threshold})", 
                    violenceSeverity, _settings.ViolenceThreshold);

                return false;
            }

            if (selfHarmSeverity > _settings.SelfHarmThreshold)
            {
                _logger.LogWarning("Text blocked due to SelfHarm severity ({Severity}) exceeding threshold ({Threshold})", 
                    selfHarmSeverity, _settings.SelfHarmThreshold);

                return false;
            }

            _logger.LogInformation("Text passed Content Safety checks.");

            return true;
        }

        private static int GetCategorySeverity(
            Azure.Response<AnalyzeTextResult> response,
            TextCategory category)
        {
            return response.Value.CategoriesAnalysis
                .FirstOrDefault(a => a.Category == category)?.Severity ?? 0;
        }
    }
}
