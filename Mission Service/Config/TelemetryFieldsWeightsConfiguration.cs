using Core.Common.Enums;
using Mission_Service.Common.Enums;

namespace Mission_Service.Config
{
    public class TelemetryWeightsConfiguration
    {
        public Dictionary<UAVType, Dictionary<TelemetryFields, double>> Weights { get; set; } =
            new();

        public Dictionary<TelemetryFields, double> GetWeights(UAVType uavType)
        {
            return Weights.TryGetValue(uavType, out Dictionary<TelemetryFields, double>? weights)
                ? weights
                : new Dictionary<TelemetryFields, double>();
        }
    }
}
