using System.Collections.Concurrent;
using Core.Common.Enums;
using Mission_Service.Common.Enums;
using Mission_Service.Models;
using Mission_Service.Services.UAVStatusService.Interfaces;

namespace Mission_Service.Services.UAVStatusService
{
    public class UAVStatus : IUAVStatusService
    {
        private readonly ConcurrentDictionary<int, Mission> _uavActiveMissionsByTailId;

        public UAVStatus()
        {
            _uavActiveMissionsByTailId = new ConcurrentDictionary<int, Mission>();
        }

        public UAVType DetermineUAVType(Dictionary<TelemetryFields, double> telemetryData)
        {
            return telemetryData.ContainsKey(TelemetryFields.DataStorageUsedGB)
                ? UAVType.Surveillance
                : UAVType.Armed;
        }

        public bool IsInActiveMission(Dictionary<TelemetryFields, double> telemetryData)
        {
            bool hasSpeed = telemetryData.TryGetValue(
                TelemetryFields.CurrentSpeedKmph,
                out double currentSpeed
            );
            return hasSpeed && currentSpeed != 0;
        }

        public Mission? GetActiveMission(int tailId)
        {
            _uavActiveMissionsByTailId.TryGetValue(tailId, out Mission? activeMission);
            return activeMission;
        }

        public void SetActiveMission(int tailId, Mission mission)
        {
            _uavActiveMissionsByTailId.AddOrUpdate(
                tailId,
                mission,
                (uavTailId, existingMission) => mission
            );
        }

        public void ClearActiveMission(int tailId)
        {
            _uavActiveMissionsByTailId.TryRemove(tailId, out _);
        }
    }
}
