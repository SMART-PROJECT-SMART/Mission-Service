
using Core.Common.Enums;
using Mission_Service.Common.Enums;

namespace Mission_Service.Config
{
    public class TelemetryWeightsConfig
    {
        public Dictionary<UAVType, Dictionary<TelemetryFields, double>> Weights { get; set; } = new();

        public Dictionary<TelemetryFields, double> GetWeights(UAVType uavType)
        {
            return Weights.TryGetValue(uavType, out var weights) ? weights : new Dictionary<TelemetryFields, double>();
        }
    }
}