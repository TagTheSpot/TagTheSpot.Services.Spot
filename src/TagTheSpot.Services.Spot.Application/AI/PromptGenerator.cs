using TagTheSpot.Services.Spot.Domain.Submissions;

namespace TagTheSpot.Services.Spot.Application.AI
{
    public static class PromptGenerator
    {
        public static string GenerateModerateSubmissionDescriptionPrompt(Submission submission)
        {
            return $@"
                Moderate the spot description and return JSON only.

                Definitions:
                - Relevant: The text must look like a real description of the spot. It must not contradict any of the spot's fields. Random, repeated, or placeholder text is NOT relevant.
                - Appropriate: The text must not contain offensive, vulgar, discriminatory, or insulting language in any language.

                Spot details:
                Type: {submission.Type}
                Skill: {submission.SkillLevel?.ToString() ?? "?"}
                Covered: {submission.IsCovered?.ToString() ?? "?"}
                Light: {submission.Lighting?.ToString() ?? "?"}
                Access: {submission.Accessibility?.ToString() ?? "?"}
                Condition: {submission.Condition?.ToString() ?? "?"}

                Description: ""{submission.Description}""
                JSON output format:
                {{""isRelevant"": true|false, ""isAppropriate"": true|false}}
                ";
        }

    }
}
