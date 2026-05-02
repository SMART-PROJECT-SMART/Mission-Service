using System.Collections.Concurrent;
using Core.Common.Enums;
using Mission_Service.Models;
using Mission_Service.Models.Ro;
using Mission_Service.Services.UAVStatusService.Interfaces;

namespace Mission_Service.Services.UAVStatusService
{
    public class UAVStatus : IUAVStatusService
    {
        private readonly ConcurrentDictionary<int, Mission> _uavActiveMissionsByTailId;

        public UAVStatus()
        {
            _uavActiveMissionsByTailId = new ConcurrentDictionary<int, Mission>();

            // TODO: Remove — temporary seed data for development
            _uavActiveMissionsByTailId.TryAdd(1, new Mission
            {
                Id = "seed-mission-1",
                Title = "Border Surveillance Alpha",
                RequiredUAVType = UAVType.Surveillance,
                Priority = Common.Enums.MissionPriority.High,
                Location = new Core.Models.Location(33.1, 33.4, 300),
                TimeWindow = new TimeWindow(DateTime.UtcNow, DateTime.UtcNow.AddHours(2))
            });
            _uavActiveMissionsByTailId.TryAdd(3, new Mission
            {
                Id = "seed-mission-2",
                Title = "Armed Patrol Bravo",
                RequiredUAVType = UAVType.Armed,
                Priority = Common.Enums.MissionPriority.Medium,
                Location = new Core.Models.Location(33.7, 36.6, 500),
                TimeWindow = new TimeWindow(DateTime.UtcNow.AddMinutes(30), DateTime.UtcNow.AddHours(3))
            });
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

        public IEnumerable<ActiveMissionRo> GetAllActiveMissions()
        {
            return _uavActiveMissionsByTailId.Select(entry => new ActiveMissionRo
            {
                TailId = entry.Key,
                Mission = entry.Value
            });
        }
    }
}
