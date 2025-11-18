using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mission_Service.Common.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UAVType
    {
        Surveillance,
        Armed
    }
}
