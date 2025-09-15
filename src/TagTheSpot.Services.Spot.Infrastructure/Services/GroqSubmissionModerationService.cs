using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using TagTheSpot.Services.Spot.Application.Abstractions.AI;
using TagTheSpot.Services.Spot.Application.AI;
using TagTheSpot.Services.Spot.Application.DTO.AI;
using TagTheSpot.Services.Spot.Application.Options;
using TagTheSpot.Services.Spot.Domain.Submissions;
using TagTheSpot.Services.Spot.Infrastructure.Options;
using TagTheSpot.Services.Spot.Infrastructure.Services.DTO;

namespace TagTheSpot.Services.Spot.Infrastructure.Services
{
    public sealed class GroqSubmissionModerationService
        : ISubmissionModerationService
    {
        private readonly HttpClient _httpClient;
        private readonly GroqApiSettings _apiSettings;
        private readonly SubmissionModerationSettings _moderationSettings;

        public GroqSubmissionModerationService(
            HttpClient httpClient,
            IOptions<GroqApiSettings> apiSettings,
            IOptions<SubmissionModerationSettings> moderationSettings)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _moderationSettings = moderationSettings.Value;
        }

        public async Task<DescriptionModerationResult> ModerateDescriptionAsync(Submission submission)
        {
            if (!_moderationSettings.Enabled)
            {
                return new DescriptionModerationResult(true, true);
            }

            var prompt = PromptGenerator.GenerateModerateSubmissionDescriptionPrompt(submission);

            var requestPayload = BuildRequestPayload(prompt);

            var jsonResponse = await SendRequestAsync(requestPayload);

            var groqResponse = ParseGroqResponse(jsonResponse);

            return ParseAssistantContent(groqResponse);
        }

        private object BuildRequestPayload(string prompt)
        {
            return new
            {
                model = _apiSettings.Model,
                messages = new[] { new { role = "user", content = prompt } },
                temperature = 0,
                response_format = new { type = "json_object" },
                stream = false
            };
        }

        private async Task<string> SendRequestAsync(object payload)
        {
            var response = await _httpClient.PostAsJsonAsync(_apiSettings.CompletionsEndpoint, payload);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private GroqChatResponse ParseGroqResponse(string json)
        {
            var groqResponse = JsonSerializer.Deserialize<GroqChatResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (groqResponse?.Choices.Count == 0)
            {
                throw new InvalidOperationException("No choices returned from Groq.");
            }

            return groqResponse!;
        }

        private DescriptionModerationResult ParseAssistantContent(GroqChatResponse groqResponse)
        {
            var content = groqResponse.Choices[0].Message.Content;

            var result = JsonSerializer.Deserialize<DescriptionModerationResult>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result is null)
            {
                throw new InvalidOperationException("Failed to parse AI moderation result.");
            }

            return result;
        }
    }
}
