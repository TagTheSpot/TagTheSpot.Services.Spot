namespace TagTheSpot.Services.Spot.Infrastructure.Services.DTO
{
    public sealed class GroqChatResponse
    {
        public List<Choice> Choices { get; set; } = [];

        public sealed class Choice
        {
            public Message Message { get; set; } = new();
        }

        public sealed class Message
        {
            public string Role { get; set; } = string.Empty;

            public string Content { get; set; } = string.Empty;
        }
    }
}
