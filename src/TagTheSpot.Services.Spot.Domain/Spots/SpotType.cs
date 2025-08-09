using System.Text.Json.Serialization;

namespace TagTheSpot.Services.Spot.Domain.Spots
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SpotType
    {
        Street,
        Park,
        Dirt
    }
}
