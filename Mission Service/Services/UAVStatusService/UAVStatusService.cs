using System.Collections.Concurrent;
using Mission_Service.Common.Enums;
using Mission_Service.Models;
using Core.Common.Enums;

namespace Mission_Service.Services.UAVStatusService
{
    public class UAVStatusService : IUAVStatusService
    {
 private readonly ConcurrentDictionary<int, Mission> _activeMissions;

 public UAVStatusService()
        {
            _activeMissions = new ConcurrentDictionary<int, Mission>();
        }

        public UAVType DetermineUAVType(Dictionary<TelemetryFields, double> telemetryData)
      {
            return telemetryData.ContainsKey(TelemetryFields.DataStorageUsedGB)
     ? UAVType.Surveillance
          : UAVType.Armed;
      }

      public bool IsInActiveMission(Dictionary<TelemetryFields, double> telemetryData)
        {
  return telemetryData.TryGetValue(TelemetryFields.CurrentSpeedKmph, out double speed) && speed != 0;
  }

        public Mission? GetActiveMission(int tailId)
     {
     _activeMissions.TryGetValue(tailId, out Mission? mission);
            return mission;
        }

        public void SetActiveMission(int tailId, Mission mission)
        {
_activeMissions.AddOrUpdate(tailId, mission, (key, existing) => mission);
     }

        public void ClearActiveMission(int tailId)
      {
         _activeMissions.TryRemove(tailId, out _);
        }
    }
}
