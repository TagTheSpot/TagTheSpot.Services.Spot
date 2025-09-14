namespace TagTheSpot.Services.Spot.Application.AI
{
    public static class PromptGenerator
    {
        public static string GenerateModerateSpotDescriptionPrompt(Domain.Spots.Spot spot)
        {
            return $@"
                Moderate the spot description and return JSON only.

                Definitions:
                - Relevant: the text describes the spot and matches its fields; slang is acceptable.
                - Appropriate: the text is not offensive, vulgar, discriminatory, or insulting. Negative opinions or warnings are allowed.

                Spot details:
                Type: {spot.Type}
                Skill: {spot.SkillLevel?.ToString() ?? "?"}
                Covered: {spot.IsCovered?.ToString() ?? "?"}
                Light: {spot.Lighting?.ToString() ?? "?"}
                Access: {spot.Accessibility?.ToString() ?? "?"}
                Condition: {spot.Condition?.ToString() ?? "?"}

                Description: ""{spot.Description}""
                JSON output format:
                {{""isRelevant"": true|false, ""isAppropriate"": true|false}}
                ";
        }

    }
}
