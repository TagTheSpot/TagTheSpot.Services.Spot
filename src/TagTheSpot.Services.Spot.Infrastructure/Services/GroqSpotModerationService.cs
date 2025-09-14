using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using TagTheSpot.Services.Spot.Application.Abstractions.AI;
using TagTheSpot.Services.Spot.Application.AI;
using TagTheSpot.Services.Spot.Application.DTO.AI;
using TagTheSpot.Services.Spot.Application.Options;
using TagTheSpot.Services.Spot.Infrastructure.Options;
using TagTheSpot.Services.Spot.Infrastructure.Services.DTO;

namespace TagTheSpot.Services.Spot.Infrastructure.Services
{
    public sealed class GroqSpotModerationService
        : ISpotModerationService
    {
        private readonly HttpClient _httpClient;
        private readonly GroqApiSettings _apiSettings;
        private readonly SpotModerationSettings _moderationSettings;

        public GroqSpotModerationService(
            HttpClient httpClient, 
            IOptions<GroqApiSettings> apiSettings, 
            IOptions<SpotModerationSettings> moderationSettings)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _moderationSettings = moderationSettings.Value;
        }

        public async Task<DescriptionModerationResult> ModerateDescriptionAsync(
            Domain.Spots.Spot spot)
        {
            if (!_moderationSettings.Enabled)
            {
                return new DescriptionModerationResult(true, true);
            }

            var prompt = PromptGenerator.GenerateModerateSpotDescriptionPrompt(spot);

            var request = new
            {
                model = _apiSettings.Model,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(
                requestUri: _apiSettings.CompletionsEndpoint, 
                request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var groqResponse = JsonSerializer.Deserialize<GroqChatResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (groqResponse?.Choices.Count == 0)
            {
                throw new InvalidOperationException("No choices returned from Groq.");
            }

            var assistantContent = groqResponse!.Choices[0].Message.Content;

            var result = JsonSerializer.Deserialize<DescriptionModerationResult>(assistantContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
            {
                throw new InvalidOperationException("Failed to parse AI moderation result.");
            }

            return result;
        }
    }
}
