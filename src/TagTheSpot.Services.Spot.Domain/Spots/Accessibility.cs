using System.Text.Json.Serialization;

namespace TagTheSpot.Services.Spot.Domain.Spots
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Accessibility
    {
        Public,
        Private,
        DIY,
        Unknown,
        Chargeable
    }
}
