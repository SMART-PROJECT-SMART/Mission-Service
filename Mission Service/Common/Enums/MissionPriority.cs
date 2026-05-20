using System.Text.Json.Serialization;

namespace Mission_Service.Common.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MissionPriority
    {
        Low,
        Medium,
        High,
    }
}
